using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField roomKeyInput;
    public TextMeshProUGUI occupancyRateText;
    public TextMeshProUGUI playerNameDisplay;  // TextMeshProUGUI for displaying the player's name
    public GameObject nameEditPanel;           // Panel for editing the name
    public TMP_InputField nameInputField;      // Input field for new player name
    public GameObject loadingBufferUI;

    private string mapType;
    public static string RoomKey;
    private bool isDisconnecting = false;

    private const int MaxPlayersInRoom = 4; // Max players per room
    private List<RoomInfo> availableRooms = new List<RoomInfo>();  // Store available rooms

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        if (!PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            PhotonNetwork.JoinLobby();  // Join the lobby to get the room list updates
        }

        // Retrieve the player's name from PlayerPrefs and display it
        string playerName = PlayerPrefs.GetString("PlayerName", "Player"); // Default to "Player" if not found
        playerNameDisplay.text = playerName; // Display the player name in the UI
    }

    #region UI Callback Methods
    // For creating a private room (play with friends)
    public void OnCreateRoomButtonClicked()
    {
        RoomKey = GenerateRoomKey();
        Debug.Log($"Creating a private room with key: {RoomKey}");
        TryCreateAndJoinRoom(isPrivateRoom: true);  // Create a private room
    }

    public void OnJoinRoomButtonClicked()
    {
        RoomKey = roomKeyInput.text.Trim();
        if (!string.IsNullOrEmpty(RoomKey))
        {
            TryJoinRoom();
        }
        else
        {
            Debug.LogWarning("Room key is empty. Cannot join the room.");
        }
    }

    public void OnEditNameButtonClicked()
    {
        nameEditPanel.SetActive(true); // Show the name edit panel
        nameInputField.text = playerNameDisplay.text; // Populate the input field with the current name
    }

    public void OnSaveNameButtonClicked()
    {
        string newName = nameInputField.text.Trim();
        if (!string.IsNullOrEmpty(newName))
        {
            playerNameDisplay.text = newName; // Update the displayed name
            PlayerPrefs.SetString("PlayerName", newName); // Save the new name in PlayerPrefs
            PlayerPrefs.Save(); // Ensure PlayerPrefs data is saved
            nameEditPanel.SetActive(false); // Hide the name edit panel
        }
        else
        {
            Debug.LogWarning("Player name cannot be empty."); // Warning for empty name
        }
    }

    public void OnCancelEditButtonClicked()
    {
        nameEditPanel.SetActive(false); // Hide the name edit panel without saving
    }

    public void OnBackToJoinRoom()
    {
        if (!isDisconnecting)
        {
            isDisconnecting = true;
            if (loadingBufferUI != null) loadingBufferUI.SetActive(true);
            StartCoroutine(DisconnectAndReturnWithBuffer());
        }
    }

    // Play button to join a random public room
    public void OnPlayButtonClicked()
    {
        TryJoinRoomWithQueue();  // Join a public room
    }
    #endregion

    #region Photon Callback Methods
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();  // Join the lobby after connecting
    }

    public override void OnCreatedRoom()
    {
        SceneManager.LoadScene("RoomCreated");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined room with key: {RoomKey}");

        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("MapType", out object mapTypeObj))
        {
            mapType = (string)mapTypeObj;
            if (mapType == "Outdoor") PhotonNetwork.LoadLevel("World_Outdoor");
            else if (mapType == "School") PhotonNetwork.LoadLevel("World_School");
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Failed to join room: {message}");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("No random room available or all rooms are full. Creating a new public room.");
        RoomKey = GenerateRoomKey();
        TryCreateAndJoinRoom(isPrivateRoom: false);  // Create a public room
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        availableRooms = roomList;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (isDisconnecting)
        {
            isDisconnecting = false;
            if (loadingBufferUI != null) loadingBufferUI.SetActive(false);
            SceneManager.LoadScene("JoinRoom");
        }
    }
    #endregion

    #region Private Methods
    // Try to join a public room with the queuing system
    private void TryJoinRoomWithQueue()
    {
        StartCoroutine(CheckForAvailableRoom());
    }

    private IEnumerator CheckForAvailableRoom()
    {
        foreach (RoomInfo room in availableRooms)
        {
            if (room.PlayerCount < MaxPlayersInRoom && !room.RemovedFromList && room.IsVisible)
            {
                Debug.Log($"Found a public room: {room.Name}. Joining...");
                RoomKey = room.Name;
                PhotonNetwork.JoinRoom(room.Name);
                yield break;
            }
        }

        // No suitable public room found, create a new public room
        Debug.Log("No public room available. Creating a new public room...");
        RoomKey = GenerateRoomKey();
        TryCreateAndJoinRoom(isPrivateRoom: false);  // Create a public room
    }

    // Create a room, can be public or private
    private void TryCreateAndJoinRoom(bool isPrivateRoom)
    {
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = MaxPlayersInRoom,
            IsVisible = !isPrivateRoom,  // Set visibility based on whether the room is private or public
            IsOpen = true,  // Allow players to join
            CustomRoomProperties = new ExitGames.Client.Photon.Hashtable
            {
                { "RoomKey", RoomKey },
                { "MapType", mapType }
            },
            CustomRoomPropertiesForLobby = new[] { "MapType", "RoomKey" }
        };

        PhotonNetwork.CreateRoom(RoomKey, roomOptions);
    }

    private void TryJoinRoom()
    {
        PhotonNetwork.JoinRoom(RoomKey);
    }

    private string GenerateRoomKey()
    {
        const string digits = "0123456789";
        System.Random random = new System.Random();
        return new string(Enumerable.Repeat(digits, 4).Select(s => s[random.Next(s.Length)]).ToArray());
    }

    private IEnumerator DisconnectAndReturnWithBuffer()
    {
        if (PhotonNetwork.IsConnected) PhotonNetwork.Disconnect();
        yield return new WaitForSeconds(2f);
        while (PhotonNetwork.IsConnected) yield return null;
        SceneManager.LoadScene("JoinRoom");
    }
    #endregion
}
    