using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent (typeof (Animator))]
public class PlayerCharacter : MonoBehaviour
{
    private readonly float SPEED = 4f;
    private Animator _animator;
    public GameObject _mouseClickPrefab;

    private Coroutine moveCoroutine;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Check for right mouse button click
        if (Input.GetMouseButtonDown(1))
        {
            // If a movement coroutine is already running, stop it
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }

            // Start a new movement coroutine to move the character
            moveCoroutine = StartCoroutine(MoveToMousePosition());
            InstantiateClickAnimation();
        }
    }

    private IEnumerator MoveToMousePosition()
    {
        _animator.SetBool("IsWalking", true);

        // Get the target position (mouse position)
        Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPosition.z = 0f; // Ensure z-position is the same as character's

        if (targetPosition.x > transform.position.x)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }

        // Move towards the target position smoothly
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            // Calculate the movement direction
            Vector3 direction = (targetPosition - transform.position).normalized;

            // Move the character towards the target position
            transform.position += direction * Time.deltaTime * SPEED; // Adjust speed as needed

            yield return null;
        }

        _animator.SetBool("IsWalking", false);
    }

    private void InstantiateClickAnimation()
    {
        // Instantiate click animation prefab at mouse position
        Vector3 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        clickPosition.z = 0f; // Ensure z-position is correct for 2D
        GameObject mouseClick = Instantiate(_mouseClickPrefab, clickPosition, Quaternion.identity);

        // Start the coroutine for the click animation
        StartCoroutine(ClickAnimationCoroutine(mouseClick));
    }

    private IEnumerator ClickAnimationCoroutine(GameObject mouseClick)
    {
        SpriteRenderer spriteRenderer = mouseClick.GetComponent<SpriteRenderer>();

        // Get initial scale
        Vector3 initialScale = mouseClick.transform.localScale;

        // Target scale (zero scale)
        Vector3 targetScale = Vector3.zero;

        float timer = 0f;
        float duration = 1f; // Duration of the animation in seconds

        while (timer < duration)
        {
            // Increment the timer
            timer += Time.deltaTime;

            // Calculate the progress (0 to 1) based on the timer and duration
            float progress = Mathf.Clamp01(timer / duration);

            // Interpolate between initial and target scale using progress
            Vector3 newScale = Vector3.Lerp(initialScale, targetScale, progress);

            // Apply the new scale to the sprite renderer
            spriteRenderer.transform.localScale = newScale;

            yield return null;
        }

        // Ensure the scale is set to the target scale at the end of the animation
        spriteRenderer.transform.localScale = targetScale;

        // Destroy the click animation object
        Destroy(mouseClick);
    }
}
