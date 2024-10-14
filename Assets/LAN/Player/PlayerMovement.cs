using UnityEngine;
using Photon.Pun;

public class PacMan3DMovement : MonoBehaviourPun
{
    public float speed = 5f;  // Player movement speed
    public DynamicJoystick joystick;  // Reference to the joystick

    private Vector3 direction;
    private Vector3 lastMovementDirection; // To store the last movement direction

    void Start()
    {
        // You can set up any additional initializations here
    }

    void Update()
    {
        if (!photonView.IsMine) return;  // Ensure only local player controls the movement

        // Get direction from joystick
        direction.x = joystick.Horizontal;  // Get horizontal input
        direction.z = joystick.Vertical;  // Get vertical input

        // Normalize direction to ensure consistent speed
        if (direction.magnitude > 1)
            direction.Normalize();

        // Update last movement direction
        lastMovementDirection = direction;

        MovePlayer();  // Move the player based on joystick input
    }

    void MovePlayer()
    {
        // Move the player in the direction calculated
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        // Send movement updates to other clients
        if (direction != Vector3.zero)
        {
            photonView.RPC("SyncMovement", RpcTarget.Others, direction);
        }
    }

    [PunRPC]
    void SyncMovement(Vector3 moveDirection)
    {
        // Move the player in the direction received from the RPC
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
    }

    // New method to get the last movement direction
    public Vector3 GetLastMovementDirection()
    {
        return lastMovementDirection; // Return the last recorded direction
    }
}
