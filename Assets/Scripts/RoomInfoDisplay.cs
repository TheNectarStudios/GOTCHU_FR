using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class RoomInfoDisplay : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI playerListText;  // Reference to the UI Text to display the player list
    public TextMeshProUGUI roomKeyText;     // Reference to display the room key
    public GameObject startGameButton;      // Reference to the Start Game button

    private void Start()
    {
        // Display the current room key in the UI
        roomKeyText.text = $"<b><color=#FFD700>Room Code: {RoomManager.RoomKey}</color></b>";
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

        // Number and list each player's name
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Player player = PhotonNetwork.PlayerList[i];
            string playerName = !string.IsNullOrEmpty(player.NickName) ? player.NickName : "Unknown Player";

            // Add player number and name to the list
            playerNames += $"{i + 1}. {playerName}\n";
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
            // Make the room private so no one else can join
            PhotonNetwork.CurrentRoom.IsOpen = false;  // This will close the room for any new players
            PhotonNetwork.CurrentRoom.IsVisible = false;  // This will make the room invisible in the lobby

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

    // New function to exit the room, disconnect from Photon, and load "CreateRoom" scene
    public void ExitRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();  // Leave the room first
        }
        else
        {
            DisconnectAndLoadScene();  // If not in room, directly disconnect and load the scene
        }
    }

    // Called when successfully left the room
    public override void OnLeftRoom()
    {
        DisconnectAndLoadScene();  // Once the room is left, disconnect and load scene
    }

    // Disconnect from Photon and load the "CreateRoom" scene
    private void DisconnectAndLoadScene()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();  // Disconnect from Photon
            StartCoroutine(WaitForDisconnect());
        }
        else
        {
            SceneManager.LoadScene("CreateRoom");  // Load the "CreateRoom" scene immediately if already disconnected
        }
    }

    // Coroutine to ensure we only switch scenes after the disconnection is complete
    private IEnumerator WaitForDisconnect()
    {
        while (PhotonNetwork.IsConnected)
        {
            yield return null;  // Wait until we're disconnected from Photon
        }

        // Now that we're disconnected, load the "CreateRoom" scene
        SceneManager.LoadScene("CreateRoom");
    }
}
