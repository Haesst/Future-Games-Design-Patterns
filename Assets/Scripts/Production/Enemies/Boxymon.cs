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

[SelectionBase]
public class Boxymon : MonoBehaviour, IEffectable
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

    [SerializeField] private float                                      m_CurrentHealth = 0.0f;
    [SerializeField] private float                                      m_CurrentSpeed = 0.0f;

    private const string m_PlayerBaseTag               = "PlayerBase";
    private const string m_AnimWalkingParam            = "isWalking";
    private const string m_AnimDamageParam             = "Damaged";
    private const string m_AnimKilledParam             = "Killed";

    public event Action<Boxymon>                       OnBoxymonDeath;
    private bool                                       m_InvokedDeathEvent = false;

    public float MoveSpeed { get => m_CurrentSpeed; set => m_CurrentSpeed = value; }
    public List<IEffect> ActiveEffects { get; private set; }

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
        ActiveEffects = new List<IEffect>();
    }

    void Update()
    {
        if(GameTime.IsPaused)
        {
            return;
        }

        if (m_MapData != null)
        {
            if (!m_GoingToPoint)
            {
                if (m_Path.Count <= 0)
                {
                    Vector2Int goalPosition = m_MapData.PlayerBase.GetValueOrDefault();
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

    public void TakeDamage(float damage, List<IEffect> effects)
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

            foreach (var effect in effects)
            {
                StartEffect(effect);
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

    public void StartEffect(IEffect effect)
    {
        if (ActiveEffectsContainsType(effect.GetType()))
        {
            if (!effect.Stackable)
            {
                return;
            }

            foreach (var activeEffect in ActiveEffects)
            {
                if(effect.GetType() == activeEffect.GetType() && activeEffect.TopStack == true)
                {
                    activeEffect.StackEffect(gameObject);
                    activeEffect.TopStack = false;
                }
            }
        }

        if(effect.Stackable)
        {
            effect.TopStack = true;
        }
        effect.DisableRoutine = StartCoroutine(ApplyEffect(effect));
        ActiveEffects.Add(effect);
    }

    private bool ActiveEffectsContainsType(Type effectType)
    {
        foreach (var effect in ActiveEffects)
        {
            if(effect.GetType() == effectType)
            {
                return true;
            }
        }

        return false;
    }

    public IEnumerator ApplyEffect(IEffect effect)
    {
        effect.ApplyEffect(gameObject);

        yield return new WaitForSeconds(effect.Duration);

        effect.DisableEffect(gameObject);
        ActiveEffects.Remove(effect);
    }
}
