using UnityEngine;
using System.Collections;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class PlayerHitDetection : MonoBehaviourPunCallbacks
{
    public GameObject targetPrefab;          // Assignable prefab that will be disabled/enabled
    public float blinkDuration = 2f;         // Total time for blinking
    public int blinkCount = 3;               // Number of times the object blinks
    private bool isBlinking = false;         // Boolean to prevent multiple hit registrations during blinking
    private int hitCounter = 0;              // Player's hit counter
    public int maxHits = 3;                  // Maximum hits allowed before returning to the RoomCreated scene

    private void Start()
    {
        // Ensure that the target prefab is assigned
        if (targetPrefab == null)
        {
            Debug.LogError("Target prefab is not assigned! Please assign a prefab to disable/enable.");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the object that collided with the player is tagged as "Ghost"
        if (collision.gameObject.CompareTag("Ghost") && !isBlinking)
        {
            Debug.Log("Player hit by the ghost!");

            // Increment the hit counter and start the blink effect
            hitCounter++;
            Debug.Log("Hit counter: " + hitCounter);

            // Start the blink effect and set isBlinking to true to prevent multiple hits
            StartCoroutine(BlinkTargetPrefab());

            // Check if the player has reached the maximum allowed hits
            if (hitCounter >= maxHits)
            {
                Debug.Log("Player reached max hits. Reverting to ResultScene...");

                // Send a network-wide result message (Ghost wins)
                photonView.RPC("BroadcastGameResult", RpcTarget.All, "Ghosts won by eliminating the protagonist!");

                // Trigger a network-wide scene change using Photon
                photonView.RPC("ReturnToResultScene", RpcTarget.All);
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

    [PunRPC]
    private void BroadcastGameResult(string resultMessage)
    {
        // Store the result message in PlayerPrefs on all clients
        PlayerPrefs.SetString("GameResult", resultMessage);
    }

    [PunRPC]
    private void ReturnToResultScene()
    {
        // Load the ResultScene scene on all clients
        SceneManager.LoadScene("ResultScene");
    }
}
