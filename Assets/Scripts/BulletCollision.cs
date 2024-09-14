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

            // Get the PhotonView of the ghost to ensure network synchronization
            PhotonView ghostPhotonView = other.GetComponent<PhotonView>();

            if (ghostPhotonView != null && PhotonNetwork.IsMasterClient)
            {
                // Handle the ghost's death or respawn logic here
                HandleGhostHit(ghostPhotonView);
            }

            // Destroy the bullet across the network
            PhotonNetwork.Destroy(gameObject); // Destroy the bullet after collision
        }
        // Check if the bullet collided with the maze (by checking the layer)
        else if (other.gameObject.layer == LayerMask.NameToLayer("maze"))
        {
            Debug.Log("Bullet hit the maze and is destroyed!");

            // Destroy the bullet immediately upon collision with the maze
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void HandleGhostHit(PhotonView ghostPhotonView)
    {
        // Add your logic here to handle what happens when the ghost is hit.
        // Example: respawn the ghost or turn it into a spectator
        Debug.Log("Handling ghost hit logic");
    }

    private void Start()
    {
        // Destroy the bullet after a certain amount of time (fallback) to prevent it from lingering
        Destroy(gameObject, destroyDelay);
    }
}
