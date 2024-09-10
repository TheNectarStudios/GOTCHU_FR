using UnityEngine;
using Photon.Pun;

public class PacMan3DMovement : MonoBehaviourPun
{
    public float speed = 5f;
    public LayerMask obstacleLayer;
    private Vector3 direction = Vector3.zero;
    private Vector3 nextDirection = Vector3.zero;
    private Vector3 initialPosition;

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
            SetNextDirection(Vector3.forward);
        }
    }

    public void MoveDown()
    {
        if (photonView.IsMine)
        {
            SetNextDirection(Vector3.back);
        }
    }

    public void MoveLeft()
    {
        if (photonView.IsMine)
        {
            SetNextDirection(Vector3.left);
        }
    }

    public void MoveRight()
    {
        if (photonView.IsMine)
        {
            SetNextDirection(Vector3.right);
        }
    }

    void SetNextDirection(Vector3 newDirection)
    {
        if (!IsOppositeDirection(newDirection))
        {
            nextDirection = newDirection;
            photonView.RPC("SyncDirection", RpcTarget.Others, newDirection);
        }
    }

    void Move()
    {
        if (nextDirection != direction && CanMoveInDirection(nextDirection))
        {
            direction = nextDirection;
        }

        if (CanMoveInDirection(direction))
        {
            transform.Translate(direction * speed * Time.deltaTime, Space.World);
           
        }
    }

    bool CanMoveInDirection(Vector3 dir)
    {
        Ray ray = new Ray(transform.position, dir);
        RaycastHit hit;
        return !Physics.Raycast(ray, out hit, 1f, obstacleLayer);
    }

    bool IsOppositeDirection(Vector3 newDirection)
    {
        return Vector3.Dot(direction, newDirection) < -0.9f;
    }


    void SyncDirection(Vector3 direction)
    {
        if (photonView.IsMine) return;
        SetNextDirection(direction);
    }


    void UpdatePosition(Vector3 newPosition)
    {
        if (!photonView.IsMine)
        {
            transform.position = newPosition;
        }
    }

    public void ResetPosition()
    {
        if (photonView.IsMine)
        {
            transform.position = initialPosition;
            direction = Vector3.zero;
            nextDirection = Vector3.zero;
        }
    }
}
