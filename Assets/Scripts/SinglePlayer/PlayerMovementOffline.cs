using UnityEngine;

public class PlayerMovementOffline : MonoBehaviour
{
    public float speed = 5f;  // Original player movement speed
    public float maxSpeed = 5f;  // Maximum speed the player can reach
    public float acceleration = 5f;  // How fast the player accelerates
    public float deceleration = 5f;  // How fast the player decelerates
    private float currentSpeed = 0f;  // The current speed of the player

    private DynamicJoystick joystick;  // Reference to the joystick
    private Vector3 direction;
    private Vector3 lastMovementDirection;  // To store the last movement direction

    void Start()
    {
        joystick = FindObjectOfType<DynamicJoystick>();

        if (joystick == null)
        {
            Debug.LogError("Joystick not found in the scene!");
        }

        // Initialize speed with maxSpeed
        speed = maxSpeed;
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

        // Handle acceleration and deceleration
        if (direction.magnitude > 0)
        {
            // Accelerate
            currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, maxSpeed);
        }
        else
        {
            // Decelerate
            currentSpeed = Mathf.Max(currentSpeed - deceleration * Time.deltaTime, 0f);
        }

        // Move the player using the current speed
        MovePlayer();
    }

    void MovePlayer()
    {
        transform.Translate(direction.normalized * currentSpeed * Time.deltaTime, Space.World);
    }

    // Method to get the last movement direction
    public Vector3 GetLastMovementDirection()
    {
        return lastMovementDirection;
    }
}
