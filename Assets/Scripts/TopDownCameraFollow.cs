using UnityEngine;

public class TopDownCameraFollow : MonoBehaviour
{
    public Transform target; // The player’s transform
    public Vector3 offset = new Vector3(0f, 60f, 0f); // Offset directly above the player
    public float smoothSpeed = 0.005f; // Smooth follow speed

    private Vector3 velocity = Vector3.zero;

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
}
