using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;  // Import SceneManagement for scene transitions

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField playerNameInput;

    private void Start()
    {
        // Ensure any UI setup if needed
    }

    public void JoinLobby()
    {
        // Log the name entered in the TMP input field
        string playerName = playerNameInput.text;
        Debug.Log("Player Name: " + playerName);

        // Set the Photon Network player nickname
        PhotonNetwork.NickName = playerName;

        // Connect to the Photon Master Server
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server");
        PhotonNetwork.JoinLobby();  // Join the lobby after connecting to the master server
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");

        // Load the CreateRoom scene
        SceneManager.LoadScene("CreateRoom");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError("Disconnected from Photon: " + cause.ToString());
        // Handle disconnection logic if needed
    }
}
