using UnityEngine;

public class LeanTowardsMovement : MonoBehaviour
{
    public float rotationSpeed = 5f; // Speed of rotation towards the velocity direction

    private Vector3 previousPosition;

    void Start()
    {
        previousPosition = transform.position; // Initialize the previous position
    }

    void Update()
    {
        Vector3 velocity = (transform.position - previousPosition) / Time.deltaTime; // Calculate velocity
        previousPosition = transform.position; // Update the previous position

        // If the object is moving, rotate it towards the movement direction
        if (velocity.magnitude > 0.1f)
        {
            // Calculate the direction the object is moving in
            Vector3 direction = velocity.normalized;

            // Find the yaw angle (rotation around Y-axis) based on movement direction
            float targetYawAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            // Current rotation
            Vector3 currentRotation = transform.rotation.eulerAngles;

            // Create a new rotation that only affects the Y-axis (keeping X and Z unchanged)
            Quaternion targetRotation = Quaternion.Euler(currentRotation.x, targetYawAngle, currentRotation.z);

            // Smoothly rotate towards the target Y-axis rotation
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
