using System.Collections;
using UnityEngine;

public class ReverseControls : MonoBehaviour
{
    public float reverseDuration = 5f; // Duration for which the controls are reversed
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

        // Invert the controls for all ghosts
        foreach (GameObject ghost in ghosts)
        {
            PacMan3DMovement movement = ghost.GetComponent<PacMan3DMovement>();
            if (movement != null)
            {
                movement.speed *= -1; // Invert the speed to reverse controls
            }
        }

        // Hide the power-up and disable its collider
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        // Start coroutine to restore controls after a delay
        StartCoroutine(UnreverseAfterDelay());
    }

    private IEnumerator UnreverseAfterDelay()
    {
        // Wait for the duration of the reverse effect
        yield return new WaitForSeconds(reverseDuration);

        // Find all game objects with the tag "Ghost" again
        ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        // Restore the controls for all ghosts
        foreach (GameObject ghost in ghosts)
        {
            PacMan3DMovement movement = ghost.GetComponent<PacMan3DMovement>();
            if (movement != null)
            {
                movement.speed *= -1; // Restore the original speed
            }
        }

        // Destroy the power-up object
        Destroy(gameObject);
    }
}
