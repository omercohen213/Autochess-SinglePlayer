using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BenchSlot : MonoBehaviour
{
    public int Column { get; private set; }
    public int Row { get; private set; }

    public bool _isTaken;
    public bool IsTaken { get => _isTaken; set => _isTaken = value; }

    private void Start()
    {
        _isTaken = false;
    }

    // Method to set the row and column of the hex
    public void SetPosition(int row, int column)
    {
        Column = column;
        Row = row;
    }

    public int[] GetPosition()
    {
        return new int[] { Column, Row };
    }

    public override string ToString()
    {
        string str = "Column: " + Column + "Row: " + Row;
        return str;
    }
}
