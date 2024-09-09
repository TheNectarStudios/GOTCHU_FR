using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    public float speed = 5f;
    private Vector3 currentDirection = Vector3.zero;
    private Vector3 lastDirection = Vector3.zero;
    
    private bool canMove = true;

    private Vector2 startTouchPosition, endTouchPosition;

    void Update()
    {
        if (!IsOwner) return;  // Only control the local player

        DetectSwipe();

        // Continuously move the player in the current direction
        if (canMove)
        {
            // Ensure movement in the right direction
            transform.Translate(currentDirection * speed * Time.deltaTime, Space.World);
        }
    }

    // Swipe detection logic
    void DetectSwipe()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                startTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                endTouchPosition = touch.position;
                HandleSwipe();
            }
        }
    }

    void HandleSwipe()
    {
        Vector2 swipeDelta = endTouchPosition - startTouchPosition;

        // Check if swipe is significant
        if (swipeDelta.magnitude > 50)
        {
            // Check if it's a horizontal or vertical swipe
            if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
            {
                // Horizontal swipe
                if (swipeDelta.x > 0 && lastDirection != Vector3.right)
                {
                    currentDirection = Vector3.left;  // Inverted: Move left
                }
                else if (swipeDelta.x < 0 && lastDirection != Vector3.left)
                {
                    currentDirection = Vector3.right;  // Inverted: Move right
                }
            }
            else
            {
                // Vertical swipe
                if (swipeDelta.y > 0 && lastDirection != Vector3.back)
                {
                    currentDirection = Vector3.back;  // Inverted: Move down
                }
                else if (swipeDelta.y < 0 && lastDirection != Vector3.forward)
                {
                    currentDirection = Vector3.forward;  // Inverted: Move up
                }
            }

            lastDirection = currentDirection;  // Store the last direction to restrict opposite movement
        }
    }
}
