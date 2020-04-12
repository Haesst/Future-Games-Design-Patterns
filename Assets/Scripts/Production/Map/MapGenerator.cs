using System;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private MapBuilder m_MapBuilder = new MapBuilder();
    [SerializeField] private bool       m_LoadLevelOneOnPlay = true;

    private MapParser                   m_MapParser = new MapParser();
    private MapData                     m_MapData;
    private Queue<BoxymonWave>          m_BoxymonWaves;

    private Dictionary<int, TextAsset>  maps = new Dictionary<int, TextAsset>();

    public event Action<EnemyBase>      OnEnemyBaseLoaded;
    public event Action<PlayerBase>     OnPlayerBaseLoaded;

    [SerializeField] private int        mapCount = 0; // Used in custom editorscript

    private void Awake()
    {
        m_MapBuilder.InitPools();
        m_MapBuilder.OnEnemyBaseLoaded += EnemyBaseLoaded;
        m_MapBuilder.OnPlayerBaseLoaded += PlayerBaseLoaded;

        LoadAllMaps();

        if(m_LoadLevelOneOnPlay)
        {
            GenerateMap(0);
        }
    }

    public void OnDisable()
    {
        m_MapBuilder.OnEnemyBaseLoaded -= EnemyBaseLoaded;
        m_MapBuilder.OnPlayerBaseLoaded -= PlayerBaseLoaded;
    }

    public void PlayerBaseLoaded(PlayerBase playerBase)
    {
        OnPlayerBaseLoaded.Invoke(playerBase);
    }

    public void EnemyBaseLoaded(EnemyBase enemyBase)
    {
        OnEnemyBaseLoaded.Invoke(enemyBase);
    }

    public void GenerateMap(int mapIndex)
    {
        if (mapIndex == -1)
        {
            m_MapBuilder.CleanMap();
            m_MapData = null;
        }
        else
        {
            CreateMap(maps[mapIndex]);
        }
    }

    private void CreateMap(TextAsset map)
    {
        m_MapData = m_MapParser.ParseMap(map.text);

        m_MapBuilder.BuildMap(m_MapData);
    }

    private void LoadAllMaps()
    {
        UnityEngine.Object[] loadedMaps = Resources.LoadAll("MapSettings", typeof(TextAsset));

        for (int i = 0; i < loadedMaps.Length; i++)
        {
            maps.Add(i, (TextAsset)loadedMaps[i]);
            mapCount++;
        }
    }
}
