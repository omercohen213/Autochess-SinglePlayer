using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent (typeof(PolygonCollider2D))]
[RequireComponent (typeof(CircleCollider2D))]
public class Hex : MonoBehaviour
{
    // Hex properties
    public int X { get; private set; }
    public int Y { get; private set; }

    //private bool _isTaken;
    public bool IsTaken;// { get => _isTaken; set => _isTaken = value; }
    public GameUnit UnitOnHex; 

    private Color hexColor;
    private Color HOVER_ALLOWED = Color.gray;
    private Color HOVER_NOT_ALLOWED = new(200 / 255f, 70 / 255f, 70 / 255f);

    private CircleCollider2D circleCollider;
    public HashSet<Hex> AdjacentHexes = new HashSet<Hex>();

    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
    }

    private void Start()
    {
        hexColor = gameObject.GetComponent<SpriteRenderer>().color;
        FindAdjacentHexes();
    }

    public void Initialize(int x, int y)
    {
        X = x;
        Y = y;
        name = $"Hex ({x},{y})";
    }

    // Method to find adjacent hexes
    public void FindAdjacentHexes()
    {
        // Clear the list of adjacent hexes
        AdjacentHexes.Clear();

        // Find all colliders overlapping with the circle collider
        Collider2D[] colliders = Physics2D.OverlapCircleAll(circleCollider.bounds.center, circleCollider.radius);

        foreach (Collider2D collider in colliders)
        {
            // Check if the collider belongs to a hex other than this one
            if (collider.gameObject != gameObject && collider.TryGetComponent(out Hex adjacentHex))
            {
                AdjacentHexes.Add(adjacentHex);
            }
        }
    }

    public bool IsAdjacentToHex (Hex hex)
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
