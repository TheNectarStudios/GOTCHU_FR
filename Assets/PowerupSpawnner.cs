using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;

public class PowerUpSpawner : MonoBehaviourPunCallbacks
{
    public List<Transform> spawnPoints; // List of potential spawn points
    public List<GameObject> powerUpPrefabs; // List of power-up prefabs
    public int maxPowerUps = 3; // Maximum number of power-ups to be active at a time
    public float respawnDelay = 5.0f; // Delay before a power-up respawns

    private List<GameObject> activePowerUps = new List<GameObject>(); // List of currently active power-ups

    void Start()
    {
        // Only the MasterClient is responsible for spawning power-ups
        if (PhotonNetwork.IsMasterClient)
        {
            // Start spawning power-ups at regular intervals
            InvokeRepeating("TrySpawnPowerUp", 2.0f, 1.0f);
        }
    }

    void TrySpawnPowerUp()
    {
        if (PhotonNetwork.IsMasterClient && activePowerUps.Count < maxPowerUps)
        {
            SpawnPowerUp();
        }
    }

    void SpawnPowerUp()
    {
        if (spawnPoints.Count == 0 || powerUpPrefabs.Count == 0)
        {
            Debug.LogError("No spawn points or power-up prefabs set.");
            return;
        }

        // Pick a random spawn point
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

        // Pick a random power-up prefab
        GameObject selectedPowerUp = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Count)];

        // Spawn the power-up at the selected spawn point (only the MasterClient can spawn)
        GameObject newPowerUp = PhotonNetwork.Instantiate(selectedPowerUp.name, spawnPoint.position, Quaternion.identity);
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
        activePowerUps.Remove(powerUp);
        Destroy(powerUp);
    }

    // MasterClient changed (in case of disconnection)
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        // New MasterClient will take over the power-up spawning
        if (PhotonNetwork.IsMasterClient)
        {
            InvokeRepeating("TrySpawnPowerUp", 2.0f, 1.0f);
        }
    }
}
