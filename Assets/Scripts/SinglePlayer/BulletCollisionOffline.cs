using UnityEngine;

public class BulletCollisionOffline : MonoBehaviour
{
    public float destroyDelay = 2f; // Time after which the bullet will be destroyed if no collision happens
    public GameObject impactPrefab; // The impact effect prefab to be instantiated upon collision
    public AudioClip impactSound; // Sound to be played upon impact
    private AudioSource audioSource; // Reference to the AudioSource component

    private void Start()
    {
        // Create an AudioSource component if not already attached
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = impactSound; // Set the audio clip

        // Destroy the bullet after a certain amount of time (fallback) to prevent it from lingering
        Destroy(gameObject, destroyDelay);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the bullet collided with the "Ghost" tag or the "maze" layer
        if (other.CompareTag("Ghost") || other.gameObject.layer == LayerMask.NameToLayer("maze"))
        {
            // Instantiate the impact effect at the bullet's position and rotation
            if (impactPrefab != null)
            {
                Instantiate(impactPrefab, transform.position, transform.rotation);
            }

            // Play the impact sound
            if (impactSound != null)
            {
                audioSource.Play(); // Play the impact sound
            }

            // If it's a ghost, handle ghost-specific logic
            if (other.CompareTag("Ghost"))
            {
                // Debug.Log("Bullet hit the ghost!");

                // Access the GhostHitManager component from the ghost
                GhostHitManager ghostHitManager = other.GetComponent<GhostHitManager>();

                if (ghostHitManager != null)
                {
                    // Teleport the ghost when hit
                    ghostHitManager.TeleportToSpawnPoint();

                    // Trigger camera shake via GhostHitManager
                    TopDownCameraFollow cameraFollow = FindObjectOfType<TopDownCameraFollow>();
                    if (cameraFollow != null)
                    {
                        cameraFollow.ShakeCamera(); // Trigger the camera shake
                        Debug.Log("Camera shake triggered on bullet hit!");
                    }
                }
            }

            // Destroy the bullet after the impact
            Destroy(gameObject);
        }
    }
}
