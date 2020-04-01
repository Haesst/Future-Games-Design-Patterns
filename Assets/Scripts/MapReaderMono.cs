using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct MapKeyDataMono
{
    [SerializeField] private TileType type;
    [SerializeField] private GameObject prefab;

    public TileType Type => type;
    public GameObject Prefab => prefab;
}

public class MapReaderMono : MonoBehaviour
{
    [SerializeField] private MapKeyDataMono[] mapKeyDataMonos = default;

    private MapReader mapReader = default;
    private List<MapKeyData> data = new List<MapKeyData>();

    private void Awake()
    {
        data.Clear();

        foreach (MapKeyDataMono item in mapKeyDataMonos)
        {
            data.Add(new MapKeyData(item.Type, item.Prefab));
        }


        mapReader = new MapReader(data);

        mapReader.ReadMap();
    }
}
