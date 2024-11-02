using UnityEngine;

public class ReverseControlsOffline : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SpawnManager spawnManager = FindObjectOfType<SpawnManager>();
            if (spawnManager != null)
            {
                spawnManager.UpdateInventory("Bullet");
                // Destroy(gameObject);
            }
        }
    }
}
