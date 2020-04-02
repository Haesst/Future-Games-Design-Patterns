using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*
[Serializable]
public class MapKeyDataMono
{
    [SerializeField] private TileType type = default;
    [SerializeField] private GameObject prefab = default;

    public TileType Type => type;
    public GameObject Prefab => prefab;
}

public class MapReaderMono : MonoBehaviour
{
    [SerializeField] private MapKeyDataMono[] mapKeyDataMonos = default;

    private MapReader mapReader = default;
    private MapParser mapParser = new MapParser();
    private MapBuilder mapBuilder = new MapBuilder();
    private readonly Dictionary<TileType, GameObject> prefabsById = new Dictionary<TileType, GameObject>();
    private List<MapKeyData> data = new List<MapKeyData>();

    private void Awake()
    {
        data.Clear();

        foreach (MapKeyDataMono mapKeyData in mapKeyDataMonos)
        {
            data.Add(new MapKeyData(mapKeyData.Type, mapKeyData.Prefab));
            prefabsById.Add(mapKeyData.Type, mapKeyData.Prefab);
        }


        mapReader = new MapReader(data);

        TextAsset mapAsset = LoadMapFile("MapSettings/map_2");
        MapData mapData = mapParser.ParseMap(mapAsset.ToString());

        mapBuilder.BuildMap(mapData, prefabsById);
    }

    private TextAsset LoadMapFile(string mapPath)
    {
        TextAsset txt = Resources.Load(mapPath) as TextAsset;
        return txt;
    }
}
*/