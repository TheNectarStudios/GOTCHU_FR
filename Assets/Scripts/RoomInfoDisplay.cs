using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class RoomInfoDisplay : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI playerListText;  // Reference to the UI Text to display the player list
    public TextMeshProUGUI roomKeyText;     // Reference to display the room key
    public GameObject startGameButton;      // Reference to the Start Game button

    private void Start()
    {
        // Display the current room key in the UI
        roomKeyText.text = $"Room Key: {RoomManager.RoomKey}";
        UpdatePlayerList();

        // Enable the start game button only for the host (the player who created the room)
        if (PhotonNetwork.IsMasterClient)
        {
            startGameButton.SetActive(true);
        }
        else
        {
            startGameButton.SetActive(false);
        }
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

    // Called when the Start Game button is clicked by the host
    public void OnStartGameButtonClicked()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Get the number of players in the room
            int playerCount = PhotonNetwork.PlayerList.Length;

            // Store the player count in PlayerPrefs so it can be accessed by the SpawnManager
            PlayerPrefs.SetInt("PlayerCount", playerCount);
            PlayerPrefs.Save();

            // Load the game scene for all players
            PhotonNetwork.LoadLevel("GameScene");
        }
    }

    // Called when the current player is no longer the host
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        // Update the start game button visibility
        if (PhotonNetwork.IsMasterClient)
        {
            startGameButton.SetActive(true);
        }
        else
        {
            startGameButton.SetActive(false);
        }
    }
}
