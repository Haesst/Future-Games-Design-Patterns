using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MapData
{
    public TileType[,] tiles;
    public IEnumerable<int> unitCount;
    public List<Vector2Int> accessibles = new List<Vector2Int>();

    public Vector3 origin;
    public Vector2Int tileScale;

    public Vector2Int? Start { get; private set; }
    public Vector2Int? End { get; private set; }

    public MapData(TileType[,] tiles, IEnumerable<int> unitCount /* <- to be done */)
    {
        this.tiles = tiles;
        this.unitCount = unitCount;

        origin = Vector3.zero; // This need to move to arguments
        tileScale = new Vector2Int(2, 2); // This need to move to arguments

        for (int i = 0; i < tiles.GetLength(0); i++)
        {
            for(int j = 0; j < tiles.GetLength(1); j++)
            {
                if(tiles[i,j] == TileType.Start)
                {
                    Start = new Vector2Int(i, j);
                }
                if(tiles[i,j] == TileType.End)
                {
                    End = new Vector2Int(i, j);
                }
            }
        }

        Assert.IsTrue(Start.HasValue, "No start found in map!");
        Assert.IsTrue(End.HasValue, "No start found in map!");

        ReadAcceccibleTiles();
    }

    private void ReadAcceccibleTiles()
    {
        for (int x = 0; x < tiles.GetLength(0); x++)
        {
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                if (TileMethods.IsWalkable(tiles[x, y]))
                {
                    accessibles.Add(new Vector2Int(x, y));
                }
            }
        }
    }

    public Vector3 TileToWorldPosition(Vector2Int tilePosition)
    {
        return TileToWorldPosition(tilePosition.x, tilePosition.y);
    }

    public Vector3 TileToWorldPosition(int x, int y)
    {
        return new Vector3(x * tileScale.x, 0, y * tileScale.y);
    }

    public Vector2Int WorldToTilePosition(Vector3 worldPosition)
    {
        return new Vector2Int((int)(origin.x + (worldPosition.x / tileScale.x)), (int)(origin.z + (worldPosition.z / tileScale.y)));
    }
}
