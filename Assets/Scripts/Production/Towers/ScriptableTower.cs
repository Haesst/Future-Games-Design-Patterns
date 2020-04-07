using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tower", menuName = "ScriptableObject/Towers/ShootingTower")]
public class ScriptableTower : ScriptableObject
{
    [SerializeField] private Material m_TowerBaseMaterial = default;
    [SerializeField] private Material m_TowerTopMaterial = default;
    [SerializeField] private float m_TowerRange = 4.0f;
    [SerializeField] private float m_RotateAngleStepPerFrame = 200.0f;

    public Material TowerBaseMaterial => m_TowerBaseMaterial;
    public Material TowerTopMaterial => m_TowerTopMaterial;
    public float TowerRange => m_TowerRange;
    public float RotateAngleStepPerFrame => m_RotateAngleStepPerFrame;
}
