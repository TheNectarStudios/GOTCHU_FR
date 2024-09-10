using UnityEngine;

public class PacManMovement : MonoBehaviour
{
    public float speed = 5f;  // Movement speed
    public LayerMask obstacleLayer;  // LayerMask to detect obstacles
    private Vector3 direction = Vector3.zero;  // Current movement direction
    private Vector3 nextDirection = Vector3.zero;  // Next desired movement direction

    private Vector3 initialPosition;  // Stores initial position to reset when needed
    private Vector3[] directions = new Vector3[] { Vector3.right, Vector3.left, Vector3.up, Vector3.down };  // Possible movement directions

    void Start()
    {
        initialPosition = transform.position;  // Store initial position
    }

    void Update()
    {
        HandleInput();  // Handle player input
        Move();  // Handle movement
    }

    // Handle player input for direction change
    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
            nextDirection = Vector3.up;  // Up
        else if (Input.GetKeyDown(KeyCode.S))
            nextDirection = Vector3.down;  // Down
        else if (Input.GetKeyDown(KeyCode.A))
            nextDirection = Vector3.left;  // Left
        else if (Input.GetKeyDown(KeyCode.D))
            nextDirection = Vector3.right;  // Right
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

    // Reset Pac-Man to initial position (if needed, for example, after death or restart)
    public void ResetPosition()
    {
        transform.position = initialPosition;
        direction = Vector3.zero;
        nextDirection = Vector3.zero;
    }
}
