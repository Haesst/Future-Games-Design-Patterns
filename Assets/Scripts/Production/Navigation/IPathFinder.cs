using System.Collections.Generic;
using UnityEngine;

public interface IPathFinder
{
    IEnumerable<Vector2Int> FindPath(Vector2Int start, Vector2Int goal);
}
public struct PathNode
{
	public int x;
	public int y;

	public int index;

	public int gCost;
	public int hCost;
	public int fCost;

	public bool isWalkable;

	public int cameFromNodeIndex;

	public void CalculateFCost()
	{
		fCost = gCost + hCost;
	}
}