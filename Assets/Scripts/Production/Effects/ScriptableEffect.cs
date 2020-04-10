using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="Effect", menuName ="ScriptableObject/Effect")]
public class ScriptableEffect : ScriptableObject
{
    [SerializeField] private float m_SpeedModifier = 0.0f;
    [SerializeField] private float m_Duration = 0.0f;

    public float SpeedModifier => m_SpeedModifier;
    private float Duration => m_Duration;
}
