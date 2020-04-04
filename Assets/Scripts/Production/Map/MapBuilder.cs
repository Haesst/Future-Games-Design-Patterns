using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapBuilder
{
    [SerializeField] private GameObjectScriptablePool m_PathTilePool = default;
    [SerializeField] private GameObjectScriptablePool m_ObstacleTilePool = default;
    [SerializeField] private GameObjectScriptablePool m_TowerTilePool = default;
    [SerializeField] private GameObjectScriptablePool m_StartPool = default;
    [SerializeField] private GameObjectScriptablePool m_EndPool = default;

    private HashSet<GameObject> activeTiles = new HashSet<GameObject>();

    public void BuildMap(MapData mapData)
    {
        CleanMap();

        for (int x = 0; x < mapData.tiles.GetLength(0); x++)
        {
            for (int y = 0; y < mapData.tiles.GetLength(1); y++)
            {
                GameObject currentTile = RentGameObjectByTileType(mapData.tiles[x, y]);
                currentTile.transform.position = mapData.TileToWorldPosition(x,y);

                activeTiles.Add(currentTile);
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
            case TileType.TowerTwo:
                return m_TowerTilePool.Rent(true);
            case TileType.Start:
                return m_StartPool.Rent(true);
            case TileType.End:
                return m_EndPool.Rent(true);
            default:
                return null;
        }
    }

    public void CleanMap()
    {
        foreach (GameObject tile in activeTiles)
        {
            tile.SetActive(false);
        }
    }
}
