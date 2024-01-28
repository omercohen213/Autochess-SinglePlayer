using UnityEngine;

public class Hex : MonoBehaviour
{
    // Hex properties
    public int X { get; private set; }
    public int Y { get; private set; }

    // Method to set the row and column of the hex
    public void SetPosition(int row, int column)
    {
        X  = row;
        Y = column;
    }

    public int[] GetPosition()
    {
        return new int[] { X, Y };
    }

    public override string ToString()
    {
        string str = "Hex: ("+ X+ ", "+ Y+")";
        return str;
    }
}
