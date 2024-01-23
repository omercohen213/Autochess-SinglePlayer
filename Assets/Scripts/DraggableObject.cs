using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Check position of mouse press
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            // Start dragging if collider was hit
            if (hit.collider != null && hit.collider.gameObject == this.gameObject)
            {
                StartDragging(mousePosition);
            }
        }
        // Continue dragging gameobject
        else if (isDragging && Input.GetMouseButton(0))
        {
            ContinueDragging();
        }

        // Stop dragging gameobject
        else if (isDragging && Input.GetMouseButtonUp(0))
        {
            StopDragging();
        }
    }
    
    // Set the starting offset position
    private void StartDragging(Vector3 mousePosition)
    {
        isDragging = true;
        offset = transform.position - mousePosition;
    }

    // Change position of the draggableObject to the mouse position
    private void ContinueDragging()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mousePosition.x + offset.x, mousePosition.y + offset.y, transform.position.z);
    }

    // Invoke the OnDragStopped action for each draggableObject to allow custom implementation
    private void StopDragging()
    {
        isDragging = false;

        DragEvents.InvokeDragStopped(gameObject, transform.position);
    }
}
