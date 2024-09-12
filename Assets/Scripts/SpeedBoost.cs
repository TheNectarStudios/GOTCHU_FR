using System.Collections;
using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    public float speedMultiplier = 1.5f; // Speed boost multiplier
    public float duration = 5f; // Duration of the speed boost
    private PacMan3DMovement playerMovement;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object colliding with the power-up is tagged "Player"
        if (other.CompareTag("Player"))
        {
            playerMovement = other.GetComponent<PacMan3DMovement>();
            if (playerMovement != null)
            {
                ActivatePowerUp();
            }
        }
    }

    public void ActivatePowerUp()
    {
        // Apply the speed boost
        playerMovement.speed *= speedMultiplier;

        // Instead of disabling the Renderer/Collider, just hide the object visually by setting it transparent
        GetComponent<Renderer>().material.color = new Color(1, 1, 1, 0); // Set alpha to 0 (make invisible)

        // Start the coroutine to restore the original speed after a delay
        StartCoroutine(ResetSpeedAfterDelay());
    }

    private IEnumerator ResetSpeedAfterDelay()
    {
        // Wait for the duration of the speed boost
        yield return new WaitForSeconds(duration);

        // Reset the player's speed to its original value
        playerMovement.speed /= speedMultiplier;

        // Destroy the power-up object after the effect duration
        Destroy(gameObject);
    }
}
