using UnityEngine;
using Photon.Pun;

public class BulletPowerUp : MonoBehaviour
{
    public float duration = 10f; // Duration for how long the power-up lasts
    public GameObject bulletPrefab; // The bullet prefab the player can shoot

    private void OnTriggerEnter(Collider other)
    {
        // Ensure the player that picks up the power-up is controlled by the local client
        if (other.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine)
        {
            PlayerShooting playerShooting = other.GetComponent<PlayerShooting>();

            if (playerShooting != null)
            {
                // Give the player the ability to shoot by enabling shooting on the local player's script
                playerShooting.EnableShooting(bulletPrefab);

                // Optionally, hide the power-up and destroy it after a delay
                GetComponent<Renderer>().enabled = false; // Hide the power-up visually
                GetComponent<Collider>().enabled = false; // Disable the collider
                Destroy(gameObject, 1f); // Destroy the power-up after 1 second
            }
        }
    }
}
