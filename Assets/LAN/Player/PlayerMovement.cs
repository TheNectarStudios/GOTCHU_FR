using UnityEngine;
using Photon.Pun;

public class PacMan3DMovement : MonoBehaviourPun
{
    public float speed = 5f;
    public LayerMask obstacleLayer;
    public Vector3 playerSize = new Vector3(1f, 1f, 1f);  // Size of the player for collision detection
    private Vector3 direction = Vector3.zero;
    private Vector3 nextDirection = Vector3.zero;
    private Vector3 initialPosition;
    private bool hasQueuedDirection = false;
    public float snapThreshold = 0.1f;  // The threshold for snapping to grid

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
        Move();
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
        photonView.RPC("SyncDirection", RpcTarget.Others, newDirection);
    }

    void Move()
    {
        // Check if we can apply the next direction
        if (hasQueuedDirection && CanMoveInDirection(nextDirection))
        {
            direction = nextDirection;
            hasQueuedDirection = false;
        }

        // Move in the current direction
        if (CanMoveInDirection(direction))
        {
            transform.Translate(direction * speed * Time.deltaTime, Space.World);
        }
    }

    bool CanMoveInDirection(Vector3 dir)
    {
        // Use a box cast to check for obstacles with the player's size
        Vector3 halfExtents = playerSize / 2f;  // Half extents for the box cast (like a bounding box)
        RaycastHit hit;
        
        // Cast the box slightly ahead in the direction of movement
        bool hitSomething = Physics.BoxCast(transform.position, halfExtents, dir, out hit, Quaternion.identity, 0.75f, obstacleLayer);

        return !hitSomething;  // Return true if no obstacles were hit
    }

    // Check if the player's position is aligned for turning
    bool IsAlignedForTurn()
    {
        Vector3 currentPos = transform.position;

        // Allow snapping along the X and Z axes depending on the direction
        if (direction == Vector3.forward || direction == Vector3.back)
        {
            return Mathf.Abs(Mathf.Round(currentPos.x) - currentPos.x) < snapThreshold;
        }
        else if (direction == Vector3.left || direction == Vector3.right)
        {
            return Mathf.Abs(Mathf.Round(currentPos.z) - currentPos.z) < snapThreshold;
        }

        return false;
    }

    void SyncDirection(Vector3 newDirection)
    {
        if (photonView.IsMine) return;
        nextDirection = newDirection;
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
}
