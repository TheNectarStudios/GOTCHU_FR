using UnityEngine;
using Photon.Pun;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab; // Bullet prefab that the player will shoot
    public Transform firePoint;     // The point from which the bullet will be fired
    public float bulletForce = 20f; // The speed of the bullet

    private bool canShoot = false;  // Controls if the player can shoot
    private Vector3 lastPosition;   // To track player movement
    private bool isMoving = false;  // To determine if the player is moving
    private Vector3 moveDirection;  // Stores the direction in which the player is moving

    // Call this method when the player picks up the shooting power-up
    public void EnableShooting(GameObject bullet)
    {
        canShoot = true;            // Enable shooting
        bulletPrefab = bullet;      // Assign the bullet prefab
    }

    void Update()
    {
        // Calculate the player's movement direction
        moveDirection = transform.position - lastPosition;

        // Check if the player is moving
        if (moveDirection.magnitude > 0.01f)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        lastPosition = transform.position;

        // Check if the player is allowed to shoot, is moving, and presses the spacebar
        if (canShoot && isMoving && Input.GetKeyDown(KeyCode.Space))
        {
            Shoot(moveDirection.normalized); // Pass the direction of movement to Shoot()
        }
    }

    void Shoot(Vector3 direction)
    {
        // Ensure the bullet is instantiated across the network
        if (PhotonNetwork.IsConnected && canShoot)
        {
            // Use PhotonNetwork.Instantiate to instantiate the bullet across the network
            GameObject bullet = PhotonNetwork.Instantiate(bulletPrefab.name, firePoint.position, firePoint.rotation);
            
            // Apply velocity to the bullet in the direction the player was moving
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = direction * bulletForce; // Use direction of player movement
            }

            // Disable shooting after the bullet is fired
            canShoot = false;
        }
        else
        {
            Debug.LogError("Not connected to Photon or shooting is disabled!");
        }
    }
}
