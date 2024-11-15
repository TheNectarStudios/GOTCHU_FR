using UnityEngine;

public class PlayerShootingOffline : MonoBehaviour
{
    public GameObject bulletPrefab; // Bullet prefab that the player will shoot
    public Transform firePoint;     // The point from which the bullet will be fired
    public float bulletForce = 20f; // The speed of the bullet

    private bool canShoot = false;  // Controls if the player can shoot
    private int bulletCount = 0;    // Current number of bullets available
    private const int maxBullets = 3; // Maximum number of bullets allowed
    private PacMan3DMovement pacManMovement; // Reference to the movement script

    void Start()
    {
        // Find the PacMan3DMovement script attached to the player
        pacManMovement = GetComponent<PacMan3DMovement>();
    }

    // Call this method when the player picks up the shooting power-up
    public void EnableShooting(GameObject bullet)
    {
        // Only increase the bullet count if it's less than the maximum allowed
        if (bulletCount < maxBullets)
        {
            bulletCount = maxBullets; // Refill to the max number of bullets
            bulletPrefab = bullet;    // Assign the bullet prefab
            canShoot = true;          // Enable shooting
        }
    }

    void Update()
    {
        // Check if the player is allowed to shoot, has bullets, and presses the spacebar
        if (canShoot && bulletCount > 0 && Input.GetKeyDown(KeyCode.Space))
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
        // Log the direction for debugging
        Debug.Log("Shooting in direction: " + direction);

        // Instantiate the bullet locally
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // Apply velocity to the bullet in the direction the player is moving
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = direction * bulletForce; // Apply the correct movement direction and force to the bullet

            // Log the velocity applied to the bullet for debugging
            Debug.Log("Bullet velocity: " + rb.velocity);
        }

        // Decrease the bullet count
        bulletCount--;

        // Disable shooting if no bullets are left
        if (bulletCount <= 0)
        {
            canShoot = false;
        }
    }
}
