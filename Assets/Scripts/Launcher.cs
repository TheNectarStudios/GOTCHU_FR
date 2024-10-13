using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;  // Import SceneManagement for scene transitions

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public PanelShake panelShake;  // Reference to the PanelShake script

    private void Start()
    {
        // Ensure any UI setup if needed
        if (panelShake == null)
        {
            panelShake = FindObjectOfType<PanelShake>(); // Find the PanelShake script in the scene if not assigned
        }

        // Generate a random player name
        string randomPlayerName = "Player" + Random.Range(100, 1000).ToString();
        Debug.Log("Assigned Player Name: " + randomPlayerName);

        // Set the Photon Network player nickname
        PhotonNetwork.NickName = randomPlayerName;

        // Save the player name for later use
        PlayerPrefs.SetString("PlayerName", randomPlayerName);
        PlayerPrefs.Save();  // Ensure the data is saved

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

        // Trigger panel shake only when there is an error
        if (panelShake != null)
        {
            panelShake.TriggerShake();
        }
    }
}
