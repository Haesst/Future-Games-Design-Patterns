using System;
using UnityEngine;
using System.Collections.Generic;

namespace AI
{
    public class BreadthFirst : IPathFinder
    {
        private List<Vector2Int> accessibleTiles = new List<Vector2Int>();
        private List<Vector2Int> shortestPath = new List<Vector2Int>();
        private Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        private Dictionary<Vector2Int, bool> visitedPoints = new Dictionary<Vector2Int, bool>();
        private Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();

        private HashSet<Vector2Int> directions = new HashSet<Vector2Int>() { new Vector2Int(0,1), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(-1, 0) };

        private Vector2Int currentPoint = new Vector2Int();

        public BreadthFirst(List<Vector2Int> accessibleTiles)
        {
            this.accessibleTiles.Clear();
            this.accessibleTiles.AddRange(accessibleTiles);
        }

        public IEnumerable<Vector2Int> FindPath(Vector2Int start, Vector2Int goal)
        {
            // Reset the lists so we don't need to allocate memory while creating new ones
            ClearLists();

            // Add the start point.
            frontier.Enqueue(start);
            //frontier.Add(start);
            cameFrom.Add(start, start);

            while (frontier.Count > 0)
            {
                currentPoint = frontier.Dequeue();

                if(currentPoint == goal)
                {
                    break;
                }

                AddNeighboursToFrontier(currentPoint);
            }

            while(currentPoint != start)
            {
                shortestPath.Add(currentPoint);
                currentPoint = cameFrom[currentPoint];
            }

            shortestPath.Add(start);

            shortestPath.Reverse();

            return shortestPath;
        }

        private void ClearLists()
        {
            shortestPath.Clear();
            frontier.Clear();
            visitedPoints.Clear();
            cameFrom.Clear();
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
            //Vector2Int topNeighbour = new Vector2Int(point.x, point.y + 1);
            //Vector2Int rightNeighbour = new Vector2Int(point.x + 1, point.y);
            //Vector2Int bottomNeighbour = new Vector2Int(point.x, point.y - 1);
            //Vector2Int leftNeighbour = new Vector2Int(point.x - 1, point.y);

            //if(!cameFrom.ContainsKey(topNeighbour) && accessibleTiles.Contains(topNeighbour))
            //{
            //    frontier.Add(topNeighbour);
            //    cameFrom.Add(topNeighbour, point);
            //}

            //if (!cameFrom.ContainsKey(rightNeighbour) && accessibleTiles.Contains(rightNeighbour))
            //{
            //    frontier.Add(rightNeighbour);
            //    cameFrom.Add(rightNeighbour, point);
            //}

            //if (!cameFrom.ContainsKey(bottomNeighbour) && accessibleTiles.Contains(bottomNeighbour))
            //{
            //    frontier.Add(bottomNeighbour);
            //    cameFrom.Add(bottomNeighbour, point);
            //}

            //if (!cameFrom.ContainsKey(leftNeighbour) && accessibleTiles.Contains(leftNeighbour))
            //{
            //    frontier.Add(leftNeighbour);
            //    cameFrom.Add(leftNeighbour, point);
            //}
        }
    }
}
