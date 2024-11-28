using UnityEngine;
using UnityEngine.InputSystem;

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
    private Vector2 moveInput;              // Movement input from the player
    private Vector2 rotateInput;            // Rotation input from the player
    private Vector3 wobbleOffset;
    private Vector3 desiredTilt;            // Desired tilt direction
    private Quaternion initialRotation;     // UFO's initial upright rotation

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;  // UFOs don't need gravity!
        rb.drag = 1f;           // Helps with realistic inertia
        initialRotation = transform.rotation; // Store initial upright rotation
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
        // Get input from Unity's Input System
        var keyboard = Keyboard.current;

        if (keyboard != null)
        {
            // Movement (WASD)
            moveInput = new Vector2(
                (keyboard.aKey.isPressed ? -1 : 0) + (keyboard.dKey.isPressed ? 1 : 0),
                (keyboard.sKey.isPressed ? -1 : 0) + (keyboard.wKey.isPressed ? 1 : 0)
            );

            // Rotation (Mouse movement)
            var mouse = Mouse.current;
            if (mouse != null)
            {
                rotateInput = new Vector2(
                    mouse.delta.x.ReadValue(),
                    mouse.delta.y.ReadValue()
                );
            }
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
        // Use rotateInput for yaw
        float yaw = rotateInput.x * rotationSpeed * Time.fixedDeltaTime;

        // Apply yaw rotation
        Quaternion yawRotation = Quaternion.Euler(0, yaw, 0);
        rb.MoveRotation(rb.rotation * yawRotation);
    }

    private void ApplyWobble()
    {
        // Create a smooth oscillating wobble effect
        float wobbleX = Mathf.Sin(Time.time * wobbleSpeed) * wobbleIntensity;
        float wobbleZ = Mathf.Cos(Time.time * wobbleSpeed) * wobbleIntensity;

        wobbleOffset = new Vector3(wobbleX, 0, wobbleZ);

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
