using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;  // For UI Button

public class SpawnManager : MonoBehaviourPunCallbacks
{
    // Separate spawn points for protagonist and antagonist
    public Transform protagonistSpawnPoint;
    public Transform antagonistSpawnPoint;

    public GameObject protagonistPrefab;
    public GameObject antagonistPrefab;

    public GameObject buttonUp;    // Reference to the UI buttons
    public GameObject buttonDown;
    public GameObject buttonLeft;
    public GameObject buttonRight;

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
        // Spawn protagonist at the designated spawn point
        GameObject protagonist = PhotonNetwork.Instantiate(protagonistPrefab.name, protagonistSpawnPoint.position, protagonistSpawnPoint.rotation);

        // If this is the local player, link the movement to the UI buttons
        if (photonView.IsMine)
        {
            AssignButtonControls(protagonist);
        }
    }

    [PunRPC]
    private void SetAntagonist()
    {
        // Spawn antagonist at the designated spawn point
        GameObject antagonist = PhotonNetwork.Instantiate(antagonistPrefab.name, antagonistSpawnPoint.position, antagonistSpawnPoint.rotation);

        // If this is the local player, link the movement to the UI buttons
        if (photonView.IsMine)
        {
            AssignButtonControls(antagonist);
        }
    }

    // Function to assign UI button controls to the local player
    private void AssignButtonControls(GameObject player)
    {
        PacMan3DMovement movementScript = player.GetComponent<PacMan3DMovement>();

        if (movementScript != null)
        {
            // Assign button actions
            buttonUp.GetComponent<Button>().onClick.AddListener(movementScript.MoveUp);
            buttonDown.GetComponent<Button>().onClick.AddListener(movementScript.MoveDown);
            buttonLeft.GetComponent<Button>().onClick.AddListener(movementScript.MoveLeft);
            buttonRight.GetComponent<Button>().onClick.AddListener(movementScript.MoveRight);
        }
    }
}
