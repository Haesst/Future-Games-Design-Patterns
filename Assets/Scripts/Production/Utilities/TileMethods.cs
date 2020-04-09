using System.Collections.Generic;
using UnityEngine;

public static class TileMethods
{
    public static IReadOnlyDictionary<int, TileType> TypeById { get; } = new Dictionary<int, TileType>
    {
        { 0,  TileType.Path },
        { 1,  TileType.Obstacle },
        { 2,  TileType.TowerOne },
        { 3,  TileType.TowerTwo},
        { 8,  TileType.Start },
        { 9,  TileType.End },
    };

    public static IReadOnlyDictionary<int, TileType> TypeByIdChar { get; } = new Dictionary<int, TileType>
    {
        { '0',  TileType.Path },
        { '1',  TileType.Obstacle },
        { '2',  TileType.TowerOne },
        { '3',  TileType.TowerTwo},
        { '8',  TileType.Start },
        { '9',  TileType.End },
    };

    public class ObjectConverter
    {
        private GameObject prefab;
        
        ObjectConverter(GameObject prefab)
        {
            this.prefab = prefab;
        }

        public GameObject GetPrefab()
        {
            return prefab;
        }
    }

    public static bool IsWalkable(TileType type)
    {
        return type == TileType.Path || type == TileType.Start || type == TileType.End;
    }
}
