using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class DragManager : MonoBehaviour
{
    private bool isDragging = false;
    private RaycastHit2D hit;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isDragging)
        {
            int shopLayer = LayerMask.NameToLayer("Shop");
            int boardLayer = LayerMask.NameToLayer("Board");
            int benchLayer = LayerMask.NameToLayer("Bench");
            int layerMask = ~(1 << shopLayer) & ~(1 << boardLayer) & ~(1 << benchLayer);

            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, layerMask);

            // Start dragging if collider was hit
            if (hit.collider != null && hit.collider.gameObject.CompareTag("DraggableObject"))
            {
                StartDragging();
            }
        }
        // Continue dragging gameobject
        else if (isDragging && Input.GetMouseButton(0))
        {
            ContinueDragging(hit.collider.gameObject);
        }

        // Stop dragging gameobject
        else if (isDragging && Input.GetMouseButtonUp(0))
        {
            StopDragging();
        }
    }

    // Set the starting offset position
    private void StartDragging()
    {
        isDragging = true;

        hit.collider.gameObject.TryGetComponent(out GameUnit unit);
        if (unit != null)
        {
            unit.HandleDragStarted();
        }

    }

    // Change position of the draggableObject to the mouse position
    private void ContinueDragging(GameObject gameObject)
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        gameObject.transform.position = new Vector3(mousePosition.x, mousePosition.y);
    }

    private void StopDragging()
    {
        isDragging = false;

        hit.collider.gameObject.TryGetComponent(out GameUnit unit);
        if (unit != null)
        {
            GameObject objDraggedOn = GetObjectDraggedOn(hit.collider.gameObject.transform.position, hit.collider.gameObject);
            unit.HandleDragStopped(objDraggedOn);
        }
    }

    private GameObject GetObjectDraggedOn(Vector3 mousePosition, GameObject objToIgnore)
    {
        // Create a layer mask to ignore the collider of the dragged object
        int layerMask = ~(1 << objToIgnore.layer);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, layerMask);
        if (hit.collider != null)
        {
            return hit.collider.gameObject;
        }
        return null;
    }
}
