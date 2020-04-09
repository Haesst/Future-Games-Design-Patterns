using System.Collections;
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
                GameObject endInstance = m_EnemyBasePool.Rent(true);
                endInstance.GetComponent<EnemyBase>().Init(m_CurrentMapData, m_EnemyPool);
                return endInstance;
            case TileType.End:
                return m_PlayerBasePool.Rent(true);
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
