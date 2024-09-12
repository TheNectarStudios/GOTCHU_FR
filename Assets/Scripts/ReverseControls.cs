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

        // Instead of disabling the Renderer/Collider, just hide the object visually by setting it transparent
        GetComponent<Renderer>().material.color = new Color(1, 1, 1, 0); // Set alpha to 0 (make invisible)

        // Start the coroutine to restore controls after a delay
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

        // Now destroy the power-up object after the effect duration
        Destroy(gameObject);
    }
}
