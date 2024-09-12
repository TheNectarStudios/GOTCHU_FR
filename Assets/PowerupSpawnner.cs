using UnityEngine;
using System.Collections.Generic;

public class PowerUpSpawner : MonoBehaviour
{
    public List<Transform> spawnPoints; // List of potential spawn points
    public List<GameObject> powerUpPrefabs; // List of power-up prefabs
    public int maxPowerUps = 3; // Maximum number of power-ups to be active at a time
    public float respawnDelay = 5.0f; // Delay before a power-up respawns

    private List<GameObject> activePowerUps = new List<GameObject>(); // List of currently active power-ups

    void Start()
    {
        // Start spawning power-ups at regular intervals
        InvokeRepeating("TrySpawnPowerUp", 2.0f, 1.0f); // Check every second if a new power-up should spawn
    }

    void TrySpawnPowerUp()
    {
        // Check if the current number of power-ups is less than the maximum allowed
        if (activePowerUps.Count < maxPowerUps)
        {
            SpawnPowerUp();
        }
    }

    void SpawnPowerUp()
    {
        // Pick a random spawn point
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

        // Pick a random power-up prefab
        GameObject selectedPowerUp = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Count)];

        // Spawn the power-up at the selected spawn point
        GameObject newPowerUp = Instantiate(selectedPowerUp, spawnPoint.position, Quaternion.identity);
        activePowerUps.Add(newPowerUp);

        // Ensure that the power-up is removed from the list once it is picked up
        PowerUp powerUpComponent = newPowerUp.GetComponent<PowerUp>();
        if (powerUpComponent != null)
        {
            powerUpComponent.onPickedUp += () => HandlePowerUpPickedUp(newPowerUp);
        }
    }

    void HandlePowerUpPickedUp(GameObject powerUp)
    {
        // Remove the power-up from the active list
        activePowerUps.Remove(powerUp);
        // Optionally, add a delay before allowing respawn
        Destroy(powerUp);
    }
}
