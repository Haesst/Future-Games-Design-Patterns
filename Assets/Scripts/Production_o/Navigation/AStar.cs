//using System.Linq;
//using System.Collections.Generic;
//using UnityEngine;
//using Unity.Mathematics;
//using Unity.Collections;
//using Unity.Jobs;
//using Unity.Burst;

//public class AStar : IPathFinder
//{
//	private const int MOVE_STRAIGHT_COST = 10;
//	private const int MOVE_DIAGONAL_COST = 14;

//	private bool m_CanMoveDiagonal = false;
//	private TileType[,] m_TileGrid;
	

//	public AStar(bool canMoveDiagonal, TileType[,] tileGrid)
//	{
//		m_CanMoveDiagonal = canMoveDiagonal;
//		m_TileGrid = tileGrid;
//	}

//	public IEnumerable<Vector2Int> FindPath(Vector2Int start, Vector2Int goal)
//	{
//		int2 startPosition = new int2(start.x, start.y);
//		int2 endPosition = new int2(goal.x, goal.y);

//		int2 gridSize = new int2(m_TileGrid.GetLength(0), m_TileGrid.GetLength(1));

//		NativeList<int2> accessibleTiles = new NativeList<int2>(Allocator.Temp);
//		NativeList<int2> nativePath = new NativeList<int2>(Allocator.TempJob);

//		for(int x = 0; x < m_TileGrid.GetLength(0); x++)
//		{
//			for(int y = 0; y < m_TileGrid.GetLength(1); y++)
//			{
//				if(TileMethods.IsWalkable(m_TileGrid[x,y]))
//				{
//					accessibleTiles.Add(new int2(x, y));
//				}
//			}
//		}

//		FindPathJob findPathJob = new FindPathJob
//		{
//			m_GridSize = gridSize,
//			m_StartPosition = startPosition,
//			m_EndPosition = endPosition,
//			m_AccessibleTiles = accessibleTiles,
//			m_CanMoveDiagonal = m_CanMoveDiagonal,
//			path = nativePath
//		};

//		JobHandle job = findPathJob.Schedule();

//		job.Complete();

//		List<Vector2Int> path = new List<Vector2Int>();

//		for (int i = 0; i < findPathJob.path.Length; i++)
//		{
//			path.Add(new Vector2Int(findPathJob.path[i].x, findPathJob.path[i].y));
//		}

//		path.Add(new Vector2Int(findPathJob.nextX, findPathJob.nextY));

//		nativePath.Dispose();
//		accessibleTiles.Dispose();

//		return path;
//	}

//	// [BurstCompile] <- need to import this package
//	private struct FindPathJob : IJob
//	{
//		public int nextX;
//		public int nextY;

//		public int2 m_GridSize;
//		public int2 m_StartPosition;
//		public int2 m_EndPosition;

//		public NativeArray<int2> m_AccessibleTiles;
//		public NativeList<int2> path;

//		public bool m_CanMoveDiagonal;

//		public void Execute()
//		{
//			NativeArray<PathNode> pathNodeArray = new NativeArray<PathNode>(m_GridSize.x * m_GridSize.y, Allocator.Temp);

//			for (int x = 0; x < m_GridSize.x; x++)
//			{
//				for (int y = 0; y < m_GridSize.y; y++)
//				{
//					PathNode pathNode = new PathNode();
//					pathNode.x = x;
//					pathNode.y = y;

//					pathNode.index = CalculateIndex(x, y, m_GridSize.x);

//					pathNode.gCost = int.MaxValue;
//					pathNode.hCost = CalculateDistanceCost(new int2(x, y), m_EndPosition);
//					pathNode.CalculateFCost();

//					pathNode.isWalkable = m_AccessibleTiles.Contains(new int2(x, y));
//					pathNode.cameFromNodeIndex = -1;

//					pathNodeArray[pathNode.index] = pathNode;
//				}
//			}

//			NativeArray<int2> neighbourOffsetArray = new NativeArray<int2>(8, Allocator.Temp);
//			neighbourOffsetArray[0] = new int2(0, 1); // Up
//			neighbourOffsetArray[1] = new int2(1,0); // Right
//			neighbourOffsetArray[2] = new int2(0,-1); // Down
//			neighbourOffsetArray[3] = new int2(-1, 0); // Left
//			neighbourOffsetArray[4] = new int2(1,1); // Right Up
//			neighbourOffsetArray[5] = new int2(1,-1); // Right Down
//			neighbourOffsetArray[6] = new int2(-1,-0); // Left Down
//			neighbourOffsetArray[7] = new int2(-1,1); // Left Up

//			int endNodeIndex = CalculateIndex(m_EndPosition.x, m_EndPosition.y, m_GridSize.x);

//			PathNode startNode = pathNodeArray[CalculateIndex(m_StartPosition.x, m_StartPosition.y, m_GridSize.x)];
//			startNode.gCost = 0;
//			startNode.CalculateFCost();

//			// Since this is a value type we're not modifying the actual object
//			// we're modifying a copy. Because of that we need to set the node
//			// in the array to the updated value.
//			pathNodeArray[startNode.index] = startNode;

//			NativeList<int> openList = new NativeList<int>(Allocator.Temp);
//			NativeList<int> closedList = new NativeList<int>(Allocator.Temp);

//			openList.Add(startNode.index);

//			while (openList.Length > 0)
//			{
//				int currentNodeIndex = GetLowestFCostNodeIndex(openList, pathNodeArray);
//				PathNode currentNode = pathNodeArray[currentNodeIndex];

//				if (currentNodeIndex == endNodeIndex)
//				{
//					// Reached goal
//					break;
//				}

//				// Remove current node from open list
//				for (int i = 0; i < openList.Length; i++)
//				{
//					if (openList[i] == currentNodeIndex)
//					{
//						openList.RemoveAtSwapBack(i);
//						break;
//					}
//				}

//				closedList.Add(currentNodeIndex);

//				for (int i = 0; i < neighbourOffsetArray.Length; i++)
//				{
//					int2 neighbourOffset = neighbourOffsetArray[i];
//					int2 neighbourPosition = new int2(currentNode.x + neighbourOffset.x, currentNode.y + neighbourOffset.y);

//					if (!IsPositionInsideGrid(neighbourPosition, m_GridSize))
//					{
//						// Neighbour is not a valid position
//						continue;
//					}

//					int neighbourNodeIndex = CalculateIndex(neighbourPosition.x, neighbourPosition.y, m_GridSize.x);

//					if (closedList.Contains(neighbourNodeIndex))
//					{
//						// We have already closed this node
//						continue;
//					}

//					PathNode neighbourNode = pathNodeArray[neighbourNodeIndex];
//					if (!neighbourNode.isWalkable)
//					{
//						// Not walkable
//						continue;
//					}

//					int2 currentNodePosition = new int2(currentNode.x, currentNode.y);

//					int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNodePosition, neighbourPosition);
//					if (tentativeGCost < neighbourNode.gCost)
//					{
//						neighbourNode.cameFromNodeIndex = currentNodeIndex;
//						neighbourNode.gCost = tentativeGCost;
//						neighbourNode.CalculateFCost();
//						pathNodeArray[neighbourNodeIndex] = neighbourNode;

//						if (!openList.Contains(neighbourNodeIndex))
//						{
//							openList.Add(neighbourNodeIndex);
//						}
//					}
//				}
//			}

//			PathNode endNode = pathNodeArray[endNodeIndex];

//			if (endNode.cameFromNodeIndex == -1)
//			{
//				// Didn't find a path
//				Debug.Log("No path found");
//				path = new NativeList<int2>();
//			}
//			else
//			{
//				// Found a path
//				Debug.Log("Found a path");
//				path = CalculatePath(pathNodeArray, endNode);
//				nextX = path[0].x;
//				nextY = path[0].y;
//			}

//			pathNodeArray.Dispose();
//			openList.Dispose();
//			closedList.Dispose();
//			neighbourOffsetArray.Dispose();
//		}
//		private NativeList<int2> CalculatePath(NativeArray<PathNode> pathNodeArray, PathNode endNode)
//		{
//			if (endNode.cameFromNodeIndex == -1)
//			{
//				return new NativeList<int2>(Allocator.Temp);
//			}
//			else
//			{
//				NativeList<int2> path = new NativeList<int2>(Allocator.Temp);
//				path.Add(new int2(endNode.x, endNode.y));

//				PathNode currentNode = endNode;
//				while (currentNode.cameFromNodeIndex != -1)
//				{
//					PathNode cameFromNode = pathNodeArray[currentNode.cameFromNodeIndex];
//					path.Add(new int2(cameFromNode.x, cameFromNode.y));
//					currentNode = cameFromNode;
//				}

//				path.Reverse();

//				return path;
//			}
//		}

//		private bool IsPositionInsideGrid(int2 gridPosition, int2 gridSize)
//		{
//			return
//				gridPosition.x >= 0 &&
//				gridPosition.y >= 0 &&
//				gridPosition.x < gridSize.x &&
//				gridPosition.y < gridSize.y;
//		}

//		private int CalculateIndex(int x, int y, int gridWidth)
//		{
//			return x + y * gridWidth;
//		}

//		private int CalculateDistanceCost(int2 aPosition, int2 bPosition)
//		{
//			int xDistance = Mathf.Abs(aPosition.x - bPosition.x);
//			int yDistance = Mathf.Abs(aPosition.y - bPosition.y);
//			int remaining = Mathf.Abs(xDistance - yDistance);

//			if (m_CanMoveDiagonal)
//			{
//				return MOVE_DIAGONAL_COST * math.min(xDistance, yDistance) + MOVE_DIAGONAL_COST * remaining;
//			}
//			else
//			{
//				return xDistance + yDistance;
//			}
//		}

//		private int GetLowestFCostNodeIndex(NativeList<int> openList, NativeArray<PathNode> pathNodeArray)
//		{
//			PathNode lowestCostPathNode = pathNodeArray[openList[0]];
//			for (int i = 1; i < openList.Length; i++)
//			{
//				PathNode testPathNode = pathNodeArray[openList[i]];
//				if (testPathNode.fCost < lowestCostPathNode.fCost)
//				{
//					lowestCostPathNode = testPathNode;
//				}
//			}

//			return lowestCostPathNode.index;
//		}
//	}
//}
