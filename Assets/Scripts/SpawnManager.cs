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

    public Button buttonUp;    // Reference to the UI buttons
    public Button buttonDown;
    public Button buttonLeft;
    public Button buttonRight;

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

        // Assign UI button controls only for the local player
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

        // Assign UI button controls only for the local player
        if (photonView.IsMine)
        {
            AssignButtonControls(antagonist);
        }
    }

    // Function to assign UI button controls to the local player
    private void AssignButtonControls(GameObject player)
    {
        // Ensure the player object has the movement script
        PacMan3DMovement movementScript = player.GetComponent<PacMan3DMovement>();

        if (movementScript != null)
        {
            // Remove existing listeners to avoid duplication
            buttonUp.onClick.RemoveAllListeners();
            buttonDown.onClick.RemoveAllListeners();
            buttonLeft.onClick.RemoveAllListeners();
            buttonRight.onClick.RemoveAllListeners();

            // Assign button actions
            buttonUp.onClick.AddListener(() => movementScript.MoveUp());
            buttonDown.onClick.AddListener(() => movementScript.MoveDown());
            buttonLeft.onClick.AddListener(() => movementScript.MoveLeft());
            buttonRight.onClick.AddListener(() => movementScript.MoveRight());
        }
        else
        {
            Debug.LogError("PacMan3DMovement script not found on the player.");
        }
    }
}
