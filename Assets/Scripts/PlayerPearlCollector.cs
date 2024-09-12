using System.Collections;  // Required for IEnumerator
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerPearlCollector : MonoBehaviourPun
{
    public int collectedPearls = 0;  // Track the number of collected pearls

    private void OnTriggerEnter(Collider other)
    {
        if (photonView.IsMine)  // Ensure only the local player can collect the pearl
        {
            if (other.CompareTag("Pearl"))  // Assuming the pearl has the tag "Pearl"
            {
                CollectPearl(other.gameObject);
            }
        }
    }

    void CollectPearl(GameObject pearl)
    {
        // Increase the number of collected pearls
        collectedPearls++;

        // Optionally, add some visual/audio feedback
        Debug.Log("Collected a pearl! Total pearls: " + collectedPearls);

        // Get the PhotonView of the pearl
        PhotonView pearlView = pearl.GetComponent<PhotonView>();
        if (pearlView != null)
        {
            // Check if the local player is the owner of the pearl
            if (pearlView.IsMine)
            {
                // If the local player owns the pearl, destroy it
                PhotonNetwork.Destroy(pearl);
            }
            else
            {
                // If the player is the MasterClient, they can request ownership and destroy it
                RequestOwnershipAndDestroy(pearlView);
            }
        }
    }

    void RequestOwnershipAndDestroy(PhotonView pearlView)
    {
        // If the current player is the MasterClient
        if (PhotonNetwork.IsMasterClient)
        {
            // MasterClient should request ownership first, even though it can destroy objects
            Debug.Log("MasterClient requesting ownership of pearl before destroying.");
            pearlView.RequestOwnership();
            StartCoroutine(WaitAndDestroy(pearlView));
        }
        else
        {
            // If not the MasterClient, destroy after ownership transfer
            Debug.Log("Requesting ownership and waiting to destroy.");
            pearlView.RequestOwnership();
            StartCoroutine(WaitAndDestroy(pearlView));
        }
    }

    IEnumerator WaitAndDestroy(PhotonView pearlView)
    {
        // Wait for ownership transfer to complete
        yield return new WaitForSeconds(0.2f);

        // Check if ownership is transferred to this client
        if (pearlView.IsMine)
        {
            // Now the local player owns the pearl, so destroy it
            Debug.Log("Ownership transfer successful, destroying pearl.");
            PhotonNetwork.Destroy(pearlView.gameObject);
        }
        else
        {
            Debug.LogError("Failed to take ownership of the pearl. Unable to destroy.");
        }
    }
}
