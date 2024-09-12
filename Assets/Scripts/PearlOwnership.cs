using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PearlOwnership : MonoBehaviourPunCallbacks, IPunOwnershipCallbacks
{
    PhotonView m_PhotonView;

    private void Awake()
    {
        m_PhotonView = GetComponent<PhotonView>();
    }

    // Method called when the pearl is collected
    public void CollectPearl(GameObject player)
    {
        Debug.Log("Player is attempting to collect the pearl.");

        // Check if the object that collided has the tag "Player"
        if (player.CompareTag("Player"))
        {
            Debug.Log("Object has 'Player' tag. Proceeding with ownership transfer.");

            if (m_PhotonView.Owner == PhotonNetwork.LocalPlayer)
            {
                // If already owned by the local player, destroy it immediately
                DestroyPearl();
            }
            else
            {
                // Transfer ownership to the local player before destroying
                TransferOwnershipToPlayer(player);
                StartCoroutine(WaitAndDestroyPearl());
            }
        }
        else
        {
            Debug.LogError("The object does not have the 'Player' tag. Ownership transfer and destruction aborted.");
        }
    }

    // Transfer ownership of the pearl to the player who collected it
    public void TransferOwnershipToPlayer(GameObject player)
    {
        Player newOwner = player.GetComponent<PhotonView>().Owner;

        if (m_PhotonView.Owner != newOwner)
        {
            Debug.Log("Requesting ownership transfer to: " + newOwner.NickName);
            m_PhotonView.TransferOwnership(newOwner);
        }
    }

    // Coroutine to wait before destroying the pearl to ensure ownership transfer completes
    IEnumerator WaitAndDestroyPearl()
    {
        yield return new WaitForSeconds(0.2f);  // Wait for 0.2 seconds to ensure the ownership transfer completes

        // Destroy the pearl across the network after the wait
        DestroyPearl();
    }

    // Destroy the pearl across the network
    void DestroyPearl()
    {
        if (m_PhotonView.IsMine)
        {
            Debug.Log("Destroying the pearl.");
            PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            Debug.LogError("Cannot destroy the pearl. Not the owner.");
        }
    }

    // Called when ownership is requested
    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        if (targetView != m_PhotonView)
        {
            return;
        }

        Debug.Log("Ownership requested for: " + targetView.name + " by " + requestingPlayer.NickName);
        m_PhotonView.TransferOwnership(requestingPlayer);
    }

    // Called when ownership is transferred
    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        Debug.Log("Ownership transferred for: " + targetView.name + " from " + previousOwner.NickName + " to " + targetView.Owner.NickName);
    }

    // Called when ownership transfer fails
    public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
    {
        Debug.LogError("Ownership transfer failed for: " + targetView.name + " requested by " + senderOfFailedRequest.NickName);
    }
}
