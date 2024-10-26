using UnityEngine;
using System.Collections;
using TMPro;

public class GhostHitManagerOffline : MonoBehaviour
{
    public GameObject pearlPrefab; // Reference to the pearl prefab
    private GameObject placedPearl; // Reference to the pearl placed by the ghost
    private GameObject naturalSpawnPoint; // Reference to the natural spawn point in the scene
    public string spawnPointTag = "GhostSpawn"; // Tag to identify spawn points
    public float respawnDelay = 3f; // Delay in seconds before the ghost is visible and movable again
    public float scaleTransitionDuration = 1f; // Time it takes to scale down/up the object
    public float preTeleportBuffer = 0.5f; // Buffer time before the ghost teleports after shrinking

    private Vector3 lastPosition; // Position to teleport the ghost to
    public GameObject objectToDisable; // Object to disable (assigned in the editor)
    public TextMeshProUGUI textToDisable; // TextMeshProUGUI to disable when hit
    private Rigidbody ghostRigidbody; // Reference to the Rigidbody to control movement
    private MonoBehaviour ghostMovementScript; // Reference to any movement script (if applicable)

    public GameObject hitAnimationPrefab; // Reference to the animation prefab to be played when hit by a bullet

    private Vector3 originalScale; // Store the original scale of the object

    private TopDownFollowCameraOffline cameraFollow; // Reference to the camera follow script

    private void Start()
    {
        naturalSpawnPoint = GameObject.FindWithTag(spawnPointTag);

        if (naturalSpawnPoint == null)
        {
            Debug.LogError("Natural spawn point not found! Make sure it's tagged correctly.");
        }

        ghostRigidbody = GetComponent<Rigidbody>();

        if (ghostRigidbody == null)
        {
            Debug.LogError("No Rigidbody found on the ghost!");
        }

        ghostMovementScript = GetComponent<MonoBehaviour>(); // Replace with your actual movement script

        if (objectToDisable != null)
        {
            originalScale = objectToDisable.transform.localScale;
        }

        cameraFollow = FindObjectOfType<TopDownFollowCameraOffline>();
        if (cameraFollow == null)
        {
            Debug.LogError("No TopDownFollowCameraOffline script found on the camera!");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("H key pressed! Triggering camera shake.");

            if (cameraFollow != null)
            {
                cameraFollow.ShakeCamera();
            }
        }
    }

    public void TeleportToSpawnPoint()
    {
        GameObject pearlInScene = GameObject.FindWithTag("Pearl");

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
            lastPosition = transform.position; 
        }

        StartCoroutine(ShrinkThenTeleport());
    }

    private IEnumerator ShrinkThenTeleport()
    {
        yield return StartCoroutine(ScaleObject(Vector3.zero, scaleTransitionDuration));

        yield return new WaitForSeconds(preTeleportBuffer);

        TeleportGhost(lastPosition);
    }

    private void TeleportGhost(Vector3 teleportPosition)
    {
        Debug.Log("Teleporting ghost to position: " + teleportPosition);
        transform.position = teleportPosition;

        StartCoroutine(ScaleAndDisable(respawnDelay));
    }

    private IEnumerator ScaleAndDisable(float delay)
    {
        if (objectToDisable != null)
        {
            objectToDisable.SetActive(false);
            Debug.Log("Object is invisible for " + delay + " seconds.");
        }

        if (textToDisable != null)
        {
            textToDisable.gameObject.SetActive(false);
            Debug.Log("TextMeshProUGUI is disabled.");
        }

        if (ghostRigidbody != null)
        {
            ghostRigidbody.isKinematic = true; 
            Debug.Log("Ghost movement is disabled.");
        }

        if (ghostMovementScript != null)
        {
            ghostMovementScript.enabled = false; 
            Debug.Log("Ghost movement script is disabled.");
        }

        yield return new WaitForSeconds(delay);

        if (objectToDisable != null)
        {
            objectToDisable.SetActive(true);
            Debug.Log("Object is visible again.");
        }

        if (textToDisable != null)
        {
            textToDisable.gameObject.SetActive(true);
            Debug.Log("TextMeshProUGUI is re-enabled.");
        }

        yield return StartCoroutine(ScaleObject(originalScale, scaleTransitionDuration));

        if (ghostRigidbody != null)
        {
            ghostRigidbody.isKinematic = false;
            Debug.Log("Ghost movement is re-enabled.");
        }

        if (ghostMovementScript != null)
        {
            ghostMovementScript.enabled = true;
            Debug.Log("Ghost movement script is re-enabled.");
        }
    }

    private IEnumerator ScaleObject(Vector3 targetScale, float duration)
    {
        Vector3 initialScale = objectToDisable.transform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            objectToDisable.transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        objectToDisable.transform.localScale = targetScale;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Debug.Log("Ghost hit by bullet!");

            if (cameraFollow != null)
            {
                cameraFollow.ShakeCamera();
                Debug.Log("Camera shake triggered!");
            }
            else
            {
                Debug.LogError("Camera follow script is missing!");
            }

            // Play hit animation
            if (hitAnimationPrefab != null)
            {
                Instantiate(hitAnimationPrefab, transform.position, Quaternion.identity);
                Debug.Log("Hit animation played!");
            }
            else
            {
                Debug.LogError("Hit animation prefab is not assigned!");
            }

            TeleportToSpawnPoint();
        }
        else
        {
            Debug.Log("Collision with non-bullet object: " + collision.gameObject.name);
        }
    }

    public void PlacePearl(Vector3 position)
    {
        if (placedPearl == null)
        {
            placedPearl = Instantiate(pearlPrefab, position, Quaternion.identity);
            Debug.Log("Pearl placed at: " + position);
        }
    }

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
