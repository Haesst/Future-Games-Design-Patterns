using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Boxymon", menuName = "ScriptableObject/Enemies/Boxymon")]
public class ScriptableBoxymon : ScriptableObject
{
    [SerializeField] private Material m_BodyMaterial = default;
    [SerializeField] private Material m_EyeMaterial = default;
    [SerializeField] private Vector3 m_Scale = default;

    [SerializeField] private float m_BaseSpeed = default;
    [SerializeField] private float m_RotateAngleStep = default;
    [SerializeField] private float m_Health = 10;
    [SerializeField] private float m_Damage = 10;
    [SerializeField] private float m_TimeBetweenSpawns = 1.0f;

    public Material BodyMaterial => m_BodyMaterial;
    public Material EyeMaterial => m_EyeMaterial;
    public Vector3 Scale => m_Scale;
    public float BaseSpeed => m_BaseSpeed;
    public float RotateAngleStep => m_RotateAngleStep;
    public float Health => m_Health;
    public float Damage => m_Damage;
    public float TimeBetweenSpawns => m_TimeBetweenSpawns;
}
