using UnityEngine;

public class AssignGridCoordinates : MonoBehaviour
{
    public float cellSize = 1f; // Size of one hex cell (distance between centers)

    void Start()
    {
        // Find all grid cells in the scene
        GridCell[] allCells = FindObjectsOfType<GridCell>();

        Debug.Log($"Found {allCells.Length} grid cells in the scene.");

        if (allCells.Length == 0)
        {
            Debug.LogWarning("No GridCell objects found in the scene.");
            return;
        }

        // Assign coordinates based on world position
        foreach (GridCell cell in allCells)
        {
            Vector2Int coords = CalculateGridCoordinates(cell.transform.position);
            cell.x = coords.x;
            cell.z = coords.y;

            Debug.Log($"{cell.name} assigned coordinates ({cell.x}, {cell.z}), World position: {cell.transform.position}");
        }
    }

    private Vector2Int CalculateGridCoordinates(Vector3 position)
    {
        // Round the world position to nearest integer grid coordinates
        int x = Mathf.RoundToInt(position.x / cellSize);
        int z = Mathf.RoundToInt(position.z / cellSize);

        return new Vector2Int(x, z);
    }
}
