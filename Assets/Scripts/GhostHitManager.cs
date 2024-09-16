using UnityEngine;
using Photon.Pun;
using System.Collections;

public class GhostHitManager : MonoBehaviourPun
{
    public GameObject pearlPrefab;       // Reference to the pearl prefab
    private GameObject placedPearl;      // Reference to the pearl placed by the ghost
    private GameObject naturalSpawnPoint; // Reference to the natural spawn point in the scene
    public string spawnPointTag = "GhostSpawn"; // Tag to identify spawn points
    public float respawnDelay = 3f; // Delay in seconds before the ghost is visible and movable again

    private Vector3 lastPosition; // Position to teleport the ghost to
    private MeshRenderer ghostMeshRenderer; // Reference to the MeshRenderer of the ghost
    private Rigidbody ghostRigidbody; // Reference to the Rigidbody to control movement
    private MonoBehaviour ghostMovementScript; // Reference to any movement script (if applicable)

    private void Start()
    {
        // Automatically find the natural spawn point by tag at the start
        naturalSpawnPoint = GameObject.FindWithTag(spawnPointTag);

        if (naturalSpawnPoint == null)
        {
            Debug.LogError("Natural spawn point not found! Make sure it's tagged correctly.");
        }

        // Get the MeshRenderer component of the ghost to enable/disable visibility
        ghostMeshRenderer = GetComponent<MeshRenderer>();

        if (ghostMeshRenderer == null)
        {
            Debug.LogError("No MeshRenderer found on the ghost!");
        }

        // Get the Rigidbody component to freeze/unfreeze the ghost
        ghostRigidbody = GetComponent<Rigidbody>();

        if (ghostRigidbody == null)
        {
            Debug.LogError("No Rigidbody found on the ghost!");
        }

        // Get the movement script (optional, depends on your setup)
        ghostMovementScript = GetComponent<MonoBehaviour>(); // Replace with your actual movement script
    }

    public void TeleportToSpawnPoint()
    {
        // Try to find an active GameObject with the tag "Pearl"
        GameObject pearlInScene = GameObject.FindWithTag("Pearl");

        // Check if the pearl exists and hasn't been destroyed
        if (pearlInScene != null)
        {
            lastPosition = pearlInScene.transform.position;
            Debug.Log($"Pearl found in the scene at position {lastPosition}. Teleporting ghost to the pearl.");
        }
        else if (naturalSpawnPoint != null)
        {
            lastPosition = naturalSpawnPoint.transform.position;
            Debug.Log("Pearl not found. Teleporting ghost to the natural spawn point.");
        }
        else
        {
            Debug.LogError("Neither pearl nor natural spawn point found! Teleporting to current position as a fallback.");
            lastPosition = transform.position; // Fallback to current position
        }

        // Call the RPC to teleport the ghost across the network
        photonView.RPC("TeleportGhostRPC", RpcTarget.All, lastPosition);
    }



    [PunRPC]
    public void TeleportGhostRPC(Vector3 teleportPosition)
    {
        Debug.Log("Teleporting ghost to position: " + teleportPosition);
        transform.position = teleportPosition; // Move the ghost to the teleport position

        // Disable the MeshRenderer and movement, then re-enable them after the delay
        StartCoroutine(DisableMeshAndMovement(respawnDelay));
    }

    // Coroutine to disable the MeshRenderer and movement, then re-enable them after a delay
    private IEnumerator DisableMeshAndMovement(float delay)
    {
        // Disable the ghost's MeshRenderer to make it invisible
        if (ghostMeshRenderer != null)
        {
            ghostMeshRenderer.enabled = false;
            Debug.Log("Ghost is invisible for " + delay + " seconds.");
        }

        // Disable the ghost's Rigidbody and/or movement script to prevent movement
        if (ghostRigidbody != null)
        {
            ghostRigidbody.isKinematic = true; // Makes the Rigidbody static
            Debug.Log("Ghost movement is disabled.");
        }

        if (ghostMovementScript != null)
        {
            ghostMovementScript.enabled = false; // Disable the movement script
            Debug.Log("Ghost movement script is disabled.");
        }

        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Re-enable the ghost's MeshRenderer to make it visible again
        if (ghostMeshRenderer != null)
        {
            ghostMeshRenderer.enabled = true;
            Debug.Log("Ghost is visible again.");
        }

        // Re-enable movement by unfreezing the Rigidbody and movement script
        if (ghostRigidbody != null)
        {
            ghostRigidbody.isKinematic = false; // Allows the Rigidbody to move again
            Debug.Log("Ghost movement is re-enabled.");
        }

        if (ghostMovementScript != null)
        {
            ghostMovementScript.enabled = true; // Re-enable the movement script
            Debug.Log("Ghost movement script is re-enabled.");
        }
    }

    // Optional: Call this method when placing a pearl
    public void PlacePearl(Vector3 position)
    {
        if (placedPearl == null)
        {
            placedPearl = Instantiate(pearlPrefab, position, Quaternion.identity);
            Debug.Log("Pearl placed at: " + position);
        }
    }

    // Optional: Call this method to clear the pearl when collected
    public void DestroyPearl()
    {
        if (placedPearl != null)
        {
            Destroy(placedPearl);
            placedPearl = null;
            Debug.Log("Pearl destroyed.");
        }
    }
}
