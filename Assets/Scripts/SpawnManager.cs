using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviourPunCallbacks
{
    public Transform spawnPoint;
    public GameObject protagonistPrefab;
    public GameObject antagonistPrefab;

    private void Start()
    {
        // Retrieve the number of players from PlayerPrefs (default to 4 if not set)
        int playerCount = PlayerPrefs.GetInt("PlayerCount", 4);

        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
        {
            AssignRoles(playerCount);
        }
    }

    private void AssignRoles(int playerCount)
    {
        // Retrieve the list of players currently in the room
        List<Player> players = new List<Player>(PhotonNetwork.PlayerList);
        
        // Ensure we are working with the correct number of players
        playerCount = Mathf.Min(playerCount, players.Count);

        // Randomly select a protagonist
        int protagonistIndex = Random.Range(0, playerCount);

        // Assign roles to players
        for (int i = 0; i < playerCount; i++)
        {
            if (i == protagonistIndex)
            {
                photonView.RPC("SetProtagonist", players[i]);
            }
            else
            {
                photonView.RPC("SetAntagonist", players[i]);
            }
        }
    }

    [PunRPC]
    private void SetProtagonist()
    {
        // Instantiate the protagonist at the spawn point
        PhotonNetwork.Instantiate(protagonistPrefab.name, spawnPoint.position, spawnPoint.rotation);
    }

    [PunRPC]
    private void SetAntagonist()
    {
        // Instantiate the antagonist at the spawn point
        PhotonNetwork.Instantiate(antagonistPrefab.name, spawnPoint.position, spawnPoint.rotation);
    }
}
