using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;  // Import SceneManagement for scene transitions

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField playerNameInput;
    public PanelShake panelShake;  // Reference to the PanelShake script

    private void Start()
    {
        // Ensure any UI setup if needed
        if (panelShake == null)
        {
            panelShake = FindObjectOfType<PanelShake>(); // Find the PanelShake script in the scene if not assigned
        }
    }

    public void JoinLobby()
    {
        // Log the name entered in the TMP input field
        string playerName = playerNameInput.text.Trim();

        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogWarning("Player name is empty. Please enter a valid name.");
            if (panelShake != null)
            {
                panelShake.TriggerShake();  // Trigger shake when there's an error
            }
            return;  // Stop if player name is empty
        }

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

        // Trigger panel shake only when there is an error
        if (panelShake != null)
        {
            panelShake.TriggerShake();
        }
    }
}
