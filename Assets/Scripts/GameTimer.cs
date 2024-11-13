using UnityEngine;
using Photon.Pun;
using System.Collections;
using TMPro;  // Required for TextMeshPro

public class GameTimer : MonoBehaviourPun
{
    public float gameDuration = 120f;  // Duration of the game in seconds
    private float currentTime;

    // Reference to TextMeshProUGUI to display the timer
    public TextMeshProUGUI timerText;

    // Reference to the DecisionMaker script
    private DecisionMaker decisionMaker;

    private void Start()
    {
        currentTime = gameDuration;

        // Update the timer display at the start
        UpdateTimerDisplay(currentTime);

        // Get the reference to the DecisionMaker script in the scene
        decisionMaker = FindObjectOfType<DecisionMaker>();

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

        // Once the timer ends, inform the DecisionMaker script to handle the scene transition
        if (decisionMaker != null)
        {
            decisionMaker.OnTimeExpired();
        }
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
