using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 2.0f;
    [SerializeField] private float rotateAngleStep = 5.0f;

    private MapGenerator mapGenerator = default;
    private GameObject playerBase = default;
    private IPathFinder pathFinder = default;

    private List<Vector2Int> path = new List<Vector2Int>();
    private Vector3 nextPoint;
    private bool reachedNextPoint = true;

    MapData mapData = default;

    void Start()
    {
        mapGenerator = GameObject.Find("Map")?.GetComponent<MapGenerator>();
        
        if(mapGenerator)
        {
            mapData = mapGenerator.GetMapData();
            if(mapData != null)
            {
                pathFinder = new BreadthFirst(mapData.accessibles);
            }
        }


        playerBase = GameObject.FindGameObjectWithTag("PlayerBase");
    }

    // Update is called once per frame
    void Update()
    {
        if(mapData != null)
        {
            if(reachedNextPoint)
            {
                Vector2Int goalPosition = mapData.End.GetValueOrDefault();
                Vector2Int startPosition = mapData.WorldToTilePosition(transform.position);

                path.AddRange(pathFinder.FindPath(startPosition, goalPosition));

                if (path.Count > 0)
                {
                    nextPoint = mapData.TileToWorldPosition(path[0]);
                    nextPoint.y = 0.75f;

                    reachedNextPoint = false;
                }
            }

            if(!reachedNextPoint)
            {
                Vector3 newRotation = Vector3.RotateTowards(transform.forward, nextPoint - transform.position, Time.deltaTime * rotateAngleStep, 0);
                transform.rotation = Quaternion.LookRotation(newRotation);
                transform.position = Vector3.MoveTowards(transform.position, nextPoint, walkSpeed * Time.deltaTime);

                if(Vector3.Distance(transform.position, nextPoint) < 0.1f)
                {
                    reachedNextPoint = true;
                    path.Clear(); // This could be changed to remove but for now I want it to re calculate (for a game where that would be needed)
                }
            }
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Color oldColor = Gizmos.color;

    //    Gizmos.color = Color.red;

    //    foreach (var point in path)
    //    {
    //        Gizmos.DrawWireSphere(mapData.TileToWorldPosition(point), 1f);
    //    }

    //    Gizmos.color = oldColor;
    //}
}
