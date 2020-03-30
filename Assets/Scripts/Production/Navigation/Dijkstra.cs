using System;
using UnityEngine;
using System.Collections.Generic;

namespace AI
{
	//TODO: Implement IPathFinder using Dijsktra algorithm.
	public class Dijkstra : IPathFinder
	{
		private List<Vector2Int> path;
		private List<Vector2Int> finishedPoints;
		public IEnumerable<Vector2Int> FindPath(Vector2Int start, Vector2Int goal)
		{
			throw new NotImplementedException();
		}
	}    
}
