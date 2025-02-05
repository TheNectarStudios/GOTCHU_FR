using UnityEngine;
using Photon.Pun;

public class PacMan3DMovement : MonoBehaviourPun
{
    public float speed = 5f;  // Player movement speed
    private DynamicJoystick joystick;  // Reference to the joystick
    private Vector3 direction;
    private Vector3 lastMovementDirection; // To store the last movement direction

    void Start()
    {
        if (photonView.IsMine)
        {
            // Dynamically find the joystick component in the scene
            joystick = FindObjectOfType<DynamicJoystick>();

            // Optional: Add a check to ensure the joystick is found
            if (joystick == null)
            {
                Debug.LogError("Joystick not found in the scene!");
            }
        }
    }

    void Update()
    {
        if (!photonView.IsMine || joystick == null) return;  // Ensure local control and that joystick is detected

        // Capture joystick input for local player
        direction.x = joystick.Horizontal;  
        direction.z = joystick.Vertical;

        // Ensure direction is constrained to up, down, left, and right (no diagonal)
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
        {
            direction.z = 0;  // Disable vertical movement if horizontal is stronger
        }
        else
        {
            direction.x = 0;  // Disable horizontal movement if vertical is stronger
        }

        // Update last movement direction
        lastMovementDirection = direction;

        MovePlayer();  // Move the player based on joystick input
    }

    void MovePlayer()
    {
        // Move the player in the direction calculated
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        // Send movement updates to other clients if the player is moving
        if (direction != Vector3.zero)
        {
            photonView.RPC("SyncMovement", RpcTarget.Others, direction);
        }
    }

    [PunRPC]
    void SyncMovement(Vector3 moveDirection)
    {
        // Sync the movement for other players in the network
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
    }

    // Method to get the last movement direction (if needed)
    public Vector3 GetLastMovementDirection()
    {
        return lastMovementDirection; // Return the last recorded direction
    }
}
