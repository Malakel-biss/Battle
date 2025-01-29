using UnityEngine;

public class HexGrid : MonoBehaviour
{
    public int width = 6;
    public int height = 6;
    public GameObject hexPrefab;

    private float hexWidth;   // Full width of the hex
    private float hexHeight;  // Vertical distance from one hex to the next
    private Vector3 startPos;

    void Start()
    {
        // Assuming your hex prefab is scaled to Unity units properly
        hexWidth = hexPrefab.GetComponent<Renderer>().bounds.size.x;
        hexHeight = Mathf.Sqrt(3) * hexWidth / 2;  // Calculating height based on width
        startPos = transform.position;

        CreateGrid();
    }

    void CreateGrid()
    {
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                // Calculating the position for each hexagon
                float xPosition = x * hexWidth * 0.75f;
                float zPosition = z * hexHeight;

                // Staggering the grid: every second column is moved down by half the height
                if (x % 2 != 0)
                {
                    zPosition += hexHeight / 2;
                }

                Vector3 position = new Vector3(xPosition, 0, zPosition) + startPos;
                Instantiate(hexPrefab, position, Quaternion.identity, this.transform);
            }
        }
    }
}
