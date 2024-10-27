using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;  // Import UI for Image manipulation
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField roomKeyInput;
    public TextMeshProUGUI occupancyRateText;
    public TextMeshProUGUI playerNameDisplay;  // Display for player's name
    public GameObject nameEditPanel;           // Panel for editing the name
    public TMP_InputField nameInputField;      // Input field for player name
    public GameObject loadingScreen;           // Reference to the loading screen UI
    public Image loadingBar;                   // Loading bar UI (Image component)

    private string mapType;
    public static string RoomKey;
    private bool isDisconnecting = false;

    private const int MaxPlayersInRoom = 4;    // Max players in a room
    private List<RoomInfo> availableRooms = new List<RoomInfo>();  // Store available rooms

    private const string PlayerNameKey = "PlayerName";  // Key for storing player name

    private void Start()
    {
        // Ensure loading UI is hidden on start
        if (loadingScreen != null) loadingScreen.SetActive(false);

        PhotonNetwork.AutomaticallySyncScene = true;

        // Check if a player name is saved in PlayerPrefs
        if (PlayerPrefs.HasKey(PlayerNameKey))
        {
            string savedPlayerName = PlayerPrefs.GetString(PlayerNameKey);
            PhotonNetwork.NickName = savedPlayerName; // Set the saved name to Photon Network
            playerNameDisplay.text = savedPlayerName; // Display the saved name in the UI
        }
        else
        {
            // Generate a random player name
            string randomPlayerName = "Player" + Random.Range(100, 1000).ToString();
            PhotonNetwork.NickName = randomPlayerName; // Set the random name to Photon Network
            playerNameDisplay.text = randomPlayerName; // Display the player name in the UI
            PlayerPrefs.SetString(PlayerNameKey, randomPlayerName); // Save the new name
            PlayerPrefs.Save(); // Ensure data is saved
        }

        if (!PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            PhotonNetwork.JoinLobby();  // Join the lobby to get room updates
        }
    }

    #region UI Callback Methods
    public void OnCreateRoomButtonClicked()
    {
        RoomKey = GenerateRoomKey();
        Debug.Log($"Creating a private room with key: {RoomKey}");

        // Fake loading before creating the room
        StartCoroutine(FakeLoadingRoutine(1.5f, () =>
        {
            TryCreateAndJoinRoom(isPrivateRoom: true);  // Create a private room
        }));
    }

    public void OnJoinRoomButtonClicked()
    {
        RoomKey = roomKeyInput.text.Trim();
        if (!string.IsNullOrEmpty(RoomKey))
        {
            // Fake loading before joining the room
            StartCoroutine(FakeLoadingRoutine(1.5f, TryJoinRoom));
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
            PhotonNetwork.NickName = newName; // Save the new name in PhotonNetwork's NickName
            PlayerPrefs.SetString(PlayerNameKey, newName); // Save new name in PlayerPrefs
            PlayerPrefs.Save(); // Ensure data is saved
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
            if (loadingScreen != null) loadingScreen.SetActive(true);
            StartCoroutine(DisconnectAndReturnWithBuffer());
        }
    }

    public void OnPlayButtonClicked()
    {
        // Fake loading before joining a random public room
        StartCoroutine(FakeLoadingRoutine(2f, TryJoinRoomWithQueue));
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
            if (loadingScreen != null) loadingScreen.SetActive(false);
            SceneManager.LoadScene("JoinRoom");
        }
    }
    #endregion

    #region Private Methods
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

        Debug.Log("No public room available. Creating a new public room...");
        RoomKey = GenerateRoomKey();
        TryCreateAndJoinRoom(isPrivateRoom: false);  // Create a public room
    }

    private void TryCreateAndJoinRoom(bool isPrivateRoom)
    {
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = MaxPlayersInRoom,
            IsVisible = !isPrivateRoom,
            IsOpen = true,
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

    // Fake loading screen routine for actions
    private IEnumerator FakeLoadingRoutine(float duration, System.Action onComplete)
    {
        if (loadingScreen != null) loadingScreen.SetActive(true);  // Show loading screen
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // Update loading bar fill amount based on elapsed time
            if (loadingBar != null)
            {
                loadingBar.fillAmount = elapsedTime / duration;
            }

            yield return null;
        }

        // Hide loading screen and call the completion action
        if (loadingScreen != null) loadingScreen.SetActive(false);
        onComplete?.Invoke();
    }
    #endregion
}
