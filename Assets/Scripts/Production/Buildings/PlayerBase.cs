using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    [SerializeField] private float m_BaseHealth = 20f;

    public void TakeDamage(float damage)
    {
        m_BaseHealth -= damage;

        if(m_BaseHealth <= 0.0f)
        {
            Debug.Log("Game Over");
        }
        else
        {
            Debug.Log($"Health left: {m_BaseHealth}");
        }
    }
}
