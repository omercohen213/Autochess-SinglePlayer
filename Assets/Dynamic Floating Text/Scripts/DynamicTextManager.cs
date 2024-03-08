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

    public static void CreateDamageText(Vector2 position, string text, bool isCritical)
    {
        position.x += (Random.value - 0.5f) / 5f;
        position.y += Random.value / 5f + 1f;
        GameObject newText = Instantiate(CanvasPrefab, position, Quaternion.identity, Transform);
        newText.transform.GetComponent<DynamicText2D>().Initialize(text, isCritical);
    }

}
