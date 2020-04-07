using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tower", menuName = "ScriptableObject/Towers/ShootingTower")]
public class ScriptableTower : ScriptableObject
{
    [Header("Materials")]
    [SerializeField] private Material m_TowerBaseMaterial = default;
    [SerializeField] private Material m_TowerTopMaterial = default;
    [Header("Shooting")]
    [SerializeField] private float m_TowerRange = 4.0f;
    [SerializeField] private float m_RotateAngleStepPerFrame = 200.0f;
    [SerializeField] private float m_TimeBetweenShots = 1.5f;
    [SerializeField] private GameObjectScriptablePool m_BulletPool;
    [SerializeField] private BulletType m_BulletType;

    public Material TowerBaseMaterial => m_TowerBaseMaterial;
    public Material TowerTopMaterial => m_TowerTopMaterial;
    public float TowerRange => m_TowerRange;
    public float RotateAngleStepPerFrame => m_RotateAngleStepPerFrame;
    public float TimeBetweenShots => m_TimeBetweenShots;

    public GameObject Bullet => m_BulletPool.Rent(true);
    public BulletType BulletType => m_BulletType;
}