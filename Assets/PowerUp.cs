using System;
using System.Collections;
using UnityEngine;
using Photon.Pun; // Photon support

public class PowerUp : MonoBehaviourPun
{
    public static event Action<PowerUp> onPickedUp; // Event for when the power-up is picked up
    public float effectDuration = 5f;
    private GameObject[] ghosts;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ActivatePowerUp();
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
                // movement.speed *= -1; // Example effect, reverse speed
            }
        }

        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        StartCoroutine(UnfreezeAfterDelay());
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
                // movement.speed *= -1; // Revert the effect
            }
        }

        if (photonView.IsMine || PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
