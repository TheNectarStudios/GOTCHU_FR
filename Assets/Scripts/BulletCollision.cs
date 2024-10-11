using UnityEngine;
using Photon.Pun;

public class BulletCollision : MonoBehaviour
{
    public float destroyDelay = 2f; // Time after which the bullet will be destroyed if no collision happens
    public GameObject impactPrefab; // The impact effect prefab to be instantiated upon collision

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

            // If it's a ghost, handle ghost-specific logic
            if (other.CompareTag("Ghost"))
            {
                Debug.Log("Bullet hit the ghost!");

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

            // Destroy the bullet across the network after the impact
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Destroy the bullet after a certain amount of time (fallback) to prevent it from lingering
        Destroy(gameObject, destroyDelay);
    }
}
