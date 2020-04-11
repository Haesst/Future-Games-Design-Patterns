using JetBrains.Rider.Unity.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapParser
{
    private string[]    m_SplittedMapFile;
    private string[]    m_MapRows;
    private char[,]     m_CharMap;
    private TileType[,] m_Tiles;
    Queue<BoxymonWave>  m_BoxymonWaves = new Queue<BoxymonWave>();

    public MapData ParseMap(string mapFile)
    {
        m_SplittedMapFile = SplitMap(ref mapFile);
        m_MapRows = GetMapRows(ref m_SplittedMapFile[0]);
        m_BoxymonWaves.Clear();

        CreateCharMap();
        CreateTiles();
        CreateWaves();

        return new MapData(m_Tiles, m_BoxymonWaves);
    }

    private string[] SplitMap(ref string mapFile)
    {
        return mapFile.Split('#');
    }

    private string[] GetMapRows(ref string mapString)
    {
        return mapString.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
    }

    private void CreateCharMap()
    {
        m_CharMap = new char[m_MapRows.Length, GetMapWidth(m_MapRows)];

        for (int x = 0; x < m_CharMap.GetLength(0); x++)
        {
            for (int y = 0; y < m_CharMap.GetLength(1); y++)
            {
                m_CharMap[x, y] = m_MapRows[x][y];
            }
        }
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

    private void CreateTiles()
    {
        m_Tiles = new TileType[m_CharMap.GetLength(0), m_CharMap.GetLength(1)];

        for (int x = 0; x < m_Tiles.GetLength(0); x++)
        {
            for (int y = 0; y < m_Tiles.GetLength(1); y++)
            {
                if (TileMethods.TypeByIdChar.ContainsKey(m_CharMap[x, y]))
                {
                    m_Tiles[x, y] = TileMethods.TypeByIdChar[m_CharMap[x, y]];
                }
            }
        }
    }

    private void CreateWaves()
    {
        string[] waveRows = GetMapRows(ref m_SplittedMapFile[1]);

        foreach (var row in waveRows)
        {
            if (row.Length < 2)
            {
                continue;
            }

            string[] splittedRow = row.Split(' ');

            BoxymonWave currentWave = new BoxymonWave();
            currentWave.m_Boxymons = new Dictionary<BoxymonType, WaveData>();

            for (int i = 0; i < splittedRow.Length; i++)
            {
                currentWave.m_Boxymons.Add(UnitMethods.TypeById[i], new WaveData(uint.Parse(splittedRow[i])));
            }

            m_BoxymonWaves.Enqueue(currentWave);
        }
    }
}
