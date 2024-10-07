using UnityEngine;
using System.Collections;

public class TopDownCameraFollow : MonoBehaviour
{
    public Transform target; // The player’s transform
    public Vector3 offset = new Vector3(0f, 60f, 0f); // Offset directly above the player
    public float smoothSpeed = 0.005f; // Smooth follow speed

    // Variables for controlling the shake effect
    [Header("Camera Shake Settings")]
    public float shakeDuration = 0.5f;  // Default shake duration
    public float shakeMagnitude = 0.2f; // Default shake intensity

    private Vector3 velocity = Vector3.zero;
    private bool isShaking = false;

    void LateUpdate()
    {
        if (target == null) return;

        // Calculate the desired camera position based on target position and offset
        Vector3 desiredPosition = target.position + offset;

        // Smooth the camera movement
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);

        // Set the new position of the camera
        transform.position = smoothedPosition;

        // Make sure the camera is always looking straight down
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }

    // Method to trigger camera shake with adjustable duration and magnitude
    public void ShakeCamera(float? customDuration = null, float? customMagnitude = null)
    {
        if (!isShaking)
        {
            // Use provided values or fall back to default shake settings
            float duration = customDuration ?? shakeDuration;
            float magnitude = customMagnitude ?? shakeMagnitude;
            StartCoroutine(Shake(duration, magnitude));
        }
    }

    // Coroutine for shaking the camera
    private IEnumerator Shake(float duration, float magnitude)
    {
        isShaking = true;
        Vector3 originalPosition = transform.position;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // Randomly shake the camera by changing its position
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetZ = Random.Range(-1f, 1f) * magnitude;

            transform.position = new Vector3(originalPosition.x + offsetX, originalPosition.y, originalPosition.z + offsetZ);

            elapsed += Time.deltaTime;

            yield return null;
        }

        // After the shake, reset the camera back to its original position
        transform.position = originalPosition;
        isShaking = false;
    }
}
