using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapBuilder
{
    [SerializeField] private GameObjectScriptablePool m_PathTilePool = default;
    [SerializeField] private GameObjectScriptablePool m_ObstacleTilePool = default;
    [SerializeField] private GameObjectScriptablePool m_TowerTilePool = default;
    [SerializeField] private GameObjectScriptablePool m_EnemyBasePool = default;
    [SerializeField] private GameObjectScriptablePool m_PlayerBasePool = default;
    [SerializeField] private GameObjectScriptablePool m_EnemyPool = default;

    private HashSet<GameObject> m_ActiveTiles = new HashSet<GameObject>();
    private MapData m_CurrentMapData;

    public event Action<EnemyBase> OnEnemyBaseLoaded;
    public event Action<PlayerBase> OnPlayerBaseLoaded;

    public void BuildMap(MapData mapData)
    {
        CleanMap();

        m_CurrentMapData = mapData;

        for (int x = 0; x < mapData.m_Tiles.GetLength(0); x++)
        {
            for (int y = 0; y < mapData.m_Tiles.GetLength(1); y++)
            {
                GameObject currentTile = RentGameObjectByTileType(mapData.m_Tiles[x, y]);
                currentTile.transform.position = mapData.TileToWorldPosition(x,y);

                m_ActiveTiles.Add(currentTile);
            }
        }
    }

    public void InitPools()
    {
        m_PathTilePool.InitPool();
        m_ObstacleTilePool.InitPool();
        m_TowerTilePool.InitPool();
        m_EnemyBasePool.InitPool();
        m_PlayerBasePool.InitPool();
        m_EnemyPool.InitPool();
    }

    private GameObject RentGameObjectByTileType(TileType type)
    {
        switch(type)
        {
            case TileType.Path:
                return m_PathTilePool.Rent(true);
            case TileType.Obstacle:
                return m_ObstacleTilePool.Rent(true);
            case TileType.TowerOne:
                GameObject towerOneInstance = m_TowerTilePool.Rent(true);
                towerOneInstance.GetComponent<Tower>().Init(TowerType.CannonTower);
                return towerOneInstance;
            case TileType.TowerTwo:
                GameObject towerTwoInstance = m_TowerTilePool.Rent(true);
                towerTwoInstance.GetComponent<Tower>().Init(TowerType.FreezeTower);
                return towerTwoInstance;
            case TileType.Start:
                EnemyBase enemyBaseInstance = m_EnemyBasePool.Rent(true).GetComponent<EnemyBase>();
                enemyBaseInstance.Init(m_CurrentMapData, m_EnemyPool);
                OnEnemyBaseLoaded.Invoke(enemyBaseInstance);
                return enemyBaseInstance.gameObject;
            case TileType.End:
                PlayerBase playerBaseInstance = m_PlayerBasePool.Rent(true).GetComponent<PlayerBase>();
                playerBaseInstance.Init();
                OnPlayerBaseLoaded.Invoke(playerBaseInstance);
                return playerBaseInstance.gameObject;
            default:
                return null;
        }
    }

    public void CleanMap()
    {
        foreach (GameObject tile in m_ActiveTiles)
        {
            if(tile.CompareTag("EnemyBase"))
            {
                tile.GetComponent<EnemyBase>().ClearBoxymonsOnMap();
            }

            tile.SetActive(false);
        }

        m_CurrentMapData = null;
    }
}
