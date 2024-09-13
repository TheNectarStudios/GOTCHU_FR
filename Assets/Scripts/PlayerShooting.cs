using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public Transform firePoint; // The point from where the bullets are fired
    public float bulletSpeed = 10f; // Speed of the bullet
    private bool canShoot = false; // Whether the player can shoot or not
    private GameObject bulletPrefab; // The bullet prefab

    void Update()
    {
        if (canShoot && Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }

    public void EnableShooting(GameObject bullet)
    {
        canShoot = true;
        bulletPrefab = bullet;
    }

    void Shoot()
    {
        if (firePoint != null && bulletPrefab != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.velocity = firePoint.forward * bulletSpeed;
            }
        }
    }
}
