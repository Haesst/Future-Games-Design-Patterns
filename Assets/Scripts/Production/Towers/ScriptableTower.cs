using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tower", menuName = "ScriptableObject/Towers/ShootingTower")]
public class ScriptableTower : ScriptableObject
{
    [SerializeField] private Material m_TowerBaseMaterial;
    [SerializeField] private Material m_TowerTopMaterial;

    public Material TowerBaseMaterial => m_TowerBaseMaterial;
    public Material TowerTopMaterial => m_TowerTopMaterial;
}
