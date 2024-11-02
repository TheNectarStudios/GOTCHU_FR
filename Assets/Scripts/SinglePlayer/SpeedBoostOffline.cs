using UnityEngine;

public class SpeedBoostOffline : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SpawnManager spawnManager = FindObjectOfType<SpawnManager>();
            if (spawnManager != null)
            {
                spawnManager.UpdateInventory("SpeedBoost");
                // Destroy(gameObject);
            }
        }
    }
}
