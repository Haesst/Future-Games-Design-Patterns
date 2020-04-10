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

    public void Init(MapData mapData, GameObjectScriptablePool enemyPool)
    {
        m_MapData = mapData;
        m_EnemyPool = enemyPool;
        m_CurrentWave = mapData.m_BoxymonWaves.Dequeue();
        OnWaveStart?.Invoke(m_CurrentWaveNumber);
    }

    public void Update()
    {
        if(GameTime.IsPaused)
        {
            return;
        }

        if (m_MapData != null)
        {
            if (m_CurrentWave.HasValue)
            {
                if (m_WaveTimer > 0.0f)
                {
                    m_WaveTimer -= GameTime.DeltaTime;
                }
                else
                {
                    bool waveIsDone = true;

                    foreach (KeyValuePair<BoxymonType, WaveData> typeWavePair in m_CurrentWave.Value.m_Boxymons)
                    {
                        if (typeWavePair.Value.timer > 0.0f)
                        {
                            typeWavePair.Value.SubtractFromTimer(GameTime.DeltaTime);
                        }

                        if(typeWavePair.Value.ReadyToSpawn())
                        {
                            SpawnBoxymon(typeWavePair.Key);
                            typeWavePair.Value.SpawnDone(UnitMethods.TimeBetweenSpawnsByType[typeWavePair.Key]);
                        }

                        if(!typeWavePair.Value.WaveUnitDone())
                        {
                            waveIsDone = false;
                        }
                    }

                    if(waveIsDone && m_AliveBoxymons.Count <= 0)
                    {
                        OnWaveComplete?.Invoke(m_CurrentWaveNumber);
                        Debug.Log("Wave Done!");
                        
                        m_WaveTimer = m_TimeBetweenWaves;

                        ResetWaveData();

                        if (m_MapData.m_BoxymonWaves.Count > 0)
                        {
                            m_CurrentWave = m_MapData.m_BoxymonWaves.Dequeue();
                            OnWaveStart?.Invoke(++m_CurrentWaveNumber);
                        }
                    }
                }
            }
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

    private void OnDisable()
    {
        foreach (Boxymon boxymon in m_AliveBoxymons)
        {
            boxymon.OnBoxymonDeath -= BoxymonDied;
        }

        m_AliveBoxymons.Clear();
    }

    private void ResetWaveData()
    {
        foreach (KeyValuePair<BoxymonType, WaveData> typeWavePair in m_CurrentWave.Value.m_Boxymons)
        {
            typeWavePair.Value.ResetWaveData();
        }
    }
}
