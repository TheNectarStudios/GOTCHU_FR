using UnityEngine;

public class BulletPowerUp : MonoBehaviour
{
    public float duration = 10f; // Duration for how long the power-up lasts
    public GameObject bulletPrefab; // The bullet prefab the player can shoot

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerShooting playerShooting = other.GetComponent<PlayerShooting>();

            if (playerShooting != null)
            {
                // Give the player the ability to shoot
                playerShooting.EnableShooting(bulletPrefab);

                // Optionally, hide the power-up and destroy it after a delay
                GetComponent<Renderer>().enabled = false; // Hide the power-up visually
                GetComponent<Collider>().enabled = false; // Disable the collider
                Destroy(gameObject, 1f); // Destroy the power-up after 1 second
            }
        }
    }
}
