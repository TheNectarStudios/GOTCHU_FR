using UnityEngine;
using System.Collections;

public class Freeze : MonoBehaviour
{
    public float freezeDuration = 3f; // Duration for which the controls are frozen
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
        ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        // Freeze the controls for all ghosts
        foreach (GameObject ghost in ghosts)
        {
            PacMan3DMovement movement = ghost.GetComponent<PacMan3DMovement>();
            if (movement != null)
            {
                movement.enabled = false; // Disable movement script to freeze the ghost
            }
        }

        // Hide the power-up and disable its collider
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        // Start coroutine to restore controls after a delay
        StartCoroutine(UnfreezeAfterDelay());
    }

    private IEnumerator UnfreezeAfterDelay()
    {
        // Wait for the duration of the freeze effect
        yield return new WaitForSeconds(freezeDuration);

        // Find all game objects with the tag "Ghost" again
        ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        // Restore the controls for all ghosts
        foreach (GameObject ghost in ghosts)
        {
            PacMan3DMovement movement = ghost.GetComponent<PacMan3DMovement>();
            if (movement != null)
            {
                movement.enabled = true; // Re-enable movement script
            }
        }

        // Destroy the power-up object
        Destroy(gameObject);
    }
}
