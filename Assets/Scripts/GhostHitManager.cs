using UnityEngine;
using Photon.Pun;
using System.Collections;

public class GhostHitManager : MonoBehaviourPun
{
    public GameObject pearlPrefab;       // Reference to the pearl prefab
    private GameObject placedPearl;      // Reference to the pearl placed by the ghost
    private GameObject naturalSpawnPoint; // Reference to the natural spawn point in the scene
    public float respawnTime = 5f;       // Time between death and respawn
    public string spawnPointTag = "GhostSpawn"; // Tag to identify spawn points

    private Vector3 lastPosition;        // Last position where the pearl was placed

    private void Start()
    {
        if (photonView.IsMine) // Ensure this runs only on the owner’s side
        {
            // Automatically find the natural spawn point by tag at the start
            naturalSpawnPoint = GameObject.FindWithTag(spawnPointTag);

            if (naturalSpawnPoint == null)
            {
                Debug.LogError("Natural spawn point not found! Make sure it's tagged correctly.");
            }
        }
    }

    // Detect collision with bullet (without setting IsTrigger)
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the ghost has been hit by a bullet
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Debug.Log("Ghost hit by bullet!");

            // Ensure only the owner handles the hit
            if (photonView.IsMine)
            {
                // Call the method to handle respawning after hit
                RespawnAfterHit();
            }

            // Destroy the bullet across the network
            PhotonNetwork.Destroy(collision.gameObject);
        }
    }

    public void RespawnAfterHit()
    {
        // Start coroutine to respawn the ghost after a delay
        StartCoroutine(RespawnAfterDeath());
    }

    private IEnumerator RespawnAfterDeath()
    {
        Debug.Log("Ghost will respawn after a delay.");

        // Disable the ghost temporarily to simulate its death
        photonView.RPC("DisableGhost", RpcTarget.All);

        // Wait for the respawn delay
        yield return new WaitForSeconds(respawnTime);

        // Determine the respawn point based on whether the pearl was placed or not
        if (placedPearl != null)
        {
            lastPosition = placedPearl.transform.position;
            Debug.Log("Respawning at placed pearl position.");
        }
        else if (naturalSpawnPoint != null)
        {
            lastPosition = naturalSpawnPoint.transform.position;
            Debug.Log("Respawning at natural spawn position.");
        }
        else
        {
            Debug.LogError("No valid spawn point found!");
        }

        // Respawn the ghost at the selected position
        photonView.RPC("RespawnGhost", RpcTarget.All, lastPosition);
    }

    [PunRPC]
    private void DisableGhost()
    {
        Debug.Log("Disabling ghost.");
        gameObject.SetActive(false); // Deactivate the ghost (make it disappear)
    }

    [PunRPC]
    private void RespawnGhost(Vector3 respawnPosition)
    {
        Debug.Log("Respawning ghost at position: " + respawnPosition);
        transform.position = respawnPosition; // Move the ghost to the respawn position
        gameObject.SetActive(true); // Reactivate the ghost (make it reappear)
    }
}
