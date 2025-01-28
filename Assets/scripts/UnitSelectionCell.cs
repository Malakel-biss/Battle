using UnityEngine;

public class UnitSelectionCell : GridCell
{

    void Awake()
    {
        isseletioncell = true;// Mark the cell as occupied
    }
    void Start()
    {
        base.Start();  // Call the base class Start to initialize neighbors
        InitializeImovableUnit(Unit.Team.Player);  // Initialize the permanent unit on this cell
    }

    private void InitializeImovableUnit(Unit.Team team)
    {
        if (placedUnit.scene.IsValid()) // Check if the placedUnit is already in the scene
        {
            // Ensure the unit is positioned correctly and linked to the cell
            placedUnit.transform.position = new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z);
            placedUnit.transform.parent = transform;

            // Set the unit's GridCell reference
            Unit unit = placedUnit.GetComponent<Unit>();
            if (unit != null)
            {
                unit.currentCell = this;
                unit.team = team; // Set the unit's team
                // unit.isMoving = true;  // Make sure the unit is not movable
            }

            isOccupied = true; // Mark the cell as occupied
        }
        else
        {
            Debug.LogWarning($"Placed unit on cell ({x}, {z}) is a prefab. Instantiating it...");
            // Instantiate the prefab and assign it to the placedUnit
            GameObject instantiatedUnit = Instantiate(placedUnit, new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z), Quaternion.identity, transform);
            placedUnit = instantiatedUnit;

            // Set the unit's GridCell reference
            Unit unit = placedUnit.GetComponent<Unit>();
            if (unit != null)
            {
                unit.currentCell = this;
                unit.team = team; // Set the unit's team
                // unit.isMoving = true;  // Make sure the unit is not movable
            }

            isOccupied = true; // Mark the cell as occupied
        }
    }

    protected override void OnMouseDown()
    {
        Debug.Log("Mouse down on UnitSelectionCell");
        // if (placedUnit != null)
        // {
        //     GameManager.instance.SetPlayerUnitPrefab(placedUnit);
        //     Debug.Log($"Player unit prefab {placedUnit.name} set as current player unit.");
        // }
        // else
        // {
        //     Debug.LogWarning("Placed unit is null in UnitSelectionCell");
        // }
    }

}
