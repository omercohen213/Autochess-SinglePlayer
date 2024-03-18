using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemOrb : MonoBehaviour
{
    private float upDistance = 0.5f; // Distance to move up
    private float duration = 0.5f; // Duration of the animation in seconds

    private Vector3 initialPosition; // Initial position of the item
    private Vector3 finalPosition; // Final position of the item

    private void Start()
    {
        // Save the initial position of the item
        initialPosition = transform.position;

        // Calculate the final position (move up)
        finalPosition = initialPosition + Vector3.up * upDistance;

        // Start the drop animation coroutine
        StartCoroutine(ItemDropCoroutine());
    }

    private IEnumerator ItemDropCoroutine()
    {
        float elapsedTime = 0f;

        // Move the item up
        while (elapsedTime < duration)
        {
            // Increment the elapsed time
            elapsedTime += Time.deltaTime;

            // Calculate the progress (0 to 1) based on the elapsed time and duration
            float progress = Mathf.Clamp01(elapsedTime / duration);

            // Interpolate between initial and final positions using progress
            transform.position = Vector3.Lerp(initialPosition, finalPosition, progress);

            yield return null;
        }

        // Reset elapsed time for the next phase (falling down)
        elapsedTime = 0f;

        // Move the item back down
        while (elapsedTime < duration)
        {
            // Increment the elapsed time
            elapsedTime += Time.deltaTime;

            // Calculate the progress (0 to 1) based on the elapsed time and duration
            float progress = Mathf.Clamp01(elapsedTime / duration);

            // Interpolate between final and initial positions using progress
            transform.position = Vector3.Lerp(finalPosition, initialPosition, progress);

            yield return null;
        }

        // Ensure the item is back at its initial position
        transform.position = initialPosition;
    }
}
