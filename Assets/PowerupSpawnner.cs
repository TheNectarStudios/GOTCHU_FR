using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; // Photon support

public class PowerupSpawner : MonoBehaviourPun
{
    public List<Transform> spawnPoints;
    public List<GameObject> powerUpPrefabs; // List of different power-up prefabs
    public int maxPowerUps = 3;
    private List<GameObject> activePowerUps = new List<GameObject>();

    private void Start()
    {
        PowerUp.onPickedUp += HandlePowerUpPickedUp; // Subscribe to event
        StartCoroutine(SpawnPowerUpsRoutine());
    }

    private void OnDestroy()
    {
        PowerUp.onPickedUp -= HandlePowerUpPickedUp; // Unsubscribe to event to prevent memory leaks
    }

    private void HandlePowerUpPickedUp(PowerUp powerUp)
    {
        activePowerUps.Remove(powerUp.gameObject); // Remove the picked-up power-up from the list
        StartCoroutine(RespawnPowerUp()); // Optionally respawn a new power-up after a delay
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
        GameObject powerUp = PhotonNetwork.Instantiate(powerUpPrefabs[powerUpIndex].name, spawnPoint.position, Quaternion.identity);

        activePowerUps.Add(powerUp); // Add the new power-up to the list of active ones
    }

    private IEnumerator RespawnPowerUp()
    {
        yield return new WaitForSeconds(3f); // Respawn delay
        if (activePowerUps.Count < maxPowerUps)
        {
            SpawnRandomPowerUp();
        }
    }
}
