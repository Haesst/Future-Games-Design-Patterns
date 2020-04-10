using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct BoxymonTypeWithScript
{
    public BoxymonType m_BoxymonType;
    public ScriptableBoxymon m_ScriptableBoxymon;
}
public enum BoxymonType
{
    SmallBoxymon,
    BigBoxymon
}

[SelectionBase]
public class Boxymon : MonoBehaviour
{
    [Tooltip("A Boxymon type can only exist once in the array.")]
    [SerializeField] private BoxymonTypeWithScript[]   m_BoxymonTypeWithScripts = default;

    [Header("Bodyparts with material")]
    [SerializeField] private MeshRenderer              m_Chest = default;
    [SerializeField] private MeshRenderer              m_RightArm = default;
    [SerializeField] private MeshRenderer              m_LeftArm = default;
    [SerializeField] private MeshRenderer              m_RightLeg = default;
    [SerializeField] private MeshRenderer              m_LeftLeg = default;
    [SerializeField] private MeshRenderer              m_LeftEye = default;
    [SerializeField] private MeshRenderer              m_RightEye = default;

    private Dictionary<BoxymonType, ScriptableBoxymon> m_BoxymonTypeScriptDictionary = new Dictionary<BoxymonType, ScriptableBoxymon>();
    private BoxymonType                                m_CurrentBoxymonType;
    private ScriptableBoxymon                          m_CurrentScriptableBoxymon;
    

    private MapData                                    m_MapData = default;
    private IPathFinder                                m_PathFinder = default;
    private List<Vector2Int>                           m_Path = new List<Vector2Int>();
    private Vector3                                    m_NextPoint = default;
    private bool                                       m_GoingToPoint = false;
    
    private BoxCollider                                m_BoxCollider = default;
    private Animator                                   m_Animator = default;

    private float                                      m_CurrentHealth = 0.0f;
    private float                                      m_CurrentSpeed = 0.0f;
    private float                                      m_FreezeTimer = 0.0f;

    private const string m_PlayerBaseTag               = "PlayerBase";
    private const string m_AnimWalkingParam            = "isWalking";
    private const string m_AnimDamageParam             = "Damaged";
    private const string m_AnimKilledParam             = "Killed";

    public event Action<Boxymon>                       OnBoxymonDeath;
    private bool                                       m_InvokedDeathEvent = false;

    private void Awake()
    {
        try
        {
            foreach (BoxymonTypeWithScript couple in m_BoxymonTypeWithScripts)
            {
                m_BoxymonTypeScriptDictionary.Add(couple.m_BoxymonType, couple.m_ScriptableBoxymon);
            }
        }
        catch (Exception)
        {
            throw new InvalidOperationException("Multiple scriptable boxymons scripts set to a boxymon type");
        }

        m_BoxCollider = GetComponent<BoxCollider>();
        m_Animator = GetComponent<Animator>();
    }

    void Update()
    {
        if(GameTime.IsPaused)
        {
            return;
        }

        if (m_MapData != null)
        {
            if(m_FreezeTimer > 0)
            {
                m_FreezeTimer -= GameTime.DeltaTime;

                if(m_FreezeTimer <= 0)
                {
                    m_CurrentSpeed = m_CurrentScriptableBoxymon.BaseSpeed;
                }
            }

            if (!m_GoingToPoint)
            {
                if (m_Path.Count <= 0)
                {
                    Vector2Int goalPosition = m_MapData.End.GetValueOrDefault();
                    Vector2Int startPosition = m_MapData.WorldToTilePosition(transform.position);

                    m_Path.AddRange(m_PathFinder.FindPath(startPosition, goalPosition));
                }

                if (m_Path.Count > 0)
                {
                    m_NextPoint = m_MapData.TileToWorldPosition(m_Path[0]);

                    m_NextPoint.y = (m_BoxCollider.bounds.center - m_BoxCollider.bounds.min).y;

                    m_GoingToPoint = true;
                }
            }
            else
            {
                if (Vector3.Distance(transform.position, m_NextPoint) < 0.1f)
                {
                    m_Path.RemoveAt(0);
                    m_GoingToPoint = false;
                }
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(m_PlayerBaseTag))
        {
            PlayerBase playerBase = other.GetComponent<PlayerBase>();

            playerBase?.TakeDamage(m_CurrentScriptableBoxymon.Damage);
            gameObject.SetActive(false);
        }
    }

    public void FixedUpdate()
    {
        if (GameTime.IsPaused || m_CurrentHealth <= 0.0f)
        {
            return;
        }

        if (m_GoingToPoint)
        {
            if(!m_Animator.GetBool(m_AnimWalkingParam))
            {
                m_Animator.SetBool(m_AnimWalkingParam, true);
            }

            Vector3 newRotation = Vector3.RotateTowards(transform.forward, m_NextPoint - transform.position, GameTime.DeltaTime * m_CurrentScriptableBoxymon.RotateAngleStep, 0);
            transform.rotation = Quaternion.LookRotation(newRotation);
            transform.position = Vector3.MoveTowards(transform.position, m_NextPoint, m_CurrentSpeed * GameTime.DeltaTime);

        }
    }

    public void OnDisable()
    {
        m_Animator.SetBool(m_AnimKilledParam, false);
        StopAllCoroutines();

        if (this != null && !m_InvokedDeathEvent)
        {
            OnBoxymonDeath?.Invoke(this);
        }
    }

    public void Init(BoxymonType boxymonType, MapData mapData, bool forceDataUpdate = false, bool forceMaterialUpdate = false)
    {
        if (boxymonType == m_CurrentBoxymonType
                && m_CurrentScriptableBoxymon != null
                && !forceMaterialUpdate)
        {
            if(forceDataUpdate)
            {
                UpdateData(mapData);
            }

            if (forceMaterialUpdate)
            {
                UpdateMaterials();
            }

            return;
        }

        m_CurrentBoxymonType = boxymonType;
        m_CurrentScriptableBoxymon = m_BoxymonTypeScriptDictionary[boxymonType];

        UpdateData(mapData);
        UpdateMaterials();
    }

    private void UpdateData(MapData mapData)
    {
        m_InvokedDeathEvent = false;
        m_Animator.Rebind();
        transform.position = new Vector3(transform.position.x, (m_BoxCollider.bounds.center - m_BoxCollider.bounds.min).y, transform.position.z);

        m_MapData = mapData;
        m_PathFinder = new BreadthFirst(mapData.m_Accessibles);
        m_Path.Clear();
        m_GoingToPoint = false;
        m_NextPoint = Vector3.zero;
        m_CurrentHealth = m_CurrentScriptableBoxymon.Health;
        m_CurrentSpeed = m_CurrentScriptableBoxymon.BaseSpeed;
        transform.rotation = Quaternion.identity;
        transform.localScale = m_CurrentScriptableBoxymon.Scale;
    }

    private void UpdateMaterials()
    {
        UpdateBodyMaterials();
        UpdateEyeMaterials();
    }

    private void UpdateBodyMaterials()
    {
        m_Chest.material = m_CurrentScriptableBoxymon.BodyMaterial;
        m_RightArm.material = m_CurrentScriptableBoxymon.BodyMaterial;
        m_LeftArm.material = m_CurrentScriptableBoxymon.BodyMaterial;
        m_RightLeg.material = m_CurrentScriptableBoxymon.BodyMaterial;
        m_LeftLeg.material = m_CurrentScriptableBoxymon.BodyMaterial;
    }

    private void UpdateEyeMaterials()
    {
        m_LeftEye.material = m_CurrentScriptableBoxymon.EyeMaterial;
        m_RightEye.material = m_CurrentScriptableBoxymon.EyeMaterial;
    }

    public void TakeBulletDamage(float damage, BulletType bulletType, float freezeTime = 0.0f)
    {
        m_CurrentHealth -= damage;

        if (m_CurrentHealth <= 0.0f)
        {
            OnBoxymonDeath?.Invoke(this);
            m_InvokedDeathEvent = true;
            m_Animator.SetTrigger(m_AnimKilledParam);
            StartCoroutine(DisableBoxymonAfterTime(0.5f));
        }
        else
        {
            m_Animator.SetTrigger(m_AnimDamageParam);
            if (bulletType.Equals(BulletType.Freezing))
            {
                if (m_FreezeTimer <= 0)
                {
                    m_CurrentSpeed *= 0.5f;
                }

                m_FreezeTimer = freezeTime;
            }
        }
    }

    IEnumerator DisableBoxymonAfterTime(float time)
    {
        bool timerFinished = false;
        while (!timerFinished)
        {
            yield return new WaitForSeconds(time);
            timerFinished = true;
        }

        gameObject.SetActive(false);
    }
}
