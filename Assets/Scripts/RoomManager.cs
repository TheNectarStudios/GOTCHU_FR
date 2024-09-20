using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Linq;  // <-- Add this to fix the Enumerable error

public class RoomManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField roomKeyInput;  // Input field for joining rooms
    public TextMeshProUGUI occupancyRateText;

    private string mapType;
    public static string RoomKey; // Store the room key
    private bool isDisconnecting = false; // Flag to track if we are disconnecting
    public GameObject loadingBufferUI; // Optional: Loading buffer UI object

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

    public void OnBackToJoinRoom()
    {
        // If we are already disconnecting, don't start again
        if (!isDisconnecting)
        {
            Debug.Log("Disconnecting and returning to JoinRoom scene...");
            isDisconnecting = true;
            
            // Show loading buffer UI (if available)
            if (loadingBufferUI != null)
            {
                loadingBufferUI.SetActive(true);  // Show loading indicator
            }

            // Start the disconnection and scene change with buffer
            StartCoroutine(DisconnectAndReturnWithBuffer());
        }
        else
        {
            Debug.LogWarning("Already in the process of disconnecting.");
        }
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
        SceneManager.LoadScene("RoomCreated");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Successfully joined the room with key: " + RoomKey);
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

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"Disconnected from Photon: {cause}");
        // Proceed with scene change after disconnect
        if (isDisconnecting)
        {
            Debug.Log("Successfully disconnected from Photon. Loading 'JoinRoom' scene.");
            isDisconnecting = false; // Reset flag

            // Hide loading buffer UI (if available)
            if (loadingBufferUI != null)
            {
                loadingBufferUI.SetActive(false);
            }

            SceneManager.LoadScene("JoinRoom");
        }
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
        System.Random random = new System.Random();

        // Generate a 4-digit numeric key
        string key = new string(Enumerable.Repeat(digits, 4)
            .Select(s => s[random.Next(s.Length)]).ToArray());

        return key;
    }

    // Coroutine to handle disconnection and delay (buffer) before changing the scene
    private IEnumerator DisconnectAndReturnWithBuffer()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();

            // Optional: Add a small buffer time (e.g., 2 seconds)
            yield return new WaitForSeconds(2f);
        }
        else
        {
            Debug.LogWarning("Not connected to Photon, proceeding to scene change...");
        }

        // Ensure that Photon is fully disconnected before proceeding
        while (PhotonNetwork.IsConnected)
        {
            yield return null;  // Wait for disconnection
        }

        SceneManager.LoadScene("JoinRoom");  // Finally change the scene
    }
    #endregion
}
