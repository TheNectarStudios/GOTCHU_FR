using UnityEngine;
using Photon.Pun;

public class PowerUp : MonoBehaviourPun
{
    public delegate void PowerUpPickedUp();
    public event PowerUpPickedUp onPickedUp;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Trigger the event for when the power-up is picked up
            if (onPickedUp != null)
            {
                onPickedUp.Invoke();
            }

            // Notify all players that the power-up is picked up and destroy it across the network
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
