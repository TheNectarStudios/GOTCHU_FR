using UnityEngine;
using Photon.Pun;
using System.Collections;

public class GhostHitManager : MonoBehaviourPun
{
    public GameObject pearlPrefab;          // Reference to the pearl prefab
    private GameObject placedPearl;         // Reference to the pearl placed by the ghost
    private GameObject naturalSpawnPoint;   // Reference to the natural spawn point in the scene
    public string spawnPointTag = "GhostSpawn"; // Tag to identify spawn points
    public float respawnDelay = 3f;         // Delay in seconds before the ghost is visible and movable again
    public float scaleTransitionDuration = 1f; // Time it takes to scale down/up the object
    public float preTeleportBuffer = 0.5f;  // Buffer time before the ghost teleports after shrinking

    private Vector3 lastPosition;           // Position to teleport the ghost to
    public GameObject objectToDisable;      // Object to disable (assigned in the editor)
    private Rigidbody ghostRigidbody;       // Reference to the Rigidbody to control movement
    private MonoBehaviour ghostMovementScript; // Reference to any movement script (if applicable)

    private Vector3 originalScale;          // Store the original scale of the object

    private TopDownCameraFollow cameraFollow; // Reference to the camera follow script

    private void Start()
    {
        // Automatically find the natural spawn point by tag at the start
        naturalSpawnPoint = GameObject.FindWithTag(spawnPointTag);

        if (naturalSpawnPoint == null)
        {
            Debug.LogError("Natural spawn point not found! Make sure it's tagged correctly.");
        }

        // Get the Rigidbody component to freeze/unfreeze the ghost
        ghostRigidbody = GetComponent<Rigidbody>();

        if (ghostRigidbody == null)
        {
            Debug.LogError("No Rigidbody found on the ghost!");
        }

        // Get the movement script (optional, depends on your setup)
        ghostMovementScript = GetComponent<MonoBehaviour>(); // Replace with your actual movement script

        // Store the original scale of the object
        if (objectToDisable != null)
        {
            originalScale = objectToDisable.transform.localScale;
        }

        // Find the TopDownCameraFollow script on the camera
        cameraFollow = FindObjectOfType<TopDownCameraFollow>();
        if (cameraFollow == null)
        {
            Debug.LogError("No TopDownCameraFollow script found on the camera!");
        }
    }

    private void Update()
    {
        // Check if the player presses the 'H' key
        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("H key pressed! Triggering camera shake.");

            // Trigger the camera shake effect
            if (cameraFollow != null)
            {
                cameraFollow.ShakeCamera();
            }
        }
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

        // Start the shrinking, teleporting, and scaling process
        StartCoroutine(ShrinkThenTeleport());
    }

    private IEnumerator ShrinkThenTeleport()
    {
        // Step 1: Shrink the object to zero
        yield return StartCoroutine(ScaleObject(Vector3.zero, scaleTransitionDuration));

        // Step 2: Wait for the pre-teleport buffer time before teleporting
        yield return new WaitForSeconds(preTeleportBuffer);

        // Step 3: Call the RPC to teleport the ghost across the network
        photonView.RPC("TeleportGhostRPC", RpcTarget.All, lastPosition);
    }

    [PunRPC]
    public void TeleportGhostRPC(Vector3 teleportPosition)
    {
        Debug.Log("Teleporting ghost to position: " + teleportPosition);
        transform.position = teleportPosition; // Move the ghost to the teleport position

        // Step 4: Begin the process of scaling back up and re-enabling after the delay
        StartCoroutine(ScaleAndDisable(respawnDelay));
    }

    // Coroutine to scale down, disable the object, then scale up and re-enable it
    private IEnumerator ScaleAndDisable(float delay)
    {
        // Disable the assigned object in the editor (e.g., ghost model)
        if (objectToDisable != null)
        {
            objectToDisable.SetActive(false);
            Debug.Log("Object is invisible for " + delay + " seconds.");
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

        // Wait for the specified delay before reappearing
        yield return new WaitForSeconds(delay);

        // Re-enable the object and scale it back to its original size
        if (objectToDisable != null)
        {
            objectToDisable.SetActive(true);
            Debug.Log("Object is visible again.");
        }

        // Scale up the object back to its original size
        yield return StartCoroutine(ScaleObject(originalScale, scaleTransitionDuration));

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

    // Coroutine to smoothly scale the object
    private IEnumerator ScaleObject(Vector3 targetScale, float duration)
    {
        Vector3 initialScale = objectToDisable.transform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Lerp between the initial and target scale
            objectToDisable.transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final scale is set exactly
        objectToDisable.transform.localScale = targetScale;
    }

    // Bullet collision handler to trigger the camera shake
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet")) // Check if the collision is with a bullet
        {
            Debug.Log("Ghost hit by bullet!");

            // Trigger camera shake when hit by a bullet
            if (cameraFollow != null)
            {
                cameraFollow.ShakeCamera(); // Trigger camera shake
            }

            // Additional logic for when the ghost is hit by a bullet
            TeleportToSpawnPoint();
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
