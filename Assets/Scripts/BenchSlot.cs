using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BenchSlot : MonoBehaviour
{
    public int Column { get; private set; }
    public int Row { get; private set; }

    [SerializeField] private bool _isTaken;
    public bool IsTaken { get => _isTaken; set => _isTaken = value; }

    [SerializeField] private GameUnit _unitOnSlot;
    public GameUnit UnitOnSlot { get => _unitOnSlot; set => _unitOnSlot = value; }

    private Color slotColor;
    private Color HOVER_COLOR = new(70 / 255f, 70 / 255f, 70 / 255f);

    private void Start()
    {
        _isTaken = false;
        if (transform.Find("Background").gameObject.TryGetComponent(out SpriteRenderer spriteRenderer))
        {
            slotColor = spriteRenderer.color;
        }
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

    // Change color when hovered
    public void OnHover()
    {
        if (transform.Find("Background").gameObject.TryGetComponent(out SpriteRenderer spriteRenderer))
        {
            spriteRenderer.color = HOVER_COLOR;
        }
    }

    // Revert color back to normal
    public void OnHoverStopped()
    {
        if (transform.Find("Background").gameObject.TryGetComponent(out SpriteRenderer spriteRenderer))
        {
            spriteRenderer.color = slotColor;
        }
    }
}
