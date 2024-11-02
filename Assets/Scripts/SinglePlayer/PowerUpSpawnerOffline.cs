using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpawnerOffline : MonoBehaviour
{
    public List<Transform> spawnPoints; // Spawn points for power-ups
    public List<GameObject> powerUpPrefabs; // List of power-up prefabs
    public int maxPowerUps = 3;
    private List<GameObject> activePowerUps = new List<GameObject>();

    private void Start()
    {
        PowerUpOffline.onPickedUp += HandlePowerUpPickedUp; // Subscribe to event
        StartCoroutine(SpawnPowerUpsRoutine());
    }

    private void OnDestroy()
    {
        PowerUpOffline.onPickedUp -= HandlePowerUpPickedUp; // Unsubscribe on destroy
    }

    private IEnumerator SpawnPowerUpsRoutine()
    {
        while (true)
        {
            if (activePowerUps.Count < maxPowerUps)
            {
                SpawnRandomPowerUp();
            }
            yield return new WaitForSeconds(5f); // Wait for 5 seconds before checking again
        }
    }

    private void SpawnRandomPowerUp()
    {
        int spawnIndex = Random.Range(0, spawnPoints.Count);
        int powerUpIndex = Random.Range(0, powerUpPrefabs.Count);

        Transform spawnPoint = spawnPoints[spawnIndex];
        GameObject powerUp = Instantiate(powerUpPrefabs[powerUpIndex], spawnPoint.position, Quaternion.identity);

        activePowerUps.Add(powerUp);
    }

    private void HandlePowerUpPickedUp(string powerUpName, PowerUpOffline powerUp)
    {
        activePowerUps.Remove(powerUp.gameObject);

        // Send the power-up name to SpawnManagerOffline to update the UI
        SpawnManagerOffline spawnManager = FindObjectOfType<SpawnManagerOffline>();
        if (spawnManager != null)
        {
            spawnManager.UpdateInventory(powerUpName);
        }
        else
        {
            Debug.LogWarning("SpawnManagerOffline not found in the scene.");
        }

        Destroy(powerUp.gameObject);
    }
}
