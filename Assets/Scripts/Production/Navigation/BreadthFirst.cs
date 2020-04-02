using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BreadthFirst : IPathFinder
{
    private List<Vector2Int> m_AccessibleTiles = new List<Vector2Int>();
    private List<Vector2Int> m_ShortestPath = new List<Vector2Int>();
    private Queue<Vector2Int> m_TilesToEvaluate = new Queue<Vector2Int>();
    private Dictionary<Vector2Int, bool> m_VisitedPoints = new Dictionary<Vector2Int, bool>();
    private Dictionary<Vector2Int, Vector2Int> m_CameFrom = new Dictionary<Vector2Int, Vector2Int>();

    private HashSet<Vector2Int> m_Directions = new HashSet<Vector2Int>() { new Vector2Int(0,1), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(-1, 0) };

    private Vector2Int m_CurrentPoint = new Vector2Int();
    private Vector2Int m_Start;
    private Vector2Int m_Goal;

    public BreadthFirst(List<Vector2Int> accessibleTiles)
    {
        this.m_AccessibleTiles.Clear();
        this.m_AccessibleTiles.AddRange(accessibleTiles);
    }

    public IEnumerable<Vector2Int> FindPath(Vector2Int start, Vector2Int goal)
    {
        m_Start = start;
        m_Goal = goal;

        // Reset the lists so we don't need to allocate memory while creating new ones
        ClearLists();

        FindWayToGoal();

        if(m_CameFrom.ContainsKey(goal))
        {
            SetShortestPath();
            return m_ShortestPath;
        }

        return Enumerable.Empty<Vector2Int>();
    }

    private void ClearLists()
    {
        m_ShortestPath.Clear();
        m_TilesToEvaluate.Clear();
        m_VisitedPoints.Clear();
        m_CameFrom.Clear();
    }

    private void FindWayToGoal()
    {
        // Add the start point.
        m_TilesToEvaluate.Enqueue(m_Start);

        m_CameFrom.Add(m_Start, m_Start);

        // Loop through points until goal is found or there's no more
        // neighbours (In other words no valid path could be found)
        while (m_TilesToEvaluate.Count > 0)
        {
            m_CurrentPoint = m_TilesToEvaluate.Dequeue();

            // Break out early if we find the goal
            if (m_CurrentPoint == m_Goal)
            {
                break;
            }

            AddNeighboursToFrontier(m_CurrentPoint);
        }
    }

    private void AddNeighboursToFrontier(Vector2Int point)
    {
        foreach (Vector2Int direction in m_Directions)
        {
            if (!m_CameFrom.ContainsKey(point + direction) && m_AccessibleTiles.Contains(point + direction))
            {
                Vector2Int newPoint = new Vector2Int(point.x + direction.x, point.y + direction.y);

                m_TilesToEvaluate.Enqueue(newPoint);
                m_CameFrom.Add(newPoint, point);
            }
        }
    }

    private void SetShortestPath()
    {
        while (m_CurrentPoint != m_Start)
        {
            m_ShortestPath.Add(m_CurrentPoint);
            m_CurrentPoint = m_CameFrom[m_CurrentPoint];
        }

        m_ShortestPath.Reverse();
    }
}