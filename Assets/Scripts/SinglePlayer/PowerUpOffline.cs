using System;
using System.Collections;
using UnityEngine;

public class PowerUpOffline : MonoBehaviour
{
    public static event Action<string, PowerUpOffline> onPickedUp; // Event for when the power-up is picked up
    public float effectDuration = 5f;
    private GameObject[] ghosts;

    public GameObject pickupEffectPrefab;  // Reference to the prefab animation, assign in the editor

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ActivatePowerUp();
            PlayPickupEffect();

            // Clean up the name to remove (Clone) or any unnecessary suffixes
            string powerUpName = gameObject.name.Replace("(Clone)", "").Replace("Offline", "").Trim();
            onPickedUp?.Invoke(powerUpName, this);
        }
    }



    public void ActivatePowerUp()
    {
        ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in ghosts)
        {
            PacMan3DMovement movement = ghost.GetComponent<PacMan3DMovement>();
            if (movement != null)
            {
                Debug.Log("Power-Up Activated!");
                // Example effect on ghost, e.g., freeze movement or reverse speed
                // movement.speed *= -1; 
            }
        }

        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        StartCoroutine(UnfreezeAfterDelay());
    }

    private void PlayPickupEffect()
    {
        if (pickupEffectPrefab != null)
        {
            Instantiate(pickupEffectPrefab, transform.position, transform.rotation);
        }
        else
        {
            Debug.LogError("Pickup Effect Prefab is not assigned!");
        }
    }

    private IEnumerator UnfreezeAfterDelay()
    {
        yield return new WaitForSeconds(effectDuration);

        foreach (GameObject ghost in ghosts)
        {
            PacMan3DMovement movement = ghost.GetComponent<PacMan3DMovement>();
            if (movement != null)
            {
                Debug.Log("Power-Up Deactivated!");
                // Revert the effect
                // movement.speed *= -1; 
            }
        }

        Destroy(gameObject);
        Debug.Log("Power-up destroyed");
    }
}
