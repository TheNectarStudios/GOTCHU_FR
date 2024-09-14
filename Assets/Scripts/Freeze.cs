using System.Collections;
using UnityEngine;

public class Freeze : MonoBehaviour
{
    public float freezeDuration = 3f; // Duration for which the freeze effect lasts
    private GameObject[] ghosts;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object colliding with the power-up is tagged "Player"
        if (other.CompareTag("Player"))
        {
            ActivatePowerUp();
        }
    }

    public void ActivatePowerUp()
    {
        // Find all game objects with the tag "Ghost"

        Debug.Log("Freeze Power-Up Activated!");
        ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        // Disable movement for all ghosts
        foreach (GameObject ghost in ghosts)
        {
            PacMan3DMovement movement = ghost.GetComponent<PacMan3DMovement>();
            if (movement != null)
            {
                movement.enabled = false; // Disable movement
            }
        }

        // Instead of disabling the Renderer/Collider, just hide the object visually by setting it transparent
        GetComponent<Renderer>().material.color = new Color(1, 1, 1, 0); // Set alpha to 0 (make invisible)

        // Start the coroutine to unfreeze after the delay
        StartCoroutine(UnfreezeAfterDelay());
    }

    private IEnumerator UnfreezeAfterDelay()
    {
        // Wait for the duration of the freeze effect
        yield return new WaitForSeconds(freezeDuration);

        // Find all game objects with the tag "Ghost" again
        ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        // Re-enable movement for all ghosts
        foreach (GameObject ghost in ghosts)
        {
            PacMan3DMovement movement = ghost.GetComponent<PacMan3DMovement>();
            if (movement != null)
            {
                movement.enabled = true; // Re-enable movement
            }
        }

        // Now destroy the power-up object after the effect duration
        Destroy(gameObject);
    }
}
