using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] float           m_TimeBetweenWaves = 3.0f;

    private MapData                  m_MapData;
    private GameObjectScriptablePool m_EnemyPool;
    private BoxymonWave?             m_CurrentWave = null;
    private List<Boxymon>            m_AliveBoxymons = new List<Boxymon>();

    private float                    m_WaveTimer = 0.0f;
    private int                      m_CurrentWaveNumber = 1;

    public event Action<int>         OnWaveStart;
    public event Action<int>         OnWaveComplete;

    #region Unity Functions
    public void Update()
    {
        if(GameTime.IsPaused)
        {
            return;
        }

        if (m_MapData != null && m_CurrentWave.HasValue)
        {
            if (m_WaveTimer > 0.0f)
            {
                m_WaveTimer -= GameTime.DeltaTime;
            }
            else
            {
                RunWave();
                if (GetWaveIsDone() && m_AliveBoxymons.Count <= 0)
                {
                    CompleteWave();
                }
            }
        }
    }

    private void OnDisable()
    {
        foreach (Boxymon boxymon in m_AliveBoxymons)
        {
            boxymon.OnBoxymonDeath -= BoxymonDied;
        }

        m_AliveBoxymons.Clear();
    }

    #endregion Unity Functions

    public void Init(MapData mapData, GameObjectScriptablePool enemyPool)
    {
        m_MapData = mapData;
        m_EnemyPool = enemyPool;
        m_CurrentWave = mapData.m_BoxymonWaves.Dequeue();
        OnWaveStart?.Invoke(m_CurrentWaveNumber);

        if (mapData == null || enemyPool == null)
        {
            throw new InvalidOperationException("EnemyBase wasn't initialized correctly.");
        }
    }

    private void SpawnBoxymon(BoxymonType boxymonType)
    {
        Boxymon instance = m_EnemyPool.Rent(true).GetComponent<Boxymon>();
        instance.transform.position = transform.position;
        instance.Init(boxymonType, m_MapData, true);
        instance.OnBoxymonDeath += BoxymonDied;
        m_AliveBoxymons.Add(instance);
    }

    private void RunWave()
    {
        foreach (KeyValuePair<BoxymonType, WaveData> typeWavePair in m_CurrentWave.Value.m_Boxymons)
        {
            if (typeWavePair.Value.m_Timer > 0.0f)
            {
                typeWavePair.Value.SubtractFromTimer(GameTime.DeltaTime);
            }

            if (typeWavePair.Value.ReadyToSpawn())
            {
                SpawnBoxymon(typeWavePair.Key);
                typeWavePair.Value.SpawnDone(UnitMethods.TimeBetweenSpawnsByType[typeWavePair.Key]);
            }
        }
    }

    private bool GetWaveIsDone()
    {
        foreach (WaveData waveData in m_CurrentWave.Value.m_Boxymons.Values)
        {
            if(!waveData.WaveUnitDone())
            {
                return false;
            }
        }

        return true;
    }

    private void CompleteWave()
    {
        OnWaveComplete?.Invoke(m_CurrentWaveNumber);

        m_WaveTimer = m_TimeBetweenWaves;

        ResetWaveData();

        if (m_MapData.m_BoxymonWaves.Count > 0)
        {
            m_CurrentWave = m_MapData.m_BoxymonWaves.Dequeue();
            OnWaveStart?.Invoke(++m_CurrentWaveNumber);
        }
    }

    private void BoxymonDied(Boxymon boxymon)
    {
        m_AliveBoxymons.Remove(boxymon);
        boxymon.OnBoxymonDeath -= BoxymonDied;
    }

    public void ClearBoxymonsOnMap()
    {
        while(m_AliveBoxymons.Count > 0)
        {
            m_AliveBoxymons[0].gameObject.SetActive(false);
        }

        ResetWaveData();
        m_CurrentWave = null;
        m_CurrentWaveNumber = 1;
    }

    private void ResetWaveData()
    {
        foreach (KeyValuePair<BoxymonType, WaveData> typeWavePair in m_CurrentWave.Value.m_Boxymons)
        {
            typeWavePair.Value.ResetWaveData();
        }
    }
}
