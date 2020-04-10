﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    [SerializeField] private float m_BaseHealth = 20f;

    private float m_CurrentHealth = 0.0f;

    public event Action<float> OnBaseHealthChanged;

    public void Init()
    {
        m_CurrentHealth = m_BaseHealth;
        OnBaseHealthChanged?.Invoke(m_BaseHealth);
    }

    public void TakeDamage(float damage)
    {
        m_CurrentHealth -= damage;
        OnBaseHealthChanged?.Invoke(m_CurrentHealth);

        if(m_CurrentHealth <= 0.0f)
        {
            Debug.Log("Game Over");
        }
        else
        {
            Debug.Log($"Health left: {m_CurrentHealth}");
        }
    }
}
