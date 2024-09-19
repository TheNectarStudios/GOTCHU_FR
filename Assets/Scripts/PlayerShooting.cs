using UnityEngine;
using Photon.Pun;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab; // Bullet prefab that the player will shoot
    public Transform firePoint;     // The point from which the bullet will be fired
    public float bulletForce = 20f; // The speed of the bullet

    private bool canShoot = false;  // Controls if the player can shoot
    private PacMan3DMovement pacManMovement; // Reference to the movement script

    void Start()
    {
        // Find the PacMan3DMovement script attached to the player
        pacManMovement = GetComponent<PacMan3DMovement>();
    }

    // Call this method when the player picks up the shooting power-up
    public void EnableShooting(GameObject bullet)
    {
        canShoot = true;            // Enable shooting
        bulletPrefab = bullet;      // Assign the bullet prefab
    }

    void Update()
    {
        // Check if the player is allowed to shoot, is moving, and presses the spacebar
        if (canShoot && Input.GetKeyDown(KeyCode.Space))
        {
            // Get the last movement direction from the PacMan3DMovement script
            Vector3 shootingDirection = pacManMovement.GetLastMovementDirection().normalized;

            // Shoot in the direction the player is moving
            Shoot(shootingDirection);
        }
    }

    // Method to handle shooting
    void Shoot(Vector3 direction)
    {
        // Ensure the bullet is instantiated across the network
        if (PhotonNetwork.IsConnected && canShoot)
        {
            // Log the direction for debugging
            Debug.Log("Shooting in direction: " + direction);

            // Use PhotonNetwork.Instantiate to instantiate the bullet across the network
            GameObject bullet = PhotonNetwork.Instantiate(bulletPrefab.name, firePoint.position, Quaternion.identity);

            // Apply velocity to the bullet in the direction the player is moving
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = direction * bulletForce; // Apply the correct movement direction and force to the bullet

                // Log the velocity applied to the bullet for debugging
                Debug.Log("Bullet velocity: " + rb.velocity);
            }

            // Disable shooting after the bullet is fired (for single-use shooting mechanic)
            canShoot = false;
        }
        else
        {
            Debug.LogError("Not connected to Photon or shooting is disabled!");
        }
    }

}
