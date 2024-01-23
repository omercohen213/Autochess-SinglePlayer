using UnityEngine;

public class Hex : MonoBehaviour
{
    // Hex properties
    public int Row { get; private set; }
    public int Column { get; private set; }

    // Method to set the row and column of the hex
    public void SetPosition(int row, int column)
    {
        Row = row;
        Column = column;
    }

    public int[] GetPosition()
    {
        return new int[] { Row, Column };
    }
}
