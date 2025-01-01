using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ChasingClone : MonoBehaviour
{
    public float lifetime = 10f; // How long the clone chases the player
    public float stoppingDistance = 1f; // How close the clone gets to the player
    public string playerTag = "Player"; // Tag for the player

    private Transform targetPlayer;
    private NavMeshAgent navMeshAgent;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (navMeshAgent == null)
        {
            Debug.LogError("[ChasingClone] NavMeshAgent component is missing!");
            return;
        }

        navMeshAgent.stoppingDistance = stoppingDistance;

        // Find the player
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObject != null)
        {
            targetPlayer = playerObject.transform;
            Debug.Log("[ChasingClone] Player found: " + playerObject.name);
        }
        else
        {
            Debug.LogWarning("[ChasingClone] No player found with the specified tag.");
        }

        // Destroy the clone after its lifetime
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (targetPlayer != null)
        {
            if (navMeshAgent.isOnNavMesh)
            {
                navMeshAgent.SetDestination(targetPlayer.position);
                Debug.Log("[ChasingClone] Chasing player at position: " + targetPlayer.position);
            }
            else
            {
                Debug.LogError("[ChasingClone] NavMeshAgent is not on the NavMesh.");
            }
        }
        else
        {
            Debug.LogWarning("[ChasingClone] TargetPlayer is null.");
        }

        // Log NavMeshAgent state
        Debug.Log("[ChasingClone] Velocity: " + navMeshAgent.velocity.magnitude);
    }
}
