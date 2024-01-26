using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragEvents : MonoBehaviour
{
    // Allows a draggable object to call a customized OnDragStopped method
    public static event Action<GameObject, Vector3> OnDragStopped; 
    // Allows a draggable object to call a customized OnDragStopped method
    public static event Action<GameObject> OnDragStarted; 
    public static void InvokeDragStopped(GameObject sender, Vector3 finalPosition)
    {
        OnDragStopped?.Invoke(sender, finalPosition);
    }

    public static void InvokeDragStarted(GameObject sender)
    {
        OnDragStarted?.Invoke(sender);
    }
}
