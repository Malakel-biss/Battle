using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    public static List<GridCell> FindPath(GridCell startCell, GridCell targetCell)
    {
        if (startCell == null || targetCell == null)
        {
            Debug.LogError("Start cell or target cell is null!");
            return null;
        }

        // Reset pathfinding data for all cells
        GridCell[] allCells = FindObjectsOfType<GridCell>();
        ResetPathfindingData(allCells);

        List<GridCell> openSet = new List<GridCell>();
        HashSet<GridCell> closedSet = new HashSet<GridCell>();
        openSet.Add(startCell);

        startCell.gCost = 0;
        startCell.hCost = GetDistance(startCell, targetCell);

        while (openSet.Count > 0)
        {
            GridCell currentCell = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentCell.fCost || (openSet[i].fCost == currentCell.fCost && openSet[i].hCost < currentCell.hCost))
                {
                    currentCell = openSet[i];
                }
            }

            openSet.Remove(currentCell);
            closedSet.Add(currentCell);

            if (currentCell == targetCell)
            {
                return RetracePath(startCell, targetCell);
            }

            foreach (GridCell neighbor in currentCell.neighbors)
            {
                if (neighbor == null || closedSet.Contains(neighbor) || neighbor.isOccupied || neighbor.isReserved)
                    continue;

                int newCostToNeighbor = currentCell.gCost + GetDistance(currentCell, neighbor);
                if (newCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetCell);
                    neighbor.parent = currentCell;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return null; // No path found
    }

    private static List<GridCell> RetracePath(GridCell startCell, GridCell endCell)
    {
        List<GridCell> path = new List<GridCell>();
        GridCell currentCell = endCell;

        while (currentCell != startCell)
        {
            path.Add(currentCell);
            currentCell = currentCell.parent;
        }
        path.Reverse();
        return path;
    }

    private static int GetDistance(GridCell cellA, GridCell cellB)
    {
        int dx = Mathf.Abs(cellA.x - cellB.x);
        int dz = Mathf.Abs(cellA.z - cellB.z);
        return dx + dz; // Simple hexagonal distance
    }

    private static void ResetPathfindingData(GridCell[] allCells)
    {
        foreach (GridCell cell in allCells)
        {
            cell.gCost = int.MaxValue;
            cell.hCost = int.MaxValue;
            cell.parent = null;
        }
    }
}
