using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections.Generic;

public class RoomInfoDisplay : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI playerListText;  // Reference to the UI Text to display the player list
    public TextMeshProUGUI roomKeyText;     // Reference to display the room key

    private void Start()
    {
        // Display the current room key in the UI
        roomKeyText.text = $"Room Key: {RoomManager.RoomKey}";
        UpdatePlayerList();
    }

    // Update the player list in the UI
    private void UpdatePlayerList()
    {
        string playerNames = "Players in Room:\n";

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            playerNames += player.NickName + "\n";
        }

        playerListText.text = playerNames;
    }

    // Called when a new player joins the room
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    // Called when a player leaves the room
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }
}
