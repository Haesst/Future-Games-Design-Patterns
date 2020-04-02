using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MapData
{
    public TileType[,] tiles;
    public IEnumerable<int> unitCount;
    private List<Vector2Int> walkable;
    public Vector2Int? Start { get; private set; }
    public Vector2Int? End { get; private set; }

    public MapData(TileType[,] tiles, IEnumerable<int> unitCount)
    {
        this.tiles = tiles;
        this.unitCount = unitCount;

        walkable = new List<Vector2Int>();

        for(int i = 0; i < tiles.GetLength(0); i++)
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
    }

    /*
     * xPos = 
     * 
     * 
     * */
}
