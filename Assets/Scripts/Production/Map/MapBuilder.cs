using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBuilder
{
    public void BuildMap(MapData mapData, ref TilePool tilePool)
    {
        tilePool.ResetMap();

        for (int y = 0; y < mapData.tiles.GetLength(0); y++)
        {
            for (int x = 0; x < mapData.tiles.GetLength(1); x++)
            {
                GameObject currentTile = tilePool.GetObjectByTileType(mapData.tiles[y, x]);
                currentTile.transform.position = mapData.TileToWorldPosition(y,x);
            }
        }
    }
}
