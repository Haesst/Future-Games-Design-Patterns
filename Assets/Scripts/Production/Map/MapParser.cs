using System;
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

        char[,] charMap = new char[mapRows.Length, GetMapWidth(mapRows)];

        for(int x = 0; x < charMap.GetLength(0); x++)
        {
            for(int y = 0; y < charMap.GetLength(1); y++)
            {
                charMap[x, y] = mapRows[x][y];
            }
        }

        // Read the actual map
        // Get length and height of map
        int width = GetMapWidth(mapRows);
        int height = mapRows.Length;

        TileType[,] tiles = new TileType[charMap.GetLength(0), charMap.GetLength(1)];

        for(int x = 0; x < tiles.GetLength(0); x++)
        {
            for(int y = 0; y < tiles.GetLength(1); y++)
            {
                if (TileMethods.TypeByIdChar.ContainsKey(charMap[x,y]))
                {
                    tiles[x, y] = TileMethods.TypeByIdChar[charMap[x,y]];
                }
            }
        }

        Queue<BoxymonWave> boxymonWaves = new Queue<BoxymonWave>();

        string[] waveRows = GetMapRows(ref splittedMapFile[1]);

        foreach (var row in waveRows)
        {
            if(row.Length < 3)
            {
                continue;
            }

            string[] splittedRow = row.Split(' ');

            BoxymonWave currentWave = new BoxymonWave();
            currentWave.m_SmallBoxymons = Int32.Parse(splittedRow[0]);
            currentWave.m_BigBoxymons = Int32.Parse(splittedRow[1]);

            boxymonWaves.Enqueue(currentWave);
        }

        return new MapData(tiles, boxymonWaves);
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
