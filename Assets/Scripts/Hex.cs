using Unity.VisualScripting;
using UnityEngine;

public class Hex : MonoBehaviour
{
    // Hex properties
    public int X { get; private set; }
    public int Y { get; private set; }

    private bool _isTaken;
    public bool IsTaken { get => _isTaken; set => _isTaken = value; }

    private Color hexColor;
    private Color HOVER_ALLOWED = Color.gray;
    private Color HOVER_NOT_ALLOWED = new(200 / 255f, 70 / 255f, 70 / 255f);

    private void Start()
    {
        hexColor = gameObject.GetComponent<SpriteRenderer>().color;
    }

    // Method to set the row and column of the hex
    public void SetPosition(int row, int column)
    {
        X = row;
        Y = column;
    }

    public int[] GetPosition()
    {
        return new int[] { X, Y };
    }

    public override string ToString()
    {
        string str = "Hex: (" + X + ", " + Y + ")";
        return str;
    }

    // Change color when hovered
    public void OnHover()
    {
        if (gameObject.TryGetComponent(out SpriteRenderer spriteRenderer))
        {
            if (X < 4)
            {
                spriteRenderer.color = HOVER_ALLOWED;
            }
            else
            {
                spriteRenderer.color = HOVER_NOT_ALLOWED;
            }
        }
    }

    // Revert color back to normal
    public void OnHoverStopped()
    {
        if (gameObject.TryGetComponent(out SpriteRenderer spriteRenderer))
        {
            spriteRenderer.color = hexColor;
        }
    }
}
