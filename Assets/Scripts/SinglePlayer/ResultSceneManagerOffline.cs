using UnityEngine;
using TMPro;  // Required for TextMeshPro
using UnityEngine.SceneManagement;  // Required for scene management

public class ResultSceneManagerOffline : MonoBehaviour
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
        SceneManager.LoadScene("SinglePlayerArenaFinal");
    }

    // Function to load the "CreateRoom" scene
    public void LoadCreateRoomScene()
    {
        // Directly load the "CreateRoom" scene without any Photon disconnection logic
        SceneManager.LoadScene("CreateRoom");
    }
}
