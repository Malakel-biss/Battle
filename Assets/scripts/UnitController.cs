using UnityEngine;

public class UnitController : MonoBehaviour
{
    private Unit selectedUnit; // Currently selected unit

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left click to select a unit
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Check if the clicked object is a unit
                Unit clickedUnit = hit.collider.GetComponent<Unit>();
                if (clickedUnit != null)
                {
                    if (!clickedUnit.currentCell.isseletioncell)
                    {
                        selectedUnit = clickedUnit;
                        Debug.Log($"Unit {selectedUnit.currentCell.isseletioncell} selected!");
                        Debug.Log($"Unit {selectedUnit.name} selected!");
                    }
                    else
                    {
                        GameManager.instance.SetPlayerUnitPrefab(clickedUnit.gameObject);  // Set this unit as the current player unit prefab
                        Debug.Log($"Player unit prefab {clickedUnit} set!");
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1)) // Right click to move the selected unit
        {
            if (selectedUnit == null)
            {
                Debug.LogWarning("No unit selected!");
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GridCell clickedCell = hit.collider.GetComponent<GridCell>();
                if (clickedCell != null)
                {
                    Debug.Log($"Moving unit to cell ({clickedCell.x}, {clickedCell.z})");
                    selectedUnit.MoveTo(clickedCell);
                }
                else
                {
                    Debug.LogWarning("Clicked object is not a valid grid cell!");
                }
            }
        }
    }
}
