using UnityEngine;
using UnityEngine.UI;  // Import Unity UI for button functionality

public class PacMan3DMovement : MonoBehaviour
{
    public float speed = 5f;  // Movement speed
    public LayerMask obstacleLayer;  // LayerMask to detect obstacles
    private Vector3 direction = Vector3.zero;  // Current movement direction
    private Vector3 nextDirection = Vector3.zero;  // Next desired movement direction

    private Vector3 initialPosition;  // Store the initial position to reset if needed

    void Start()
    {
        initialPosition = transform.position;  // Store initial position
    }

    void Update()
    {
        Move();  // Handle movement
    }

    // Assign the movement functions to buttons
    public void MoveUp()
    {
        SetNextDirection(Vector3.forward);  // Move up
    }

    public void MoveDown()
    {
        SetNextDirection(Vector3.back);  // Move down
    }

    public void MoveLeft()
    {
        SetNextDirection(Vector3.left);  // Move left
    }

    public void MoveRight()
    {
        SetNextDirection(Vector3.right);  // Move right
    }

    // Method to set the next direction if it's not the opposite direction
    void SetNextDirection(Vector3 newDirection)
    {
        if (!IsOppositeDirection(newDirection))
        {
            nextDirection = newDirection;
        }
    }

    // Movement and obstacle detection logic
    void Move()
    {
        // Try to move in the nextDirection if possible
        if (nextDirection != direction && CanMoveInDirection(nextDirection))
        {
            direction = nextDirection;
        }

        // If there's no obstacle in the current direction, keep moving
        if (CanMoveInDirection(direction))
        {
            transform.Translate(direction * speed * Time.deltaTime, Space.World);
        }
    }

    // Check if Pac-Man can move in the specified direction
    bool CanMoveInDirection(Vector3 dir)
    {
        // Cast a ray in the intended direction to check for obstacles
        Ray ray = new Ray(transform.position, dir);
        RaycastHit hit;

        // Perform raycast slightly beyond the current position to detect any obstacles
        if (Physics.Raycast(ray, out hit, 1f, obstacleLayer))
        {
            // If an obstacle is detected within a small distance, return false
            return false;
        }

        return true;  // No obstacle detected, can move
    }

    // Check if the new direction is directly opposite to the current direction
    bool IsOppositeDirection(Vector3 newDirection)
    {
        // Opposite direction is determined if their dot product is -1
        return Vector3.Dot(direction, newDirection) < -0.9f;
    }

    // Reset Pac-Man to initial position (if needed, for example, after death or restart)
    public void ResetPosition()
    {
        transform.position = initialPosition;
        direction = Vector3.zero;
        nextDirection = Vector3.zero;
    }
}
