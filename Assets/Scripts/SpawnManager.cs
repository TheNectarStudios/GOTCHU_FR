using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviourPunCallbacks
{
    public Transform spawnPoint;
    public GameObject protagonistPrefab;
    public GameObject antagonistPrefab;
    public float bufferTime = 3.0f;  // Time to wait before assigning roles

    private void Start()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
        {
            // Start the role assignment after a buffer period
            StartCoroutine(WaitAndAssignRoles());
        }
    }

    private IEnumerator WaitAndAssignRoles()
    {
        // Wait for the buffer time to allow all players to join
        yield return new WaitForSeconds(bufferTime);

        // Now perform role assignment
        AssignRoles();
    }

    private void AssignRoles()
    {
        List<Player> players = new List<Player>(PhotonNetwork.PlayerList);
        int protagonistIndex = Random.Range(0, players.Count);

        for (int i = 0; i < players.Count; i++)
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
        PhotonNetwork.Instantiate(protagonistPrefab.name, spawnPoint.position, spawnPoint.rotation);
    }

    [PunRPC]
    private void SetAntagonist()
    {
        PhotonNetwork.Instantiate(antagonistPrefab.name, spawnPoint.position, spawnPoint.rotation);
    }
}
