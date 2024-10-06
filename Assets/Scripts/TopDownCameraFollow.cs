using UnityEngine;
using System.Collections;

public class TopDownCameraFollow : MonoBehaviour
{
    public Transform target; // The player’s transform
    public Vector3 offset = new Vector3(0f, 60f, 0f); // Offset directly above the player
    public float smoothSpeed = 0.005f; // Smooth follow speed

    private Vector3 velocity = Vector3.zero;
    private bool isShaking = false;
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0f;

    void LateUpdate()
    {
        if (target == null) return;

        // Calculate the desired camera position based on target position and offset
        Vector3 desiredPosition = target.position + offset;

        // If the camera is shaking, apply a slight offset
        if (isShaking)
        {
            desiredPosition += Random.insideUnitSphere * shakeMagnitude;
        }

        // Smooth the camera movement
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);

        // Set the new position of the camera
        transform.position = smoothedPosition;

        // Make sure the camera is always looking straight down
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }

    // Function to start the camera shake
    public void ShakeCamera(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
        StartCoroutine(CameraShake());
    }

    // Coroutine to handle the shake duration and magnitude
    private IEnumerator CameraShake()
    {
        isShaking = true;

        // Wait for the duration to pass
        yield return new WaitForSeconds(shakeDuration);

        // After the shake duration, stop shaking
        isShaking = false;
    }
}
