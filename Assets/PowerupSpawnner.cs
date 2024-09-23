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
        // Transfer ownership before destroying the power-up
        PhotonView powerUpPhotonView = powerUp.GetComponent<PhotonView>();
        if (powerUpPhotonView != null && !powerUpPhotonView.IsMine)
        {
            // Request ownership and wait until it is granted before destroying
            StartCoroutine(TransferOwnershipAndDestroy(powerUpPhotonView, powerUp.gameObject));
        }
        else
        {
            DestroyPowerUp(powerUp.gameObject);
        }
    }

    private IEnumerator TransferOwnershipAndDestroy(PhotonView photonView, GameObject powerUp)
    {
        Debug.Log("Requesting ownership transfer for power-up.");

        // Request ownership transfer
        photonView.RequestOwnership();

        // Wait for the ownership to transfer
        while (!photonView.IsMine)
        {
            yield return null; // Wait for the next frame
        }

        Debug.Log("Ownership successfully transferred. Now destroying the power-up.");
        // Once ownership is ours, destroy the power-up
        DestroyPowerUp(powerUp);
    }

    private void DestroyPowerUp(GameObject powerUp)
    {
        // Remove the power-up from the active list
        activePowerUps.Remove(powerUp);

        // Destroy across the network only if the player is the owner or MasterClient
        if (PhotonNetwork.IsMasterClient && powerUp.GetComponent<PhotonView>().IsMine)
        {
            Debug.Log("Destroying power-up across the network.");
            PhotonNetwork.Destroy(powerUp); // Ensures it's destroyed across the network
        }
        else
        {
            Debug.LogError("Cannot destroy power-up. Not the owner or not the MasterClient.");
        }
    }

    private IEnumerator SpawnPowerUpsRoutine()
    {
        while (true)
        {
            if (PhotonNetwork.IsMasterClient && activePowerUps.Count < maxPowerUps)
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
}
