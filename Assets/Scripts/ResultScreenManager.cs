using UnityEngine;
using TMPro;  // Required for TextMeshPro
using Photon.Pun;  // Required for Photon functionality
using UnityEngine.SceneManagement;  // Required for scene management
using System.Collections;

public class ResultScreenManager : MonoBehaviour
{
    public TextMeshProUGUI resultText;  // Assign this in the inspector

    private void Start()
    {
        // Retrieve the game result message from PlayerPrefs
        string resultMessage = PlayerPrefs.GetString("GameResult", "No result available");

        // Display the result message in the TextMeshProUGUI field
        resultText.text = resultMessage;
    }

    // Function to load the "RoomCreated" scene
    public void LoadRoomCreatedScene()
    {
        SceneManager.LoadScene("RoomCreated");
    }

    // Function to disconnect from Photon and load the "CreateRoom" scene
    public void DisconnectAndLoadCreateRoomScene()
    {
        PhotonNetwork.Disconnect();  // Disconnect from Photon
        StartCoroutine(WaitForDisconnect());  // Wait for disconnection before switching scenes
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
