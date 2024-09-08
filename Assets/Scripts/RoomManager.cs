using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField roomKeyInput;  // Input field for joining rooms
    public TextMeshProUGUI occupancyRateText;

    private string mapType;
    public static string RoomKey; // Store the room key

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        if (!PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            PhotonNetwork.JoinLobby();
        }
    }

    #region UI Callback Methods
    public void OnCreateRoomButtonClicked()
    {
        // Generate a unique room key
        RoomKey = GenerateRoomKey();
        Debug.Log($"Room Key Generated: {RoomKey}");

        // Connect to Photon and create a room
        TryCreateAndJoinRoom();
    }

    public void OnJoinRoomButtonClicked()
    {
        // Get the room key from the input field
        RoomKey = roomKeyInput.text.Trim();
        Debug.Log($"Attempting to join room with key: {RoomKey}");

        if (string.IsNullOrEmpty(RoomKey))
        {
            Debug.LogWarning("Room key is empty. Cannot join the room.");
            return;
        }

        // Connect to Photon and join the room
        TryJoinRoom();
    }
    #endregion

    #region Photon Callback Methods
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server");
        PhotonNetwork.JoinLobby();  // Join the lobby after connecting to the master server
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room created with key: " + RoomKey);
        // Optionally update UI with room details or load a new scene
        SceneManager.LoadScene("RoomCreated");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Successfully joined the room with key: " + RoomKey);
        // Load the game scene or any other scene based on room properties
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("MapType", out object mapTypeObj))
        {
            mapType = (string)mapTypeObj;
            if (mapType == "Outdoor")
            {
                PhotonNetwork.LoadLevel("World_Outdoor");
            }
            else if (mapType == "School")
            {
                PhotonNetwork.LoadLevel("World_School");
            }
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Failed to join room: {message}");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo room in roomList)
        {
            if (room.Name.Contains("Outdoor"))
            {
                occupancyRateText.text = $"{room.PlayerCount} / 20";
            }
            else if (room.Name.Contains("School"))
            {
                occupancyRateText.text = $"{room.PlayerCount} / 20";
            }
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError($"Disconnected from Photon: {cause}");
    }
    #endregion

    #region Private Methods
    private void TryCreateAndJoinRoom()
    {
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 4,
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
        const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        System.Random random = new System.Random();

        // Generate 6 random digits
        string key = new string(Enumerable.Repeat(digits, 6)
            .Select(s => s[random.Next(s.Length)]).ToArray());

        // Generate 2 random letters
        key += new string(Enumerable.Repeat(letters, 2)
            .Select(s => s[random.Next(s.Length)]).ToArray());

        // Shuffle the key to ensure randomness of letter positions
        key = new string(key.ToCharArray().OrderBy(c => random.Next()).ToArray());

        return key;
    }
    #endregion
}
