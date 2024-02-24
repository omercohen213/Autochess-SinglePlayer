using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class Hex : MonoBehaviour
{
    // Hex properties
    private int _x;
    private int _y;

    public int X { get => _x; }
    public int Y { get => _y; }

    [SerializeField] private bool _isTaken;
    public bool IsTaken { get => _isTaken; set => _isTaken = value; }
    public GameUnit UnitOnHex;

    public int GCost;
    public float HCost;
    public float FCost { get => GCost + HCost; }
    private Hex _connectedHex;

    private Color hexColor;
    private Color HOVER_ALLOWED = new(60 / 255f, 60 / 255f, 60 / 255f);
    private Color HOVER_NOT_ALLOWED = new(255 / 255f, 40 / 255f, 40 / 255f);

    public Hex ConnectedHex { get => _connectedHex; set => _connectedHex = value; }
    private List<Hex> _adjacentHexes = new();
    public List<Hex> AdjacentHexes { get => _adjacentHexes; set => _adjacentHexes = value; }


    private void Start()
    {
        FindAdjacentHexes();  
        hexColor = gameObject.GetComponent<SpriteRenderer>().color;
    }

    public void Initialize(int x, int y)
    {
        _x = x;
        _y = y;
        name = $"Hex ({x},{y})";
    }

    // Method to find adjacent hexes
    public void FindAdjacentHexes()
    {
        // Upper hex
        if (Board.Instance.GetHex(_x, _y - 1) != null)
        {
            _adjacentHexes.Add(Board.Instance.GetHex(_x, _y - 1));
        }

        // Bottom hex
        if (Board.Instance.GetHex(_x, _y + 1) != null)
        {
            _adjacentHexes.Add(Board.Instance.GetHex(_x, _y + 1));
        }

        if (_x % 2 == 0)
        {
            // Upper right hex
            if (Board.Instance.GetHex(_x + 1, _y - 1) != null)
            {
                _adjacentHexes.Add(Board.Instance.GetHex(_x + 1, _y - 1));
            }
            // Bottom right hex
            if (Board.Instance.GetHex(_x + 1, _y) != null)
            {
                _adjacentHexes.Add(Board.Instance.GetHex(_x + 1, _y));
            }
            // Bottom left hex
            if (Board.Instance.GetHex(_x - 1, _y) != null)
            {
                _adjacentHexes.Add(Board.Instance.GetHex(_x - 1, _y));
            }           
            // Upper left hex
            if (Board.Instance.GetHex(_x - 1, _y - 1) != null)
            {
                _adjacentHexes.Add(Board.Instance.GetHex(_x - 1, _y - 1));
            }
        }

        else
        {
            // Upper right hex
            if (Board.Instance.GetHex(_x + 1, _y) != null)
            {
                _adjacentHexes.Add(Board.Instance.GetHex(_x + 1, _y));
            }

            // Bottom right hex
            if (Board.Instance.GetHex(_x + 1, _y+1) != null)
            {
                _adjacentHexes.Add(Board.Instance.GetHex(_x + 1, _y + 1));
            }

            // Bottom left hex
            if (Board.Instance.GetHex(_x - 1, _y+1) != null)
            {
                _adjacentHexes.Add(Board.Instance.GetHex(_x - 1, _y+1));
            }
            // Upper left hex
            if (Board.Instance.GetHex(_x - 1, _y) != null)
            {
                _adjacentHexes.Add(Board.Instance.GetHex(_x - 1, _y));
            }
        }
    }

    public bool IsAdjacentToHex(Hex hex)
    {
        return AdjacentHexes.Contains(hex);
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

    // Method to convert Hex to Vector2Int
    public Vector2Int ToVector2Int()
    {
        return new Vector2Int(X, Y);
    }

    // Change color when hovered
    public void OnHover()
    {
        if (transform.Find("Background").gameObject.TryGetComponent(out SpriteRenderer spriteRenderer))
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
        if (transform.Find("Background").gameObject.TryGetComponent(out SpriteRenderer spriteRenderer))
        {
            spriteRenderer.color = hexColor;
        }
    }
}
