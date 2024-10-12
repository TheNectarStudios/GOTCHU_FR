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

    private string mapType;
    public static string RoomKey;
    private bool isDisconnecting = false;
    public GameObject loadingBufferUI;

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
    }

    #region UI Callback Methods
    public void OnCreateRoomButtonClicked()
    {
        RoomKey = GenerateRoomKey();
        TryCreateAndJoinRoom();
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

    public void OnBackToJoinRoom()
    {
        if (!isDisconnecting)
        {
            isDisconnecting = true;
            if (loadingBufferUI != null) loadingBufferUI.SetActive(true);
            StartCoroutine(DisconnectAndReturnWithBuffer());
        }
    }

    // New Play Button Callback for joining a random room
    public void OnPlayButtonClicked()
    {
        TryJoinRoomWithQueue();
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
        Debug.Log("No random room available or all rooms are full. Creating a new room.");
        RoomKey = GenerateRoomKey();
        TryCreateAndJoinRoom();
    }

    // This method is called when the room list updates
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        availableRooms = roomList;  // Update the available rooms list
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
    // Core function to handle the queuing system
    private void TryJoinRoomWithQueue()
    {
        StartCoroutine(CheckForAvailableRoom());
    }

    private IEnumerator CheckForAvailableRoom()
    {
        // Check available rooms and try to join one that is not full
        foreach (RoomInfo room in availableRooms)
        {
            if (room.PlayerCount < MaxPlayersInRoom && !room.RemovedFromList)
            {
                Debug.Log($"Found a room that is not full: {room.Name}. Joining...");
                RoomKey = room.Name;
                PhotonNetwork.JoinRoom(room.Name);
                yield break;  // Exit the coroutine as we have found a room
            }
        }

        // If no suitable room is found, create a new one
        Debug.Log("No suitable room found. Creating a new room...");
        RoomKey = GenerateRoomKey();
        TryCreateAndJoinRoom();
    }

    private void TryCreateAndJoinRoom()
    {
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = MaxPlayersInRoom,
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
