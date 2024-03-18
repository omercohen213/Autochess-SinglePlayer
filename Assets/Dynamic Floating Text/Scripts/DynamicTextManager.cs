using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;

public class DynamicTextManager : MonoBehaviour
{
    public static GameObject CanvasPrefab;
    public static Transform Transform;
    [SerializeField] private GameObject _canvasPrefab;

    private void Awake()
    {
        CanvasPrefab = _canvasPrefab;
        Transform = transform;
    }

    public static void CreateFloatingText(Vector2 position, string text, TextType textType)
    {
        Color color;

        switch (textType)
        {
            case (TextType.Normal):
                color = new Color(255 / 255f, 165 / 255f, 0);
                break;
            case (TextType.Critical):
                color = Color.red;
                break;
            case (TextType.Heal):
                color = Color.green;
                break;
            case (TextType.Magic):
                color = Color.magenta;
                break;
            default:
                color = new Color(255 / 255f, 165 / 255f, 0);
                break;
        }

        position.x += (Random.value - 0.5f) / 5f;
        position.y += Random.value / 5f + 1f;
        GameObject newText = Instantiate(CanvasPrefab, position, Quaternion.identity, Transform);
        newText.transform.GetComponent<DynamicText2D>().Initialize(text, color);
    }
}

public enum TextType
{
    Normal,
    Critical,
    Magic,
    Heal
}
