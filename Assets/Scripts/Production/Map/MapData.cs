using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MapData
{
    public TileType[,] m_Tiles;
    public IEnumerable<int> m_UnitCount;
    public List<Vector2Int> m_Accessibles = new List<Vector2Int>();

    public Vector3 m_Origin;
    public Vector2Int m_TileScale;

    public Vector2Int? Start { get; private set; }
    public Vector2Int? End { get; private set; }

    public MapData(TileType[,] tiles, IEnumerable<int> unitCount /* <- to be done */)
    {
        m_Tiles = tiles;
        m_UnitCount = unitCount;

        m_Origin = Vector3.zero; // This need to move to arguments
        m_TileScale = new Vector2Int(2, 2); // This need to move to arguments

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
        return new Vector3(x * m_TileScale.x, 0, y * m_TileScale.y);
    }

    public Vector2Int WorldToTilePosition(Vector3 worldPosition)
    {
        return new Vector2Int((int)(m_Origin.x + (worldPosition.x / m_TileScale.x)), (int)(m_Origin.z + (worldPosition.z / m_TileScale.y)));
    }
}
