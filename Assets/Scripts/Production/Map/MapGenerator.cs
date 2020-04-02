using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private MapData mapData;

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
            mapData = null;
        }
        else
        {
            GenerateMap(maps[mapIndex]);
        }
    }

    private void GenerateMap(TextAsset map)
    {
        mapData = mapParser.ParseMap(map.text);

        mapBuilder.BuildMap(mapData, ref tilePool);
    }

    private void LoadAllMaps()
    {
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
        mapParent = new GameObject("Map Parent");
        mapParent.transform.SetParent(transform);
    }

    private void GenerateTiles()
    {
        tilePool.Init(inactiveTileParent, mapParent);
        tilePool.GenerateTiles(tilesToGenerate);
    }

    // For debug enemies
    public MapData GetMapData()
    {
        return mapData;
    }

    // Debugging ----- Delete ------
    IPathFinder pathFinder;

    private void OnDrawGizmos()
    {
        if (mapData != null && mapData.accessibles != null)
        {
            foreach (var point in mapData.accessibles)
            {
                Vector3 worldPoint = new Vector3(point.y * 2, 0, point.x * 2);
                Gizmos.DrawWireCube(worldPoint, Vector3.one * 2);
            }

            // draw from start  to finish
            if (pathFinder == null)
            {
                pathFinder = new BreadthFirst(mapData.accessibles);
            }

            List<Vector2Int> path = new List<Vector2Int>(pathFinder.FindPath(mapData.Start.GetValueOrDefault(), mapData.End.GetValueOrDefault()));

            Color oldColor = Gizmos.color;
            Gizmos.color = Color.blue;
            foreach (var point in path)
            {
                Gizmos.DrawWireSphere(mapData.TileToWorldPosition(point), 0.5f);
            }
            Gizmos.color = oldColor;
        }
    }
}
