using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerCollision : MonoBehaviourPunCallbacks
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the other object is a Ghost
        if (other.CompareTag("Ghost"))
        {
            if (photonView.IsMine)
            {
                // Call an RPC to inform all players to switch scenes
                photonView.RPC("ReturnToCreateRoomScene", RpcTarget.All);

                // Destroy the local player object after notifying other players
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    [PunRPC]
    void ReturnToCreateRoomScene()
    {
        // Ensure all clients are synchronized before loading the scene
        Debug.Log("Requesting to load 'RoomCreated' scene for all clients.");
        if (PhotonNetwork.IsMasterClient)
        {
            // MasterClient initiates the scene change to ensure it's synchronized
            PhotonNetwork.LoadLevel("RoomCreated");
        }
    }

    public override void OnLeftRoom()
    {
        // Optionally handle logic for when the player leaves the room
        Debug.Log("Player has left the room.");
    }
}
