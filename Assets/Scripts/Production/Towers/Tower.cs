using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TowerTypeWithScript
{
    public TowerType m_TowerType;
    public ScriptableTower m_ScriptableTower;
}

public enum TowerType
{
    CannonTower,
    FreezeTower
}

public class Tower : MonoBehaviour
{
    [Tooltip("A tower type can only exist once in the array.")]
    [SerializeField] private TowerTypeWithScript[] m_TowerTypeWithScripts = default;

    [SerializeField] private MeshRenderer m_TowerTop = default;
    [SerializeField] private MeshRenderer m_TowerBase = default;

    private Dictionary<TowerType, ScriptableTower> m_TowerTypeScriptableTower = new Dictionary<TowerType, ScriptableTower>();

    private TowerType m_CurrentTowerType;
    private ScriptableTower m_CurrentScriptableTower;

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

    public void Init(TowerType towerType)
    {
        SetTowerType(towerType, true);
    }

    private void SetTowerType(TowerType towerType, bool forceMaterialUpdate = false)
    {
        if (towerType == m_CurrentTowerType && !forceMaterialUpdate)
        {
            return;
        }

        m_CurrentTowerType = towerType;
        m_CurrentScriptableTower = m_TowerTypeScriptableTower[towerType];

        UpdateMaterials();
    }

    private void UpdateMaterials()
    {
        m_TowerBase.material = m_CurrentScriptableTower.TowerBaseMaterial;
        m_TowerTop.material = m_CurrentScriptableTower.TowerTopMaterial;
    }
}
