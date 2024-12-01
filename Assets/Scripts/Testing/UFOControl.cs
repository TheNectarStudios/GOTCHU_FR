using UnityEngine;
using UnityEngine.UI; // Import for UI Button
using System.Collections; // Import for Coroutine

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
    public float speedBoostMultiplier = 2f; // Multiplier for speed boost
    public float boostDuration = 3f;        // Duration of the speed boost in seconds
    public float jerkIntensity = 50f;       // Intensity of the initial jerk on boost

    private Rigidbody rb;
    private Vector2 moveInput;              // Movement input from the joystick
    private float yawInput;                 // Rotation input for yaw
    private Vector3 desiredTilt;            // Desired tilt direction
    private Quaternion initialRotation;     // UFO's initial upright rotation
    private DynamicJoystick joystick;       // Reference to the joystick
    private bool isBoosting = false;        // To check if the boost is active

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

        // Check for speed boost activation via G key
        if (Input.GetKeyDown(KeyCode.G))
        {
            TriggerSpeedBoost();
        }
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

            // Snap input to cardinal directions
            if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
            {
                // Horizontal movement dominates
                moveInput.y = 0;
            }
            else
            {
                // Vertical movement dominates
                moveInput.x = 0;
            }
        }
    }

    private void HandleMovement()
    {
        // Calculate forces from input
        float currentThrustForce = isBoosting ? thrustForce * speedBoostMultiplier : thrustForce;
        float currentStrafeForce = isBoosting ? strafeForce * speedBoostMultiplier : strafeForce;

        Vector3 forwardForce = transform.forward * moveInput.y * currentThrustForce;
        Vector3 rightForce = transform.right * moveInput.x * currentStrafeForce;

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

    public void TriggerSpeedBoost()
    {
        if (!isBoosting)
        {
            StartCoroutine(SpeedBoostCoroutine());
            ApplyJerk(); // Apply the initial jerk effect when boost is activated
        }
    }

    private IEnumerator SpeedBoostCoroutine()
    {
        isBoosting = true;
        yield return new WaitForSeconds(boostDuration);
        isBoosting = false;
    }

    private void ApplyJerk()
    {
        // Calculate the current movement direction
        Vector3 currentDirection = rb.velocity.normalized;

        if (currentDirection == Vector3.zero)
        {
            // If there's no current movement, use the forward direction
            currentDirection = transform.forward;
        }

        // Apply an instantaneous force in the current direction for the jerk effect
        rb.AddForce(currentDirection * jerkIntensity, ForceMode.Impulse);
    }
}
