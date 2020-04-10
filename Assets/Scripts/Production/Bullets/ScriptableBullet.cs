using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bullet", menuName = "ScriptableObject/Bullets/TowerBullets")]
public class ScriptableBullet : ScriptableObject
{
    [SerializeField] private Vector3 m_Scale = default;
    [SerializeField] private float   m_Damage = 20.0f;
    [SerializeField] private float   m_FreezeTime = 1.0f;
    [SerializeField] private float   m_ExplosionRange = 4.0f;
    [SerializeField] private float   m_BulletSpeed = 50.0f;
    [SerializeField] private float   m_BulletLifeTime = 2.0f;
    [SerializeField] private bool    m_ShouldExplodeOnDeath = false;

    public Vector3 Scale             => m_Scale;
    public float Damage              => m_Damage;
    public float FreezeTime          => m_FreezeTime;
    public float ExplosionRange      => m_ExplosionRange;
    public float BulletSpeed         => m_BulletSpeed;
    public float BulletLifeTime      => m_BulletLifeTime;
    public bool ShouldExplodeOnDeath => m_ShouldExplodeOnDeath;
}
