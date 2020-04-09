using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private MapBuilder mapBuilder = new MapBuilder();
    [SerializeField] private bool loadLevelOneOnPlay = true;

    private MapParser mapParser = new MapParser();
    private MapData mapData;

    private Dictionary<int, TextAsset> maps = new Dictionary<int, TextAsset>();
    private GameObject inactiveTileParent;
    private GameObject mapParent;


    [SerializeField] private int mapCount = 0;

    private void Awake()
    {
        LoadAllMaps();

        if(loadLevelOneOnPlay)
        {
            GenerateMap(0);
        }
    }

    public void GenerateMap(int mapIndex)
    {
        if (mapIndex == -1)
        {
            mapBuilder.CleanMap();
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

        mapBuilder.BuildMap(mapData);
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

    // For debug enemies
    public MapData GetMapData()
    {
        return mapData;
    }

    // Debugging
    IPathFinder pathFinder;

    private void OnDrawGizmos()
    {
        if (mapData != null && mapData.m_Accessibles != null)
        {
            foreach (var point in mapData.m_Accessibles)
            {
                Gizmos.DrawWireCube(mapData.TileToWorldPosition(point), Vector3.one * 2);
            }

            // draw from start  to finish
            if (pathFinder == null)
            {
                pathFinder = new BreadthFirst(mapData.m_Accessibles);
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
