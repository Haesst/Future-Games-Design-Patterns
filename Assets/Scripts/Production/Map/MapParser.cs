using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapParser
{
    public MapData ParseMap(string mapFile)
    {
        // Map file structure:
        // map
        // # (As separator)
        // Spawn info

        string[] splittedMapFile = SplitMap(ref mapFile);

        // Assert so the map really contains the right info

        string[] mapRows = GetMapRows(ref splittedMapFile[0]);

        // Read the actual map
        // Get length and height of map
        int width = GetMapWidth(mapRows);
        int height = mapRows.Length;

        TileType[,] tiles = new TileType[height, width];

        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                char c = mapRows[y][x];
                if (TileMethods.TypeByIdChar.ContainsKey(c))
                {
                    tiles[y, x] = TileMethods.TypeByIdChar[c];
                }
            }
        }

        return new MapData(tiles, new List<int>());
    }

    private string[] SplitMap(ref string mapFile)
    {
        return mapFile.Split('#');
    }

    private string[] GetMapRows(ref string mapString)
    {
        return mapString.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
    }

    private int GetMapWidth(string[] mapRows)
    {
        int maxWidth = int.MinValue;

        foreach (string row in mapRows)
        {
            if(row.Length > maxWidth)
            {
                maxWidth = row.Length;
            }
        }

        return maxWidth;
    }
}
