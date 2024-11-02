using UnityEngine;

public class FreezeOffline : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Get the SpawnManager and update the inventory
            SpawnManager spawnManager = FindObjectOfType<SpawnManager>();
            if (spawnManager != null)
            {
                spawnManager.UpdateInventory("Freeze");
                // Destroy(gameObject); // Uncomment this line to destroy the power-up object after collection
            }
        }
    }
}
