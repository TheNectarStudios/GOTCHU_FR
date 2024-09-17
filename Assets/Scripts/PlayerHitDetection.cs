using UnityEngine;
using System.Collections;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class PlayerHitDetection : MonoBehaviourPunCallbacks
{
    public MeshRenderer playerMeshRenderer;  // Reference to the player's MeshRenderer
    public float blinkDuration = 2f;         // Total time for blinking
    public int blinkCount = 3;               // Number of times the player blinks
    private bool isBlinking = false;         // Boolean to prevent multiple hit registrations during blinking
    private int hitCounter = 0;              // Player's hit counter
    public int maxHits = 3;                  // Maximum hits allowed before returning to the RoomCreated scene

    private void Start()
    {
        // Ensure that the player's MeshRenderer is assigned
        if (playerMeshRenderer == null)
        {
            playerMeshRenderer = GetComponent<MeshRenderer>();
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
            StartCoroutine(BlinkPlayerMesh());

            // Check if the player has reached the maximum allowed hits
            if (hitCounter >= maxHits)
            {
                Debug.Log("Player reached max hits. Reverting to RoomCreated scene...");
                
                // Trigger a network-wide scene change using Photon
                photonView.RPC("ReturnToRoomCreated", RpcTarget.All);
            }
        }
    }

    private IEnumerator BlinkPlayerMesh()
    {
        isBlinking = true; // Prevent additional hit counts during blinking
        float blinkInterval = blinkDuration / (blinkCount * 2); // Time between on and off states

        for (int i = 0; i < blinkCount; i++)
        {
            // Disable the mesh renderer (make the player invisible)
            playerMeshRenderer.enabled = false;
            yield return new WaitForSeconds(blinkInterval);

            // Enable the mesh renderer (make the player visible again)
            playerMeshRenderer.enabled = true;
            yield return new WaitForSeconds(blinkInterval);
        }

        // Ensure the player is visible after blinking
        playerMeshRenderer.enabled = true;

        // Reset the blinking flag after the blinking period is over
        isBlinking = false;
    }

    [PunRPC]
    private void ReturnToRoomCreated()
    {
        // Load the RoomCreated scene
        SceneManager.LoadScene("RoomCreated");
    }
}
