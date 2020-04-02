using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBuilder
{
    public void BuildMap(MapData mapData, Dictionary<TileType, GameObject> prefabsById)
    {
        for(int y = 0; y < mapData.tiles.GetLength(0); y++)
        {
            for(int x = 0; x < mapData.tiles.GetLength(1); x++)
            {
                GameObject currentPrefab = prefabsById[mapData.tiles[y, x]];
                GameObject.Instantiate(currentPrefab, new Vector3(x * 2, 0, y * 2), Quaternion.identity);
            }
        }
    }

    public void BuildMap(MapData mapData, ref TilePool tilePool)
    {
        tilePool.ResetMap();

        for (int y = 0; y < mapData.tiles.GetLength(0); y++)
        {
            for (int x = 0; x < mapData.tiles.GetLength(1); x++)
            {
                GameObject currentTile = tilePool.GetObjectByTileType(mapData.tiles[y, x]);
                currentTile.transform.position = new Vector3(x * 2, 0, y * 2);
                //GameObject currentPrefab = prefabsById[mapData.tiles[y, x]];
                //GameObject.Instantiate(currentPrefab, new Vector3(x * 2, 0, y * 2), Quaternion.identity);
            }
        }
    }
}
