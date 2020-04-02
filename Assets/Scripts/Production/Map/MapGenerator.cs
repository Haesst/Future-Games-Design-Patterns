using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("TilePool")]
    [SerializeField] private int tilesToGenerate = 100;
    [SerializeField] private TilePool tilePool = new TilePool();

    private MapParser mapParser = new MapParser();
    private MapBuilder mapBuilder = new MapBuilder();

    private Dictionary<int, TextAsset> maps = new Dictionary<int, TextAsset>();
    private GameObject inactiveTileParent;
    private GameObject mapParent;

    [SerializeField] private int mapCount = 0;

    private void Awake()
    {
        LoadAllMaps();
        CreateInactiveTileParent();
        CreateMapParent();
        GenerateTiles();
    }

    public void GenerateMap(int mapIndex)
    {
        if (mapIndex == -1)
        {
            tilePool.ResetMap();
        }
        else
        {
            GenerateMap(maps[mapIndex]);
        }
    }

    private void GenerateMap(TextAsset map)
    {
        MapData parsedMap = mapParser.ParseMap(map.text);

        mapBuilder.BuildMap(parsedMap, ref tilePool);
    }

    private void LoadAllMaps()
    {
        // Loading map resources
        Object[] loadedMaps = Resources.LoadAll("MapSettings", typeof(TextAsset));

        for (int i = 0; i < loadedMaps.Length; i++)
        {
            maps.Add(i, (TextAsset)loadedMaps[i]);
            mapCount++;
        }
    }

    private void CreateInactiveTileParent()
    {
        inactiveTileParent = new GameObject("Inactive Tile Parent");
        inactiveTileParent.transform.SetParent(transform);
    }

    private void CreateMapParent()
    {
        mapParent = new GameObject("Inactive Tile Parent");
        mapParent.transform.SetParent(transform);
    }

    private void GenerateTiles()
    {
        tilePool.Init(inactiveTileParent, mapParent);
        tilePool.GenerateTiles(tilesToGenerate);
    }

    // This will be removed later with a custom editor script
    // that'll render this part unecessary
    [System.Serializable]
    private enum MapFile
    {
        Map1,
        Map2,
        Map3
    }
}
