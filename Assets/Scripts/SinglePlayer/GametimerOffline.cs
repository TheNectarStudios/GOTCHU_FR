using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class GameTimerOffline : MonoBehaviour
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

        // Start the countdown timer
        StartCoroutine(StartCountdown());
    }

    private IEnumerator StartCountdown()
    {
        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;

            // Update the timer display on screen
            UpdateTimerDisplay(currentTime);

            yield return null;
        }

        // Once the timer ends, transition to the result scene
        TransitionToResultScene();
    }

    private void TransitionToResultScene()
    {
        // Store a message indicating the Protagonist has won by surviving the timer
        PlayerPrefs.SetString("GameResult", "The Protagonist has won by surviving the timer!");

        // Load the "ResultScene" scene
        SceneManager.LoadScene("ResultSceneOffline");
    }

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
