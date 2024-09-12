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

        if (PhotonNetwork.IsMasterClient)  // Only the host manages the timer
        {
            StartCoroutine(StartCountdown());
        }
    }

    private IEnumerator StartCountdown()
    {
        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;

            // Update the timer on all clients
            photonView.RPC("UpdateTimerDisplay", RpcTarget.All, currentTime);

            yield return null;
        }

        // Once the timer ends, transition all players to the "RoomCreated" scene
        TransitionToResultScene();
    }

    private void TransitionToResultScene()
    {
        // Only the host triggers the scene change
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("ChangeSceneForAll", RpcTarget.All, "RoomCreated");  // Transition to "RoomCreated" scene
        }
    }

    [PunRPC]
    private void ChangeSceneForAll(string sceneName)
    {
        // This will load the "RoomCreated" scene for each player
        SceneManager.LoadScene(sceneName);
    }

    [PunRPC]
    private void UpdateTimerDisplay(float timeLeft)
    {
        // Convert the remaining time into minutes and seconds format
        int minutes = Mathf.FloorToInt(timeLeft / 60);
        int seconds = Mathf.FloorToInt(timeLeft % 60);

        // Update the TMP text field
        if (timerText != null)
        {
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}
