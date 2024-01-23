using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoard : MonoBehaviour
{
/*    public UnitsBench playerBench;

    // Assuming your hexes are organized in a 3x9 grid
    public int rows = 3;
    public int columns = 9;

    private void Start()
    {
        playerBench = Player.Instance.UnitsBench;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.TryGetComponent<Hex>(out var hex))
                {
                    Unit unit = GetSelectedUnit();
                    if (unit != null && playerBench.IsUnitOnBench(unit))
                    {
                        MoveUnitToHex(unit, hex);
                    }
                }
            }
        }
    }

    private Unit GetSelectedUnit()
    {
        // Implement logic to get the selected unit (e.g., raycast to units on the bench)
        // This can be specific to your game mechanics
        // For now, returning null as a placeholder
        return null;
    }

    private void MoveUnitToHex(Unit unit, Hex hex)
    {
        // Extract the row and column from the hex's name
        string[] hexNameParts = hex.gameObject.name.Split('_');
        if (hexNameParts.Length == 3)
        {
            int row = int.Parse(hexNameParts[1]);
            int column = int.Parse(hexNameParts[2]);

            // Implement logic to move the unit to the specified hex
            // This could involve instantiating the unit on the board at the hex position

            // For now, let's assume you have a method in your board script to handle this
            Board.Instance.AddUnitToHex(unit, row, column);

            // Remove the unit from the bench
            playerBench.RemoveUnit(unit);
        }
        else
        {
            Debug.LogWarning("Invalid hex name format.");
        }
    }*/
}
