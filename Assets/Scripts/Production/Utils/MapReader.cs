using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct MapKeyData
{
    public TileType Type { get; private set; }
    public GameObject Prefab { get; private set; }

    public MapKeyData(TileType type, GameObject prefab)
    {
        Type = type;
        Prefab = prefab;
    }
}

public class MapReader
{
    private readonly Dictionary<TileType, GameObject> prefabsById = new Dictionary<TileType, GameObject>();

    public MapReader(IEnumerable<MapKeyData> mapKeyData)
    {
        prefabsById.Clear();

        foreach (MapKeyData data in mapKeyData)
        {
            prefabsById.Add(data.Type, data.Prefab);
        }
    }
    public void ReadMap()
    {
        // Provide the map in char format

        // Create a new Map object (???)

        char currentTileChar = '0';
        TileType tileType = TileMethods.TypeByIdChar[currentTileChar];
        GameObject currentPrefab = prefabsById[tileType];
        GameObject.Instantiate(currentPrefab);
    }
}