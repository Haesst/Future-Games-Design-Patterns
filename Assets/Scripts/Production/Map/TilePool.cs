using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TilePool
{
    [SerializeField] private GameObject pathPrefab;
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private GameObject startPrefab = default;
    [SerializeField] private GameObject goalPrefab = default;

    private List<GameObject> activePaths = new List<GameObject>();
    private List<GameObject> inactivePaths = new List<GameObject>();

    private List<GameObject> activeObstacles = new List<GameObject>();
    private List<GameObject> inactiveObstacles = new List<GameObject>();

    private List<GameObject> activeTowers = new List<GameObject>();
    private List<GameObject> inactiveTowers = new List<GameObject>();

    private GameObject start;
    private GameObject goal;

    private GameObject inactiveTileParent;
    private GameObject mapParent;

    public void Init(GameObject inactiveTileParent, GameObject mapParent)
    {
        this.inactiveTileParent = inactiveTileParent;
        this.mapParent = mapParent;
    }

    public void GenerateTiles(int amountToGenerate)
    {
        for (int i = 0; i < amountToGenerate; i++)
        {
            GenerateTile(ref pathPrefab, ref inactivePaths);
            GenerateTile(ref obstaclePrefab, ref inactiveObstacles);
            GenerateTile(ref towerPrefab, ref inactiveTowers);
        }

        GenerateStart();
        GenerateGoal();
    }

    private GameObject GenerateTile(ref GameObject prefab, ref List<GameObject> list, bool active = false)
    {
        GameObject generatedGameObject = GameObject.Instantiate(prefab, inactiveTileParent.transform);
        list.Add(generatedGameObject);
        generatedGameObject.SetActive(active);

        return generatedGameObject;
    }

    private void GenerateStart()
    {
        start = GameObject.Instantiate(startPrefab, inactiveTileParent.transform);
        start.SetActive(false);
    }

    private void GenerateGoal()
    {
        goal = GameObject.Instantiate(goalPrefab, inactiveTileParent.transform);
        goal.SetActive(false);
    }

    public GameObject GetPathTile()
    {
        if (inactivePaths.Count > 0)
        {
            GameObject gameObject = inactivePaths[0];

            inactivePaths.RemoveAt(0);
            activePaths.Add(gameObject);

            gameObject.transform.SetParent(mapParent.transform);
            gameObject.SetActive(true);

            return gameObject;
        }
        else
        {
            Debug.Log($"Inactive Paths is empty, generating new tile");
            return GenerateTile(ref pathPrefab, ref activePaths, true);
        }
    }

    public GameObject GetObstacleTile()
    {
        if (inactiveObstacles.Count > 0)
        {
            GameObject gameObject = inactiveObstacles[0];

            inactiveObstacles.RemoveAt(0);
            activeObstacles.Add(gameObject);

            gameObject.transform.SetParent(mapParent.transform);
            gameObject.SetActive(true);

            return gameObject;
        }
        else
        {
            Debug.Log($"Inactive Obstacles is empty, generating new tile");
            return GenerateTile(ref obstaclePrefab, ref activeObstacles, true);
        }
    }

    public GameObject GetTowerTile()
    {
        if (inactiveTowers.Count > 0)
        {
            GameObject gameObject = inactiveTowers[0];

            inactiveTowers.RemoveAt(0);
            activeTowers.Add(gameObject);

            gameObject.transform.SetParent(mapParent.transform);
            gameObject.SetActive(true);

            return gameObject;
        }
        else
        {
            Debug.Log($"Inactive Obstacles is empty, generating new tile");
            return GenerateTile(ref towerPrefab, ref activeTowers, true);
        }
    }

    public GameObject GetStartTile()
    {
        start.SetActive(true);
        start.transform.parent = mapParent.transform;
        return start;
    }

    public GameObject GetGoalTile()
    {
        goal.SetActive(true);
        goal.transform.parent = mapParent.transform;
        return goal;
    }

    public GameObject GetObjectByTileType(TileType type)
    {
        switch (type)
        {
            case TileType.Start:
                return GetStartTile();
            case TileType.Path:
                return GetPathTile();
            case TileType.Obstacle:
                return GetObstacleTile();
            case TileType.TowerOne:
            case TileType.TowerTwo:
                return GetTowerTile();
            case TileType.End:
                return GetGoalTile();
        }

        return null;
    }

    public void ResetMap()
    {
        foreach (GameObject tile in activePaths)
        {
            SetTileAsInactive(tile, ref inactivePaths);
        }
        activePaths.Clear();

        foreach (GameObject tile in activeObstacles)
        {
            SetTileAsInactive(tile, ref inactiveObstacles);
        }
        activeObstacles.Clear();

        foreach (GameObject tile in activeTowers)
        {
            SetTileAsInactive(tile, ref inactiveTowers);
        }
        activeTowers.Clear();

        start.SetActive(false);
        start.transform.parent = inactiveTileParent.transform;

        goal.SetActive(false);
        goal.transform.parent = inactiveTileParent.transform;
    }

    private void SetTileAsInactive(GameObject tile, ref List<GameObject> inactiveCollection)
    {
        inactiveCollection.Add(tile);

        tile.SetActive(false);
        tile.transform.parent = inactiveTileParent.transform;
    }
}
