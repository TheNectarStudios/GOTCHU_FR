using System;
using System.Collections;
using UnityEngine;
using Photon.Pun; // Photon support

public class PowerUp : MonoBehaviourPun
{
    public static event Action<PowerUp> onPickedUp; // Event for when the power-up is picked up
    public float effectDuration = 5f;
    private GameObject[] ghosts;

    public GameObject pickupEffectPrefab;  // Reference to the prefab animation, assign in the editor

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ActivatePowerUp();
            PlayPickupEffect(); // Play the prefab animation
            onPickedUp?.Invoke(this); // Notify that the power-up has been picked up
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
                // Example effect on ghost, e.g. freeze movement or reverse speed
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
            // Instantiate the animation prefab at the power-up's position with its current rotation
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

        if (photonView.IsMine || PhotonNetwork.IsMasterClient)
        {
            // PhotonNetwork.Destroy(gameObject);  // Destroy the power-up after usage
            // Debug.Log("Power-up destroyed");
        }
    }
}
