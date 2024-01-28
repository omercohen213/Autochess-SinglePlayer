using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject _hexPrefab;
    [SerializeField] private Transform _board;

    public readonly int _ROWS = 5;
    public readonly int _COLUMNS = 8;
    private readonly float HEX_SPACING_Y = -1.35f; // Space between two hexes in the Y axis
    private readonly float HEX_SPACING_X = 1.15f; // Space between two hexes in the X axis
    private readonly float COLUMN_SHIFT_Y = 0.68f; // Shift odd numbered columns in the Y axis

    public static Board Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }

    private void Start()
    {
        Vector3 pos = transform.position; // Set initial hex position

        for (int i = 0; i < _COLUMNS; i++)
        {
            // Shift odd column numbered position 
            if (i % 2 == 1)
            {
                pos -= new Vector3(0, COLUMN_SHIFT_Y);
            }
            for (int j = 0; j < _ROWS; j++)
            {
                // Create the hex gameObject
                GameObject hexGo = Instantiate(_hexPrefab, pos, Quaternion.identity, transform);
                hexGo.AddComponent<Hex>();
                hexGo.GetComponent<Hex>().SetPosition(i, j);
                hexGo.name = $"Hex ({i},{j})"; 
                pos += new Vector3(0f, HEX_SPACING_Y); // Space out the hexes in the Y axis
            }
            pos = new Vector3(pos.x + HEX_SPACING_X, transform.position.y); // Space out in the X axis           
        }
    }

    // Sets the position of the unit on board
    public void PlaceUnitOnBoard(BoardUnit unitToPlace, Hex hex)
    {
        if (hex != null)
        {
            unitToPlace.transform.position = hex.transform.position;
        }
        Debug.Log("placed on hex: " + hex.ToString());
    }
}
