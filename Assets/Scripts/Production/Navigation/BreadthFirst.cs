using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
public class BreadthFirst : IPathFinder
{
    private HashSet<Vector2Int> m_AccessibleTiles = new HashSet<Vector2Int>();
    private List<Vector2Int> m_ShortestPath = new List<Vector2Int>();
    private Queue<Vector2Int> m_TilesToEvaluate = new Queue<Vector2Int>();
    private Dictionary<Vector2Int, bool> m_VisitedPoints = new Dictionary<Vector2Int, bool>();
    private Dictionary<Vector2Int, Vector2Int> m_CameFrom = new Dictionary<Vector2Int, Vector2Int>();

    private HashSet<Vector2Int> m_Directions = new HashSet<Vector2Int>() { new Vector2Int(0,1), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(-1, 0) };

    private Vector2Int m_CurrentPoint = new Vector2Int();
    private Vector2Int m_Start;
    private Vector2Int m_Goal;

    [SerializeField] private bool m_UseMultiThreadedSearch = false;

    public BreadthFirst(List<Vector2Int> accessibleTiles)
    {
        m_AccessibleTiles.UnionWith(accessibleTiles);
    }

    public IEnumerable<Vector2Int> FindPath(Vector2Int start, Vector2Int goal)
    {
        m_Start = start;
        m_Goal = goal;

        // Reset the lists so we don't need to allocate memory while creating new ones
        ClearLists();

        if (m_UseMultiThreadedSearch)
        {
            JobHandle findWayHandle = FindWayHandle();
            findWayHandle.Complete();
        }
        else
        {
            FindWayToGoal();
        }

        if (m_CameFrom.ContainsKey(goal))
        {
            SetShortestPath();
            return m_ShortestPath;
        }

        return Enumerable.Empty<Vector2Int>();
    }

    private JobHandle FindWayHandle()
    {
        // set values
        FindWayToGoalStruct m_Job = new FindWayToGoalStruct();

        NativeArray<int2> accessibleFloats = new NativeArray<int2>(m_AccessibleTiles.Count, Allocator.TempJob);

        for (int i = 0; i < m_AccessibleTiles.Count; i++)
        {
            Vector2Int point = m_AccessibleTiles.ElementAt(i);
            accessibleFloats[i] = new int2(point.x, point.y);
        }

        m_Job.accessibleTiles = accessibleFloats;
        accessibleFloats.Dispose();

        return m_Job.Schedule();
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

    //[BurstCompile] <- cant get this to work
    private struct FindWayToGoalStruct : IJob
    {
        public NativeArray<int2> accessibleTiles;

        // needs: 
        //private List<Vector2Int> m_ShortestPath = new List<Vector2Int>();
        //private Queue<Vector2Int> m_TilesToEvaluate = new Queue<Vector2Int>();
        //private Dictionary<Vector2Int, bool> m_VisitedPoints = new Dictionary<Vector2Int, bool>();
        //private Dictionary<Vector2Int, Vector2Int> m_CameFrom = new Dictionary<Vector2Int, Vector2Int>();

        //private HashSet<Vector2Int> m_Directions = new HashSet<Vector2Int>() { new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(-1, 0) };

        //private Vector2Int m_CurrentPoint = new Vector2Int();
        //private Vector2Int m_Start;
        //private Vector2Int m_Goal;
        public void Execute()
        {
        }
    }

    private void AddNeighboursToFrontier(Vector2Int point)
    {
        foreach (Vector2Int direction in m_Directions)
        {
            
            if (!m_CameFrom.TryGetValue(point + direction, out Vector2Int value) && m_AccessibleTiles.Contains(point + direction))
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