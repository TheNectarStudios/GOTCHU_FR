using UnityEngine;
using Photon.Pun;

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

        // Request MasterClient to destroy the pearl
        PhotonView pearlView = pearl.GetComponent<PhotonView>();
        if (pearlView != null)
        {
            // Call the RPC to request the MasterClient to destroy the pearl
            photonView.RPC("RequestDestroyPearl", RpcTarget.MasterClient, pearlView.ViewID);
        }
    }

    // RPC that runs on the MasterClient to destroy the pearl
    [PunRPC]
    void RequestDestroyPearl(int viewID)
    {
        // This will only be executed by the MasterClient
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonView pearlPhotonView = PhotonView.Find(viewID);
            if (pearlPhotonView != null)
            {
                // Destroy the pearl across the network
                PhotonNetwork.Destroy(pearlPhotonView.gameObject);
            }
        }
    }
}
