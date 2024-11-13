using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class DecisionMaker : MonoBehaviourPun
{
    // Method called when the timer expires in the GameTimer script
    public void OnTimeExpired()
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

    // Method called when the player is eliminated
    public void OnPlayerEliminated()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Store a message indicating the Ghosts have won by eliminating the protagonist
            PlayerPrefs.SetString("GameResult", "Ghosts won by eliminating the protagonist!");

            // Send the result to all players via an RPC call
            photonView.RPC("SetGameResultForAll", RpcTarget.All, "Ghosts won by eliminating the protagonist!");

            // Trigger the scene change for all players
            photonView.RPC("ChangeSceneForAll", RpcTarget.All, "ResultScene");
        }
    }

    // Method called when all pearls are collected
    public void OnAllPearlsCollected()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Store a message indicating the Protagonist has won by collecting all pearls
            PlayerPrefs.SetString("GameResult", "The Protagonist has won by collecting all pearls!");

            // Send the result to all players via an RPC call
            photonView.RPC("SetGameResultForAll", RpcTarget.All, "The Protagonist has won by collecting all pearls!");

            // Trigger the scene change for all players
            photonView.RPC("ChangeSceneForAll", RpcTarget.All, "ResultScene");
        }
    }

    // RPC to change the scene for all clients
    [PunRPC]
    private void ChangeSceneForAll(string sceneName)
    {
        // Load the specified scene for each player
        SceneManager.LoadScene(sceneName);
    }

    // RPC to update the game result across all clients
    [PunRPC]
    private void SetGameResultForAll(string resultMessage)
    {
        // Store the result message in PlayerPrefs on all clients
        PlayerPrefs.SetString("GameResult", resultMessage);
    }
}
