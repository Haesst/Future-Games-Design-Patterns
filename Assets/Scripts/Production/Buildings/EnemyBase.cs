using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] float m_TimeBetweenWaves;
    [SerializeField] float m_TimeBetweenSmallSpawns;
    [SerializeField] float m_TimeBetweenBigSpawns;

    public event Action<int> OnWaveChange;

    private MapData m_MapData;
    private GameObjectScriptablePool m_EnemyPool;
    private BoxymonWave? m_CurrentWave = null;
    private List<Boxymon> m_AliveBoxymons = new List<Boxymon>();

    private int m_SmallBoxymonsThisWave = 0;
    private int m_BigBoxymonsThisWave = 0;

    private float m_SmallBoxymonTimer = 0.0f;
    private float m_BigBoxymonTimer = 0.0f;

    private float m_WaveTimer = 0.0f;

    private int m_CurrentWaveNumber = 1;

    public void Init(MapData mapData, GameObjectScriptablePool enemyPool)
    {
        m_MapData = mapData;
        m_EnemyPool = enemyPool;
        m_CurrentWave = mapData.m_BoxymonWaves.Dequeue();
        OnWaveChange?.Invoke(m_CurrentWaveNumber);

        // Todo: Hook up boxymon as an observable and listen to their death to only spawn new waves when they're all dead
    }

    public void Update()
    {
        if(GameTime.m_IsPaused)
        {
            return;
        }

        if (m_MapData != null)
        {
            if (m_CurrentWave.HasValue)
            {
                if (m_WaveTimer > 0.0f)
                {
                    m_WaveTimer -= GameTime.m_DeltaTime;
                }
                else
                {
                    if (m_SmallBoxymonTimer > 0.0f)
                    {
                        m_SmallBoxymonTimer -= GameTime.m_DeltaTime;
                    }

                    if (m_BigBoxymonTimer > 0.0f)
                    {
                        m_BigBoxymonTimer -= GameTime.m_DeltaTime;
                    }

                    if (m_SmallBoxymonTimer <= 0.0f && m_SmallBoxymonsThisWave < m_CurrentWave.Value.m_SmallBoxymons)
                    {
                        SpawnBoxymon(BoxymonType.SmallBoxymon);
                        ++m_SmallBoxymonsThisWave;
                        m_SmallBoxymonTimer = m_TimeBetweenSmallSpawns;
                    }

                    if (m_BigBoxymonTimer <= 0.0f && m_BigBoxymonsThisWave < m_CurrentWave.Value.m_BigBoxymons)
                    {
                        SpawnBoxymon(BoxymonType.BigBoxymon);
                        ++m_BigBoxymonsThisWave;
                        m_BigBoxymonTimer = m_TimeBetweenBigSpawns;
                    }

                    if (m_SmallBoxymonsThisWave >= m_CurrentWave.Value.m_SmallBoxymons && m_BigBoxymonsThisWave >= m_CurrentWave.Value.m_BigBoxymons && m_AliveBoxymons.Count <= 0)
                    {
                        Debug.Log($"Wave Complete!\nS in wave: {m_CurrentWave.Value.m_SmallBoxymons} - Spawned: {m_SmallBoxymonsThisWave}\nL in wave: {m_CurrentWave.Value.m_BigBoxymons} - Spawned: {m_BigBoxymonsThisWave}");

                        m_WaveTimer = m_TimeBetweenWaves;
                        m_SmallBoxymonsThisWave = 0;
                        m_BigBoxymonsThisWave = 0;

                        if(m_MapData.m_BoxymonWaves.Count > 0)
                        {
                            m_CurrentWave = m_MapData.m_BoxymonWaves.Dequeue();
                            OnWaveChange.Invoke(++m_CurrentWaveNumber);
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
        instance.Init(boxymonType, m_MapData);
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

        m_CurrentWave = null;
        m_CurrentWaveNumber = 1;
        m_SmallBoxymonsThisWave = 0;
        m_BigBoxymonsThisWave = 0;
        m_SmallBoxymonTimer = 0;
        m_BigBoxymonTimer = 0;
    }

    private void OnDisable()
    {
        foreach (Boxymon boxymon in m_AliveBoxymons)
        {
            boxymon.OnBoxymonDeath -= BoxymonDied;
        }

        m_AliveBoxymons.Clear();
    }
}
