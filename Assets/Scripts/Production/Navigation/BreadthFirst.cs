using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BreadthFirst : AI.IPathFinder
{
    private List<Vector2Int> accessibleTiles = new List<Vector2Int>();
    private List<Vector2Int> shortestPath = new List<Vector2Int>();
    private Queue<Vector2Int> frontier = new Queue<Vector2Int>();
    private Dictionary<Vector2Int, bool> visitedPoints = new Dictionary<Vector2Int, bool>();
    private Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();

    private HashSet<Vector2Int> directions = new HashSet<Vector2Int>() { new Vector2Int(0,1), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(-1, 0) };

    private Vector2Int currentPoint = new Vector2Int();
    private Vector2Int start;
    private Vector2Int goal;

    public BreadthFirst(List<Vector2Int> accessibleTiles)
    {
        this.accessibleTiles.Clear();
        this.accessibleTiles.AddRange(accessibleTiles);
    }

    public IEnumerable<Vector2Int> FindPath(Vector2Int start, Vector2Int goal)
    {
        this.start = start;
        this.goal = goal;

        // Reset the lists so we don't need to allocate memory while creating new ones
        ClearLists();

        FindWayToGoal();

        if(cameFrom.ContainsKey(goal))
        {
            SetShortestPath();
            return shortestPath;
        }

        return Enumerable.Empty<Vector2Int>();
    }

    private void ClearLists()
    {
        shortestPath.Clear();
        frontier.Clear();
        visitedPoints.Clear();
        cameFrom.Clear();
    }

    private void FindWayToGoal()
    {
        // Add the start point.
        frontier.Enqueue(start);
        //frontier.Add(start);
        cameFrom.Add(start, start);

        // Loop through points until goal is found or there's no more
        // neighbours (In other words no valid path could be found)
        while (frontier.Count > 0)
        {
            currentPoint = frontier.Dequeue();

            // Break out early if we find the goal
            if (currentPoint == goal)
            {
                break;
            }

            AddNeighboursToFrontier(currentPoint);
        }
    }

    private void AddNeighboursToFrontier(Vector2Int point)
    {
        foreach (Vector2Int direction in directions)
        {
            if(!cameFrom.ContainsKey(point + direction) && accessibleTiles.Contains(point + direction))
            {
                Vector2Int newPoint = new Vector2Int(point.x + direction.x, point.y + direction.y);

                frontier.Enqueue(newPoint);
                cameFrom.Add(newPoint, point);
            }
        }
    }

    private void SetShortestPath()
    {
        while (currentPoint != start)
        {
            shortestPath.Add(currentPoint);
            currentPoint = cameFrom[currentPoint];
        }

        shortestPath.Add(start);
        shortestPath.Reverse();
    }
}