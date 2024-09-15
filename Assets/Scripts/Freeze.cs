using UnityEngine;
using Photon.Pun;

public class Freeze : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine)
        {
            // Get the SpawnManager and update the inventory
            SpawnManager spawnManager = FindObjectOfType<SpawnManager>();
            if (spawnManager != null)
            {
                spawnManager.UpdateInventory("Freeze");
                Destroy(gameObject);  // Destroy the power-up object after collection
            }
        }
    }
}
