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
    private ParticleSystem m_ParticleSystem;

    Collider[] explosionHits;

    #region Unity Functions
    private void Awake()
    {
        m_RigidBody = GetComponent<Rigidbody>();
        m_ParticleSystem = GetComponent<ParticleSystem>();

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

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tower"))
        {
            return;
        }

        if (other.CompareTag("Enemy"))
        {
            Boxymon boxymon = other.GetComponent<Boxymon>();

            if (boxymon.gameObject.activeSelf && m_CurrentScriptableBullet != null)
            {
                OnHit(other.transform.position, boxymon);
            }
        }

        OnHit(other.transform.position);
    }

    public void OnCollisionEnter(Collision collision)
    {
        OnHit(collision.transform.position);
    }

    #endregion Unity Functions

    public void Init(float initForce, Vector3 direction, BulletType bulletType)
    {
        m_RigidBody.velocity = direction * initForce;

        SetBulletType(bulletType);

        if(bulletType == BulletType.AoE)
        {
            explosionHits = new Collider[25];
        }
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

    private void OnHit(Vector3 impactPoint, Boxymon boxymon = null)
    {
        if (m_CurrentBulletType == BulletType.AoE)
        {
            AoEHit(impactPoint);
        }
        else
        {
            FreezeHit(boxymon);
        }

        gameObject.SetActive(false);
    }

    private void AoEHit(Vector3 impactPoint)
    {
        int collidersInsideArea = Physics.OverlapSphereNonAlloc(impactPoint, m_CurrentScriptableBullet.ExplosionRange, explosionHits);

        if(collidersInsideArea > 0)
        {
            foreach (Collider collider in explosionHits)
            {
                if (collider != null)
                {
                    if (collider.CompareTag("Enemy"))
                    {
                        Boxymon boxymon = collider.GetComponent<Boxymon>();
                        boxymon?.TakeBulletDamage(m_CurrentScriptableBullet.Damage, m_CurrentBulletType);
                    }
                }
            }
        }

        //m_ParticleSystem.Play(); <- Need to figure out how to play when object gets disabled. probably pool of just particlesystem
    }

    private void FreezeHit(Boxymon boxymon)
    {
        boxymon?.TakeBulletDamage(m_CurrentScriptableBullet.Damage, m_CurrentBulletType, m_CurrentScriptableBullet.FreezeTime);
    }
}
