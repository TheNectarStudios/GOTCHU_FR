using UnityEngine;

public class PlayerMovementOffline : MonoBehaviour
{
    public float speed = 5f;  // Player movement speed
    private DynamicJoystick joystick;  // Reference to the joystick
    private Vector3 direction;
    private Vector3 lastMovementDirection; // To store the last movement direction

    void Start()
    {
        joystick = FindObjectOfType<DynamicJoystick>();

        if (joystick == null)
        {
            Debug.LogError("Joystick not found in the scene!");
        }
    }

    void Update()
    {
        if (joystick == null) return;

        // Capture joystick input
        direction.x = joystick.Horizontal;
        direction.z = joystick.Vertical;

        // Ensure direction is constrained to up, down, left, and right
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
        {
            direction.z = 0;
        }
        else
        {
            direction.x = 0;
        }

        // Update last movement direction only if there is movement
        if (direction != Vector3.zero)
        {
            lastMovementDirection = direction;
        }

        MovePlayer();
    }

    void MovePlayer()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    // Method to get the last movement direction
    public Vector3 GetLastMovementDirection()
    {
        return lastMovementDirection;
    }
}
