using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class UFOControl : MonoBehaviour
{
    public float thrustForce = 10f;          // Forward/backward thrust
    public float strafeForce = 10f;         // Side-to-side movement
    public float rotationSpeed = 50f;       // Rotation speed for yaw
    public float tiltIntensity = 10f;       // Intensity of tilt based on movement
    public float tiltSmoothing = 5f;        // Speed of tilt adjustment
    public float wobbleIntensity = 0.5f;    // Intensity of the wobble
    public float wobbleSpeed = 2f;          // Speed of the wobble

    private Rigidbody rb;
    private Vector2 moveInput;              // Movement input from the joystick
    private float yawInput;                 // Rotation input for yaw
    private Vector3 desiredTilt;            // Desired tilt direction
    private Quaternion initialRotation;     // UFO's initial upright rotation

    private DynamicJoystick joystick;       // Reference to the joystick

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;              // UFOs don't need gravity!
        rb.drag = 1f;                       // Helps with realistic inertia
        initialRotation = transform.rotation;

        // Find the joystick in the scene
        joystick = FindObjectOfType<DynamicJoystick>();
        if (joystick == null)
        {
            Debug.LogError("Joystick not found in the scene!");
        }
    }

    private void Update()
    {
        HandleInput();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
        ApplyWobble();
        ApplyTilt();
    }

    private void HandleInput()
    {
        if (joystick != null)
        {
            // Get movement input from the joystick
            moveInput = new Vector2(joystick.Horizontal, joystick.Vertical);

            // Ensure moveInput magnitude is clamped between -1 and 1
            if (moveInput.magnitude > 1f)
            {
                moveInput.Normalize();
            }

            // Use the joystick's horizontal axis for yaw rotation (or modify for another joystick)
            yawInput = joystick.Horizontal;
        }
    }

    private void HandleMovement()
    {
        // Calculate forces from input
        Vector3 forwardForce = transform.forward * moveInput.y * thrustForce;
        Vector3 rightForce = transform.right * moveInput.x * strafeForce;

        // Apply forces to the Rigidbody
        rb.AddForce(forwardForce + rightForce);
    }

    private void HandleRotation()
    {
        // Apply yaw rotation based on joystick input
        float yaw = yawInput * rotationSpeed * Time.fixedDeltaTime;
        Quaternion yawRotation = Quaternion.Euler(0, yaw, 0);
        rb.MoveRotation(rb.rotation * yawRotation);
    }

    private void ApplyWobble()
    {
        // Create a smooth oscillating wobble effect
        float wobbleX = Mathf.Sin(Time.time * wobbleSpeed) * wobbleIntensity;
        float wobbleZ = Mathf.Cos(Time.time * wobbleSpeed) * wobbleIntensity;

        Vector3 wobbleOffset = new Vector3(wobbleX, 0, wobbleZ);

        // Add wobble to the UFO's position
        rb.position += wobbleOffset * Time.fixedDeltaTime;
    }

    private void ApplyTilt()
    {
        // Calculate tilt based on movement input
        desiredTilt.x = moveInput.y * tiltIntensity;  // Forward/backward tilt
        desiredTilt.z = -moveInput.x * tiltIntensity; // Side-to-side tilt

        // Create a rotation based on the desired tilt
        Quaternion tiltRotation = Quaternion.Euler(desiredTilt.x, 0, desiredTilt.z);

        // Smoothly interpolate to the desired tilt rotation
        rb.MoveRotation(Quaternion.Lerp(rb.rotation, initialRotation * tiltRotation, Time.fixedDeltaTime * tiltSmoothing));
    }
}
