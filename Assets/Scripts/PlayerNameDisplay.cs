using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerNameDisplay : MonoBehaviourPun
{
    public TMP_Text playerNameText; // Reference to the TextMeshPro component

    void Start()
    {
        // Check if this is the local player or another player's object
        if (photonView.IsMine)
        {
            // Set the player name text to the current player's Photon nickname (which is set from PlayerPrefs)
            playerNameText.text = PhotonNetwork.NickName;
        }
        else
        {
            // Set the player name text to the other player's Photon nickname
            playerNameText.text = photonView.Owner.NickName;
        }
    }
}
