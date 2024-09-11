using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameTimer : MonoBehaviourPun
{
    public float gameDuration = 120f;  // Duration of the game in seconds
    private float currentTime;

    private void Start()
    {
        currentTime = gameDuration;

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
}
