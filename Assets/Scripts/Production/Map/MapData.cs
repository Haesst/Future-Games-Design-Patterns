using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public struct BoxymonWave
{
    public int m_SmallBoxymons;
    public int m_BigBoxymons;
}
public class MapData
{
    public TileType[,] m_Tiles;
    public List<Vector2Int> m_Accessibles = new List<Vector2Int>();

    public Vector3 m_Origin;
    public Vector2Int m_TileScale;
    public Queue<BoxymonWave> m_BoxymonWaves = new Queue<BoxymonWave>();

    public Vector2Int? Start { get; private set; }
    public Vector2Int? End { get; private set; }

    public MapData(TileType[,] tiles, Queue<BoxymonWave> boxymonWaves)
    {
        m_Tiles = tiles;
        m_BoxymonWaves = boxymonWaves;

        m_Origin = new Vector3(-tiles.GetLength(0), 0.0f, -tiles.GetLength(1)); // This need to move to arguments
        m_TileScale = new Vector2Int(2, 2); // This need to move to arguments

        for (int x = 0; x < tiles.GetLength(0); x++)
        {
            for(int y = 0; y < tiles.GetLength(1); y++)
            {
                if(tiles[x,y] == TileType.Start)
                {
                    Start = new Vector2Int(x, y);
                }
                if(tiles[x,y] == TileType.End)
                {
                    End = new Vector2Int(x, y);
                }
            }
        }

        Assert.IsTrue(Start.HasValue, "No start found in map!");
        Assert.IsTrue(End.HasValue, "No start found in map!");

        ReadAcceccibleTiles();
    }

    private void ReadAcceccibleTiles()
    {
        for (int x = 0; x < m_Tiles.GetLength(0); x++)
        {
            for (int y = 0; y < m_Tiles.GetLength(1); y++)
            {
                if (TileMethods.IsWalkable(m_Tiles[x, y]))
                {
                    m_Accessibles.Add(new Vector2Int(x, y));
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
        return new Vector3(m_Origin.x + (x * m_TileScale.x), 0, m_Origin.z + (y * m_TileScale.y));
    }

    public Vector2Int WorldToTilePosition(Vector3 worldPosition)
    {
        return new Vector2Int(Mathf.CeilToInt((worldPosition.x - m_Origin.x) / m_TileScale.x), Mathf.CeilToInt((worldPosition.z - m_Origin.z) / m_TileScale.y));
    }
}
