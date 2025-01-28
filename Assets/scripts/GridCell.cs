using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;

public class GridCell : MonoBehaviour
{
    public bool isOccupied = false;
    public float yOffset = 0.5f;
    public List<GridCell> neighbors;

    public int x, z; // Coordinates in the grid

    // A* pathfinding properties
    public int gCost;
    public int hCost;
    public GridCell parent;

    public int fCost => gCost + hCost;

    public bool isseletioncell = false;
    public bool isPlacingCell = false;  // Indicates if this cell is a placing cell
    public bool isReserved = false; // New flag for reservation

    [Header("Assigned Unit")]
    public GameObject placedUnit; // The unit placed on this cell (visible in Inspector)

    // void Awake()
    // {
    //     if (neighbors == null || neighbors.Count == 0)
    //     {
    //         Debug.LogWarning($"{name}: GridCell has no neighbors assigned!");
    //     }
    // }


    public virtual void Start()
    {
        StartCoroutine(InitializeNeighbors());

        // If a unit is manually assigned in the Inspector, initialize the state
        if (placedUnit != null)
        {
            InitializePlacedUnit(Unit.Team.Enemy); // or Unit.Team.Enemy depending on your logic
        }
    }


    private void InitializePlacedUnit(Unit.Team team)
    {
        if (placedUnit != null)  // Ensure this is outside the 'isValid' check
        {
            GameObject instantiatedUnit = !placedUnit.scene.IsValid() ?
                Instantiate(placedUnit, new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z), Quaternion.identity, transform) :
                placedUnit;

            placedUnit = instantiatedUnit;
            placedUnit.transform.position = new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z);
            placedUnit.transform.SetParent(transform);

            Unit unit = placedUnit.GetComponent<Unit>();
            if (unit != null)
            {
                unit.currentCell = this;
                unit.team = team;
                isOccupied = true;

                // Check if the cell is not a selection cell before adding to lists
                if (!isseletioncell)
                {
                    // Add to the GameManager's list based on the team
                    if (team == Unit.Team.Enemy)
                    {
                        GameManager.instance.enemyUnits.Add(unit);
                    }
                    else if (team == Unit.Team.Player)
                    {
                        GameManager.instance.playerUnits.Add(unit);
                    }
                }
            }
        }
    }




    public void MarkOccupied(bool occupied)
    {
        isOccupied = occupied;
        isReserved = occupied; // Reservation should match occupation status
    }

    // Visualize neighbors in the Scene view
    private void OnDrawGizmos()
    {
        Gizmos.color = isOccupied ? Color.red : Color.green;
        Gizmos.DrawSphere(transform.position, 0.2f); // Visualize the cell itself

        Gizmos.color = Color.red;
        if (neighbors != null)
        {
            foreach (var neighbor in neighbors)
            {
                if (neighbor != null)
                {
                    Gizmos.DrawLine(transform.position, neighbor.transform.position); // Draw lines to neighbors
                }
            }
        }
    }


    // Method to place a unit on this cell
    public bool PlaceUnit(GameObject unitPrefab, Unit.Team team)
    {
        if (!isPlacingCell)  // Check if the cell is a valid placing cell
        {
            Debug.LogWarning("This cell is not a valid placing cell.");
            return false;
        }

        Unit unitTemplate = unitPrefab.GetComponent<Unit>();
        if (unitTemplate != null && (team == Unit.Team.Player && GameManager.instance.playergold >= unitTemplate.cost || team == Unit.Team.Enemy))
        {
            if (!isOccupied)
            {
                Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z);
                GameObject newUnit = Instantiate(unitPrefab, spawnPosition, Quaternion.identity, transform);
                Unit unit = newUnit.GetComponent<Unit>();

                if (unit == null)
                {
                    Debug.LogError("Placed unit does not have a Unit component!");
                    Destroy(newUnit);
                    return false;
                }

                unit.team = team;
                unit.currentCell = this;
                placedUnit = newUnit;
                isOccupied = true;

                // Ensure the unit's transform is parented to this cell
                unit.transform.SetParent(transform);

                if (team == Unit.Team.Player)
                {
                    GameManager.instance.playerUnits.Add(unit);
                    GameManager.instance.playergold -= unit.cost;  // Deduct the cost from player's gold
                }
                else if (team == Unit.Team.Enemy)
                {
                    GameManager.instance.enemyUnits.Add(unit);
                    // Deduct resources if needed for enemies
                }

                Debug.Log($"Unit placed. Remaining gold: {GameManager.instance.playergold}");
                return true;
            }
            Debug.LogWarning("Cell is already occupied. Cannot place unit.");
            return false;
        }
        else
        {
            Debug.LogWarning("Not enough gold or invalid team to place unit.");
            return false;
        }
    }




    // Method to remove the placed unit
    public void ClearUnit()
    {
        if (placedUnit != null)
        {
            Destroy(placedUnit); // Destroy the placed unit
            placedUnit = null;   // Clear the reference
        }
        isOccupied = false; // Mark the cell as unoccupied
        isReserved = false; // Clear reservation too
        // if (placedUnit != null)
        // {
        //     Unit unit = placedUnit.GetComponent<Unit>();
        //     if (unit != null)
        //     {
        //         unit.currentCell = null; // Clear the unit's reference to this cell
        //     }
        //     placedUnit = null; // Clear the reference to the unit
        // }
        // isOccupied = false;
        // isReserved = false; // Clear the reservation as well
    }

    protected virtual void OnMouseDown()
    {
        GameManager.instance.TryPlaceUnit(this);
    }



    // public void NotifyNeighbors(Unit movingUnit)
    // {
    //     foreach (GridCell neighbor in neighbors)
    //     {
    //         if (neighbor != null)
    //         {
    //             if (neighbor != null && neighbor.isOccupied && neighbor.placedUnit != null)
    //             {
    //                 Unit neighboringUnit = neighbor.placedUnit.GetComponent<Unit>();
    //                 if (neighboringUnit != null && neighboringUnit.team != movingUnit.team)
    //                 {
    //                     // Trigger combat between the moving unit and the neighboring unit
    //                     StartCombat(movingUnit, neighboringUnit);
    //                 }
    //             }
    //             else
    //             {
    //                 Debug.LogWarning($"Neighbor at ({neighbor.x}, {neighbor.z}) has no placed unit!");
    //             }
    //         }
    //         else
    //         {
    //             Debug.LogWarning("Null neighbor detected in the grid!");
    //         }
    //     }
    // }

    public void NotifyNeighbors(Unit movingUnit)
    {
        foreach (GridCell neighbor in neighbors)
        {
            if (neighbor != null && neighbor.isOccupied && neighbor.placedUnit != null)
            {
                Unit neighboringUnit = neighbor.placedUnit.GetComponent<Unit>();
                if (neighboringUnit != null && neighboringUnit.team != movingUnit.team && neighboringUnit.hp > 0 &&
                !neighboringUnit.currentCell.isseletioncell && !movingUnit.currentCell.isseletioncell)
                {
                    StartCombat(movingUnit, neighboringUnit);
                }
            }
        }
    }


    private void StartCombat(Unit unit1, Unit unit2)
    {
        if (unit1 != null && unit2 != null && unit1.hp > 0 && unit2.hp > 0)
        {
            unit1.StartCombatWith(unit2);
            unit2.StartCombatWith(unit1);
        }
        else
        {
            Debug.LogWarning("One or both units are null or dead, combat skipped.");
        }
    }




    // public void FindNeighbors()
    // {
    //     Vector3[] directions = new Vector3[]
    //     {
    //     new Vector3(1, 0, 0), new Vector3(-1, 0, 0), // right and left
    //     new Vector3(0.5f, 0, Mathf.Sqrt(3)/2), new Vector3(-0.5f, 0, Mathf.Sqrt(3)/2), // top right and top left
    //     new Vector3(0.5f, 0, -Mathf.Sqrt(3)/2), new Vector3(-0.5f, 0, -Mathf.Sqrt(3)/2) // bottom right and bottom left
    //     };

    //     foreach (Vector3 direction in directions)
    //     {
    //         NavMeshHit hit;
    //         if (NavMesh.SamplePosition(transform.position + direction, out hit, 1.0f, NavMesh.AllAreas))
    //         {
    //             GridCell neighbor = null;
    //             Collider[] colliders = Physics.OverlapSphere(hit.position, 0.1f);
    //             foreach (var collider in colliders)
    //             {
    //                 neighbor = collider.GetComponent<GridCell>();
    //                 if (neighbor != null)
    //                 {
    //                     break;
    //                 }
    //             }
    //             if (neighbor != null)
    //             {
    //                 neighbors.Add(neighbor);
    //             }
    //         }
    //     }
    // }
    public void FindNeighbors()
    {
        neighbors = new List<GridCell>();

        Vector3[] directions = new Vector3[]
        {
        new Vector3(1, 0, 0), new Vector3(-1, 0, 0),  // right and left
        new Vector3(0.5f, 0, Mathf.Sqrt(3)/2), new Vector3(-0.5f, 0, Mathf.Sqrt(3)/2), // top-right and top-left
        new Vector3(0.5f, 0, -Mathf.Sqrt(3)/2), new Vector3(-0.5f, 0, -Mathf.Sqrt(3)/2) // bottom-right and bottom-left
        };

        foreach (Vector3 direction in directions)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position + direction, 0.1f);
            foreach (var collider in colliders)
            {
                GridCell neighbor = collider.GetComponent<GridCell>();
                if (neighbor != null)
                {
                    neighbors.Add(neighbor);
                    break;
                }
            }
        }
    }


    IEnumerator InitializeNeighbors()
    {
        // Wait until the next frame to ensure all other cells are instantiated
        yield return new WaitForEndOfFrame();

        // Now call FindNeighbors
        FindNeighbors();
    }


}
