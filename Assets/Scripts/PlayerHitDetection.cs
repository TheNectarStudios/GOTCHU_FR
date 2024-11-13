using UnityEngine;
using System.Collections;
using Photon.Pun;

public class PlayerHitDetection : MonoBehaviourPunCallbacks
{
    public GameObject targetPrefab;          // Assignable prefab that will be disabled/enabled
    public float blinkDuration = 2f;         // Total time for blinking
    public int blinkCount = 3;               // Number of times the object blinks
    private bool isBlinking = false;         // Boolean to prevent multiple hit registrations during blinking
    private int hitCounter = 0;              // Player's hit counter
    public int maxHits = 3;                  // Maximum hits allowed before taking action

    private TopDownCameraFollow cameraFollow; // Reference to the camera follow script

    private void Start()
    {
        // Ensure that the target prefab is assigned
        if (targetPrefab == null)
        {
            Debug.LogError("Target prefab is not assigned! Please assign a prefab to disable/enable.");
        }

        // Find the TopDownCameraFollow script
        cameraFollow = FindObjectOfType<TopDownCameraFollow>();
    }

    private void Update()
    {
        // Trigger camera shake when the G key is pressed using the camera's own shake settings
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (cameraFollow != null)
            {
                cameraFollow.ShakeCamera(cameraFollow.shakeDuration, cameraFollow.shakeMagnitude); // Use camera's variables
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the object that collided with the player is tagged as "Ghost"
        if (collision.gameObject.CompareTag("Ghost") && !isBlinking)
        {
            Debug.Log("Player hit by the ghost!");

            // Trigger the camera shake using the camera script's variables
            if (cameraFollow != null)
            {
                cameraFollow.ShakeCamera(cameraFollow.shakeDuration, cameraFollow.shakeMagnitude); // Use camera's variables
            }

            // Increment the hit counter and start the blink effect
            hitCounter++;
            Debug.Log("Hit counter: " + hitCounter);

            // Start the blink effect and set isBlinking to true to prevent multiple hits
            StartCoroutine(BlinkTargetPrefab());

            // Check if the player has reached the maximum allowed hits
            if (hitCounter >= maxHits)
            {
                Debug.Log("Player reached max hits. Ghosts win.");

                // Store a message indicating the Ghosts have won by eliminating the protagonist
                PlayerPrefs.SetString("GameResult", "Ghosts won by eliminating the protagonist!");

                // Use the DecisionMaker script to change the scene
                DecisionMaker decisionMaker = FindObjectOfType<DecisionMaker>();
                if (decisionMaker != null)
                {
                    decisionMaker.OnPlayerEliminated(); // Call a new method in DecisionMaker to handle scene change
                }
                else
                {
                    Debug.LogError("DecisionMaker script not found in the scene.");
                }
            }
        }
    }

    private IEnumerator BlinkTargetPrefab()
    {
        if (targetPrefab == null) yield break; // Exit if no target is assigned

        isBlinking = true; // Prevent additional hit counts during blinking
        float blinkInterval = blinkDuration / (blinkCount * 2); // Time between on and off states

        for (int i = 0; i < blinkCount; i++)
        {
            // Disable the assigned prefab (make it invisible)
            targetPrefab.SetActive(false);
            yield return new WaitForSeconds(blinkInterval);

            // Enable the assigned prefab (make it visible again)
            targetPrefab.SetActive(true);
            yield return new WaitForSeconds(blinkInterval);
        }

        // Ensure the prefab is enabled after blinking
        targetPrefab.SetActive(true);

        // Reset the blinking flag after the blinking period is over
        isBlinking = false;
    }
}
