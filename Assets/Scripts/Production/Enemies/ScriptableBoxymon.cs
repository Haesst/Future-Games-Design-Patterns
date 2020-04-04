using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Boxymon", menuName = "ScriptableObject/Enemies/Boxymon")]
public class ScriptableBoxymon : ScriptableObject
{
    [SerializeField] private Material m_BodyMaterial;
    [SerializeField] private Material m_EyeMaterial;
    [SerializeField] private Vector3 m_Scale;

    [SerializeField] private float m_BaseSpeed;
    [SerializeField] private float m_RotateAngleStep;
    [SerializeField] private float m_Health;
    [SerializeField] private float m_Damage;

    public Material BodyMaterial => m_BodyMaterial;
    public Material EyeMaterial => m_EyeMaterial;
    public Vector3 Scale => m_Scale;
    public float BaseSpeed => m_BaseSpeed;
    public float RotateAngleStep => m_RotateAngleStep;
}
