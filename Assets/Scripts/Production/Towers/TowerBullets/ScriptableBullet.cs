using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bullet", menuName = "ScriptableObject/Bullets/TowerBullets")]
public class ScriptableBullet : ScriptableObject
{
    [SerializeField] private Vector3 m_Scale;
    [SerializeField] private float m_Damage;
    [SerializeField] private float m_FreezeTime;
    [SerializeField] private float m_ExplosionRange;

    public Vector3 Scale => m_Scale;
    public float Damage => m_Damage;
    public float FreezeTime => m_FreezeTime;
    public float ExplosionRange => m_ExplosionRange;
}
