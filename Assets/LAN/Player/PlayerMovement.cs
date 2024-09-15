using UnityEngine;
using Photon.Pun;

public class PacMan3DMovement : MonoBehaviourPun
{
    public float speed = 5f;  // Player movement speed
    public LayerMask obstacleLayer;  // Layer for obstacles (maze walls)
    public Vector3 playerSize = new Vector3(1f, 1f, 1f);  // Size of the player for collision detection
    private Vector3 direction = Vector3.zero;
    private Vector3 nextDirection = Vector3.zero;
    private Vector3 initialPosition;
    private bool hasQueuedDirection = false;
    private bool isMoving = false;  // Flag to track if the player is moving
    private float checkDistance = 0.49f;  // Distance for obstacle check
    private float moveDistance = 2f;  // Size of each grid block, 2 units
    private Vector3 lastMovementDirection = Vector3.forward; // Last movement direction

    void Start()
    {
        if (photonView.IsMine)
        {
            initialPosition = transform.position;
        }
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        // Check for a queued movement direction and apply if possible
        if (hasQueuedDirection && CanMoveInDirection(nextDirection, checkDistance))
        {
            direction = nextDirection;
            hasQueuedDirection = false;
        }

        // Move in the current direction if possible
        if (CanMoveInDirection(direction, checkDistance))
        {
            MoveInDirection();
        }
    }

    public void MoveUp()
    {
        if (photonView.IsMine)
        {
            QueueDirection(Vector3.forward);
        }
    }

    public void MoveDown()
    {
        if (photonView.IsMine)
        {
            QueueDirection(Vector3.back);
        }
    }

    public void MoveLeft()
    {
        if (photonView.IsMine)
        {
            QueueDirection(Vector3.left);
        }
    }

    public void MoveRight()
    {
        if (photonView.IsMine)
        {
            QueueDirection(Vector3.right);
        }
    }

    void QueueDirection(Vector3 newDirection)
    {
        nextDirection = newDirection;
        hasQueuedDirection = true;
        lastMovementDirection = newDirection; // Update last movement direction
    }

    // Move the player in the current direction continuously
    void MoveInDirection()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    bool CanMoveInDirection(Vector3 dir, float distance)
    {
        // Use a box cast to check for obstacles with the player's size
        Vector3 halfExtents = playerSize / 1.18f;
        RaycastHit hit;

        // Cast the box in the direction of movement
        bool hitSomething = Physics.BoxCast(transform.position, halfExtents, dir, out hit, Quaternion.identity, distance / 2, obstacleLayer);

        return !hitSomething;  // Return true if no obstacles were hit
    }

    public void ResetPosition()
    {
        if (photonView.IsMine)
        {
            transform.position = initialPosition;
            direction = Vector3.zero;
            nextDirection = Vector3.zero;
            hasQueuedDirection = false;
        }
    }

    // Method to get the last movement direction
    public Vector3 GetLastMovementDirection()
    {
        return lastMovementDirection;
    }
}
