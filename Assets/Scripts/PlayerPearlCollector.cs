using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerPearlCollector : MonoBehaviourPun
{
    public int collectedPearls = 0;  // Track the number of collected pearls
    private int totalPearlsToCollect;  // This will be set dynamically

    // Reference to the DecisionMaker script
    private DecisionMaker decisionMaker;

    private void Start()
    {
        // Find all GameObjects tagged as "Ghost" and set the totalPearlsToCollect based on that
        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");
        totalPearlsToCollect = ghosts.Length;  // Number of Ghosts equals the number of pearls to collect

        // Get the reference to the DecisionMaker script in the scene
        decisionMaker = FindObjectOfType<DecisionMaker>();

        Debug.Log("Total pearls to collect: " + totalPearlsToCollect);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (photonView.IsMine)  // Ensure only the local player can collect the pearl
        {
            if (other.CompareTag("Pearl"))  // Assuming the pearl has the tag "Pearl"
            {
                CollectPearl(other.gameObject);
            }
        }
    }

    void CollectPearl(GameObject pearl)
    {
        // Increase the number of collected pearls
        collectedPearls++;

        // Optionally, add some visual/audio feedback
        Debug.Log("Collected a pearl! Total pearls: " + collectedPearls);

        // Get the PhotonView of the pearl
        PhotonView pearlView = pearl.GetComponent<PhotonView>();
        if (pearlView != null)
        {
            // Check if the local player is the owner of the pearl
            if (pearlView.IsMine)
            {
                // If the local player owns the pearl, destroy it
                PhotonNetwork.Destroy(pearl);
            }
            else
            {
                // If the player is the MasterClient, they can request ownership and destroy it
                RequestOwnershipAndDestroy(pearlView);
            }
        }

        // Check if collected pearls reach the totalPearlsToCollect and then notify the DecisionMaker
        if (collectedPearls >= totalPearlsToCollect)
        {
            Debug.Log("Collected all pearls! Protagonist won.");

            // Store a message indicating the Protagonist has won by collecting all pearls
            PlayerPrefs.SetString("GameResult", "The Protagonist has won by collecting all the pearls!");

            // Notify the DecisionMaker to change the scene
            if (decisionMaker != null)
            {
                decisionMaker.OnAllPearlsCollected();
            }
            else
            {
                Debug.LogError("DecisionMaker script not found in the scene.");
            }
        }
    }

    void RequestOwnershipAndDestroy(PhotonView pearlView)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            pearlView.RequestOwnership();
            StartCoroutine(WaitAndDestroy(pearlView));
        }
        else
        {
            pearlView.RequestOwnership();
            StartCoroutine(WaitAndDestroy(pearlView));
        }
    }

    IEnumerator WaitAndDestroy(PhotonView pearlView)
    {
        yield return new WaitForSeconds(0.2f);

        if (pearlView.IsMine)
        {
            PhotonNetwork.Destroy(pearlView.gameObject);
        }
        else
        {
            Debug.LogError("Failed to take ownership of the pearl. Unable to destroy.");
        }
    }
}
