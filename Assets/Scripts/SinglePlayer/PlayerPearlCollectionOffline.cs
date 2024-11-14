using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;  // Required for scene management

public class PlayerPearlCollectonOffline : MonoBehaviour
{
    public int collectedPearls = 0;  // Track the number of collected pearls
    private int totalPearlsToCollect;  // This will be set dynamically

    private void Start()
    {
        // Find all GameObjects tagged as "Ghost" and set the totalPearlsToCollect based on that
        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");
        totalPearlsToCollect = ghosts.Length;  // Number of Ghosts equals the number of pearls to collect

        Debug.Log("Total pearls to collect: " + totalPearlsToCollect);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pearl"))  // Assuming the pearl has the tag "Pearl"
        {
            CollectPearl(other.gameObject);
        }
    }

    void CollectPearl(GameObject pearl)
    {
        // Increase the number of collected pearls
        collectedPearls++;

        // Optionally, add some visual/audio feedback
        Debug.Log("Collected a pearl! Total pearls: " + collectedPearls);

        // Destroy the pearl locally
        Destroy(pearl);

        // Check if collected pearls reach the totalPearlsToCollect and then change the scene
        if (collectedPearls >= totalPearlsToCollect)
        {
            Debug.Log("Collected all pearls! Protagonist won.");
            
            // Store a message indicating the Protagonist has won by collecting all pearls
            PlayerPrefs.SetString("GameResult", "The Protagonist has won by collecting all the pearls!");

            // Transition to the next scene for all players
            ChangeSceneForAllPlayers();
        }
    }

    void ChangeSceneForAllPlayers()
    {
        // Load the ResultScene for all players
        Debug.Log("Loading ResultScene...");
        SceneManager.LoadScene("ResultSceneOffline");
    }
}
