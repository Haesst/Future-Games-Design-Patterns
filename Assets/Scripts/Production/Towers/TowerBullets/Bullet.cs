using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct BulletTypeWithScript 
{
    public BulletType m_BulletType;
    public ScriptableBullet m_ScriptableBullet;
}

[Serializable]
public enum BulletType
{
    Freezing,
    AoE,
}

public class Bullet : MonoBehaviour
{
    [Tooltip("A bullet type can only exist once in the array.")]
    [SerializeField] private BulletTypeWithScript[] m_BulletTypeWithScripts = default;
    
    private Rigidbody m_RigidBody;
    private Dictionary<BulletType, ScriptableBullet> m_BulletScriptDictionary = new Dictionary<BulletType, ScriptableBullet>();

    private BulletType m_CurrentBulletType;
    private ScriptableBullet m_CurrentScriptableBullet;

    private void Awake()
    {
        m_RigidBody = GetComponent<Rigidbody>();

        try
        {
            foreach (BulletTypeWithScript couple in m_BulletTypeWithScripts)
            {
                m_BulletScriptDictionary.Add(couple.m_BulletType, couple.m_ScriptableBullet);
            }
        }
        catch (Exception)
        {
            throw new InvalidOperationException("Multiple scriptable bullet scripts set to a tower type");
        }
    }

    public void Init(float initForce, Vector3 direction, BulletType bulletType)
    {
        m_RigidBody.velocity = direction * initForce;

        SetBulletType(bulletType);
    }

    private void SetBulletType(BulletType bulletType)
    {
        if (bulletType == m_CurrentBulletType)
        {
            return;
        }

        m_CurrentBulletType = bulletType;
        m_CurrentScriptableBullet = m_BulletScriptDictionary[bulletType];
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Tower"))
        {
            return;
        }

        if(other.CompareTag("Enemy"))
        {
            Boxymon boxymon = other.GetComponentInParent<Boxymon>();

            if (boxymon.gameObject.activeSelf && m_CurrentScriptableBullet != null)
            {
                boxymon.TakeDamage(m_CurrentScriptableBullet.Damage);
            }

            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        gameObject.SetActive(false);
    }
}
