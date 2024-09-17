using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;  // Required for TextMeshPro

public class GameTimer : MonoBehaviourPun
{
    public float gameDuration = 120f;  // Duration of the game in seconds
    private float currentTime;

    // Reference to TextMeshProUGUI to display the timer
    public TextMeshProUGUI timerText;

    private void Start()
    {
        currentTime = gameDuration;

        // Update the timer display at the start
        UpdateTimerDisplay(currentTime);

        // Only the MasterClient will manage the timer countdown
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(StartCountdown());
        }
    }

    // Countdown logic is only run by the MasterClient
    private IEnumerator StartCountdown()
    {
        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;

            // Update the timer on all clients via an RPC call
            photonView.RPC("UpdateTimerDisplay", RpcTarget.All, currentTime);

            yield return null;
        }

        // Once the timer ends, transition all players to the "ResultScene" scene
        TransitionToResultScene();
    }

    // The MasterClient transitions all players to the result scene and sends the game result
    private void TransitionToResultScene()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Store a message indicating the Protagonist has won by surviving the timer
            PlayerPrefs.SetString("GameResult", "The Protagonist has won by surviving the timer!");

            // Send the result to all players via an RPC call
            photonView.RPC("SetGameResultForAll", RpcTarget.All, "The Protagonist has won by surviving the timer!");

            // Trigger the scene change for all players
            photonView.RPC("ChangeSceneForAll", RpcTarget.All, "ResultScene");
        }
    }

    // RPC to change the scene for all clients
    [PunRPC]
    private void ChangeSceneForAll(string sceneName)
    {
        // This will load the "ResultScene" scene for each player
        SceneManager.LoadScene(sceneName);
    }

    // RPC to update the game result across all clients
    [PunRPC]
    private void SetGameResultForAll(string resultMessage)
    {
        // Store the result message in PlayerPrefs on all clients
        PlayerPrefs.SetString("GameResult", resultMessage);
    }

    // RPC to update the timer display across all clients
    [PunRPC]
    private void UpdateTimerDisplay(float timeLeft)
    {
        // Convert the remaining time into minutes and seconds format
        int minutes = Mathf.FloorToInt(timeLeft / 60);
        int seconds = Mathf.FloorToInt(timeLeft % 60);

        // Update the TMP text field for the timer
        if (timerText != null)
        {
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}
