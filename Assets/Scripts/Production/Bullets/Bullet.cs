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

public class Bullet : MonoBehaviour
{
    [Tooltip("A bullet type can only exist once in the array.")]
    [SerializeField] private BulletTypeWithScript[]  m_BulletTypeWithScripts = default;
    
    private Rigidbody                                m_RigidBody;
    private Vector3                                  m_Direction;
    private float                                    m_CurrentLifeTime = 0.0f;

    private Dictionary<BulletType, ScriptableBullet> m_BulletScriptDictionary = new Dictionary<BulletType, ScriptableBullet>();
    private BulletType                               m_CurrentBulletType;
    private ScriptableBullet                         m_CurrentScriptableBullet;
    private ParticleSystem                           m_ParticleSystem;

    private const string                             m_TowerTag = "Tower";
    private const string                             m_EnemyTag = "Enemy";

    Collider[]                                       m_ExplosionHits = new Collider[50];

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

    private void Update()
    {
        if(GameTime.IsPaused)
        {
            return;
        }

        m_CurrentLifeTime -= GameTime.DeltaTime;
        if(m_CurrentLifeTime <= 0.0f)
        {
            if(m_CurrentScriptableBullet.ShouldExplodeOnDeath)
            {
                OnHit(transform.position);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    private void FixedUpdate()
    {
        if (m_Direction != null && m_CurrentScriptableBullet != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + m_Direction, m_CurrentScriptableBullet.BulletSpeed * GameTime.DeltaTime);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(m_TowerTag))
        {
            return;
        }

        if (other.CompareTag(m_EnemyTag))
        {
            Boxymon boxymon = other.GetComponent<Boxymon>();

            if (boxymon.gameObject.activeSelf && m_CurrentScriptableBullet != null)
            {
                OnHit(other.transform.position, boxymon);
                return;
            }
        }

        OnHit(other.transform.position);
    }

    public void OnCollisionEnter(Collision collision)
    {
        OnHit(collision.transform.position);
    }

    #endregion Unity Functions

    public void Init(Vector3 direction, BulletType bulletType)
    {
        SetBulletType(bulletType);

        if(bulletType == BulletType.AoE)
        {
            for(int i = 0; i < m_ExplosionHits.Length; i++)
            {
                m_ExplosionHits[i] = null;
            }
        }

        m_Direction = direction;
        m_CurrentLifeTime = m_CurrentScriptableBullet.BulletLifeTime;
    }

    private void SetBulletType(BulletType bulletType)
    {
        if (bulletType == m_CurrentBulletType 
                && m_CurrentScriptableBullet != null)
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
        int collidersInsideArea = Physics.OverlapSphereNonAlloc(impactPoint, m_CurrentScriptableBullet.ExplosionRange, m_ExplosionHits);

        if(collidersInsideArea > 0)
        {
            foreach (Collider collider in m_ExplosionHits)
            {
                if (collider != null)
                {
                    if (collider.CompareTag(m_EnemyTag))
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
