using UnityEngine;
using Photon.Pun;

public class BulletCollision : MonoBehaviour
{
    public float destroyDelay = 2f; // Time after which the bullet will be destroyed if no collision happens

    private void OnTriggerEnter(Collider other)
    {
        // Check if the bullet collided with a ghost
        if (other.CompareTag("Ghost"))
        {
            Debug.Log("Bullet hit the ghost!");

            // Access the GhostHitManager component from the ghost
            GhostHitManager ghostHitManager = other.GetComponent<GhostHitManager>();

            if (ghostHitManager != null)
            {
                // Teleport the ghost when hit
                ghostHitManager.TeleportToSpawnPoint();
            }

            // Destroy the bullet across the network
            PhotonNetwork.Destroy(gameObject);
        }
        // Check if the bullet collided with the maze (by checking the layer)
        else if (other.gameObject.layer == LayerMask.NameToLayer("maze"))
        {
            Debug.Log("Bullet hit the maze and is destroyed!");

            // Destroy the bullet immediately upon collision with the maze
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Destroy the bullet after a certain amount of time (fallback) to prevent it from lingering
        Destroy(gameObject, destroyDelay);
    }
}
