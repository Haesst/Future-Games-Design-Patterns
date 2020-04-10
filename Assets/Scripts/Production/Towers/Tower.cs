using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TowerTypeWithScript
{
    public TowerType m_TowerType;
    public ScriptableTower m_ScriptableTower;
}

[SelectionBase]
public class Tower : MonoBehaviour
{
    [Tooltip("A tower type can only exist once in the array.")]
    [SerializeField] private TowerTypeWithScript[] m_TowerTypeWithScripts = default;

    [SerializeField] private Transform             m_TowerTopTransform = default;
    [SerializeField] private Transform             m_BulletSpawnPoint = default;
    [SerializeField] private MeshRenderer          m_TowerTopMeshRenderer = default;
    [SerializeField] private MeshRenderer          m_TowerBaseMeshRenderer = default;

    private Dictionary<TowerType, ScriptableTower> m_TowerTypeScriptableTower = new Dictionary<TowerType, ScriptableTower>();

    private TowerType                              m_CurrentTowerType;
    private ScriptableTower                        m_CurrentScriptableTower;
    private SphereCollider                         m_TowerRangeCollider;

    private List<Boxymon>                          m_BoxymonsInRange = new List<Boxymon>();
    private Transform                              m_ClosestBoxymon = null;
                                                  
    private float                                  m_ShotTimer = 0.0f;

    private void Awake()
    {
        try
        {
            foreach (TowerTypeWithScript couple in m_TowerTypeWithScripts)
            {
                m_TowerTypeScriptableTower.Add(couple.m_TowerType, couple.m_ScriptableTower);
            }
        }
        catch (Exception)
        {
            throw new InvalidOperationException("Multiple scriptable tower scripts set to a tower type");
        }
    }

    private void Update()
    {
        if(GameTime.IsPaused)
        {
            return;
        }

        if(m_ShotTimer > 0.0f)
        {
            m_ShotTimer -= GameTime.DeltaTime;
        }

        if(m_BoxymonsInRange.Count > 0)
        {
            ClearDisabledBoxymons();
            SetClosestBoxymon();

            if (m_ShotTimer <= 0.0f && m_BoxymonsInRange.Count > 0)
            {
                Fire();
            }
        }
    }

    private void FixedUpdate()
    {
        if(GameTime.IsPaused)
        {
            return;
        }

        if(m_ClosestBoxymon)
        {
            Vector3 newRotation = Vector3.RotateTowards(m_TowerTopTransform.forward, m_ClosestBoxymon.position - m_TowerTopTransform.position, m_CurrentScriptableTower.RotateAngleStepPerFrame * GameTime.DeltaTime, 0);
            newRotation.y = 0;
            m_TowerTopTransform.rotation = Quaternion.LookRotation(newRotation);
        }
    }

    private void Fire()
    {
        GameObject newBullet = m_CurrentScriptableTower.Bullet;
        newBullet.transform.position = m_BulletSpawnPoint.position;
        newBullet.transform.rotation = m_BulletSpawnPoint.rotation;

        Bullet bulletComp = newBullet.GetComponent<Bullet>();
        bulletComp.Init((m_ClosestBoxymon.transform.position - (m_BulletSpawnPoint.position + m_BulletSpawnPoint.forward)).normalized, m_CurrentScriptableTower.BulletType);
        m_ShotTimer = m_CurrentScriptableTower.TimeBetweenShots;
    }

    public void Init(TowerType towerType)
    {
        SetTowerType(towerType, true);

        m_TowerRangeCollider = GetComponent<SphereCollider>();
        m_TowerRangeCollider.radius = m_CurrentScriptableTower.TowerRange;
    }

    private void SetTowerType(TowerType towerType, bool forceMaterialUpdate = false)
    {
        if (towerType == m_CurrentTowerType 
                && m_CurrentScriptableTower != null
                && !forceMaterialUpdate)
        {
            return;
        }

        m_CurrentTowerType = towerType;
        m_CurrentScriptableTower = m_TowerTypeScriptableTower[towerType];

        UpdateMaterials();
    }

    private void UpdateMaterials()
    {
        m_TowerBaseMeshRenderer.material = m_CurrentScriptableTower.TowerBaseMaterial;
        m_TowerTopMeshRenderer.material = m_CurrentScriptableTower.TowerTopMaterial;
    }

    private void ClearDisabledBoxymons()
    {
        for(int i = 0; i < m_BoxymonsInRange.Count; i++)
        {
            if(!m_BoxymonsInRange[i].isActiveAndEnabled)
            {
                m_BoxymonsInRange.RemoveAt(i);
            }
        }
    }

    private void SetClosestBoxymon()
    {
        if(m_BoxymonsInRange.Count == 1)
        {
            m_ClosestBoxymon = m_BoxymonsInRange[0].transform;
        }

        float currentMinDistance = float.MaxValue;

        foreach (Boxymon boxymon in m_BoxymonsInRange)
        {
            float currentDistance = Vector3.Distance(transform.position, boxymon.transform.position);
            if (currentDistance < currentMinDistance)
            {
                m_ClosestBoxymon = boxymon.transform;
                currentMinDistance = currentDistance;
            }
        }
    }

    private void BoxymonDied(Boxymon boxymon)
    {
        m_BoxymonsInRange.Remove(boxymon);
        boxymon.OnBoxymonDeath -= BoxymonDied;

        if(m_BoxymonsInRange.Count <= 0)
        {
            m_ClosestBoxymon = null;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        Boxymon boxymon = other.GetComponent<Boxymon>();

        if (boxymon && !m_BoxymonsInRange.Contains(boxymon))
        {
            m_BoxymonsInRange.Add(boxymon);
            boxymon.OnBoxymonDeath += BoxymonDied;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        Boxymon boxymon = other.GetComponent<Boxymon>();

        if (boxymon && m_BoxymonsInRange.Contains(boxymon))
        {
            m_BoxymonsInRange.Remove(boxymon);

            if (m_BoxymonsInRange.Count <= 0)
            {
                m_ClosestBoxymon = null;
            }
            boxymon.OnBoxymonDeath -= BoxymonDied;
        }
    }

    private void OnDisable()
    {
        foreach (Boxymon boxymon in m_BoxymonsInRange)
        {
            boxymon.OnBoxymonDeath -= BoxymonDied;
        }

        m_BoxymonsInRange.Clear();
    }
}
