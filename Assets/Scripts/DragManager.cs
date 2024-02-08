using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DragManager : MonoBehaviour
{
    private bool isDragging = false;
    private RaycastHit2D[] hits;
    private GameObject draggedObject;
    GameObject lastHexHovered = null;
    Color hexColor;

    private int shopLayer;
    private int benchLayer;
    private int layerMask;


    private void Start()
    {
        shopLayer = LayerMask.NameToLayer("Shop");
        benchLayer = LayerMask.NameToLayer("Bench");
        layerMask = ~(1 << shopLayer) & ~(1 << benchLayer);

        hexColor = FindFirstObjectByType<Hex>().GetComponent<SpriteRenderer>().color;
    }

    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        hits = Physics2D.RaycastAll(mousePosition, Vector2.zero, Mathf.Infinity, layerMask);
        if (Input.GetMouseButtonDown(0) && !isDragging)
        {

            // Loop through all hits
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null && hit.collider.gameObject.CompareTag("DraggableObject"))
                {
                    draggedObject = hit.collider.gameObject;
                    StartDragging();
                }
            }
        }
        // Continue dragging gameobject
        else if (isDragging && Input.GetMouseButton(0))
        {
            ContinueDragging();

            GameObject hex = null;
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null && hit.collider.gameObject.CompareTag("Hex"))
                {
                    hex = hit.collider.gameObject;
                    hex.GetComponent<SpriteRenderer>().color = Color.gray;
                    break;
                }
            }

            // Check if the hex under the mouse has changed
            if (hex != lastHexHovered)
            {
                // Reset color of the last hex
                if (lastHexHovered != null)
                {
                    lastHexHovered.GetComponent<SpriteRenderer>().color = hexColor;
                }
                lastHexHovered = hex;
            }
        }
        // Stop dragging gameobject
        else if (isDragging && Input.GetMouseButtonUp(0))
        {
            StopDragging();
            lastHexHovered.GetComponent<SpriteRenderer>().color = hexColor;
        }
    }

    // Set the starting offset position
    private void StartDragging()
    {
        isDragging = true;

        draggedObject.TryGetComponent(out GameUnit unit);
        if (unit != null)
        {
            unit.HandleDragStarted();
        }

    }

    // Change position of the draggableObject to the mouse position
    private void ContinueDragging()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        draggedObject.transform.position = new Vector3(mousePosition.x, mousePosition.y);
    }

    private void StopDragging()
    {
        isDragging = false;
        draggedObject.TryGetComponent(out GameUnit unit);
        if (unit != null)
        {
            GameObject objDraggedOn = GetObjectDraggedOn(draggedObject.transform.position, draggedObject);
            unit.HandleDragStopped(objDraggedOn);
        }
    }

    private GameObject GetObjectDraggedOn(Vector3 position, GameObject objToIgnore)
    {
        // Create a layer mask to ignore the collider of the dragged object
        int layerMask = ~(1 << objToIgnore.layer);
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero, Mathf.Infinity, layerMask);
        if (hit.collider != null)
        {
            return hit.collider.gameObject;
        }
        return null;
    }
}
