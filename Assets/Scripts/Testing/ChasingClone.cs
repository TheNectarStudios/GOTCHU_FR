using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CloneBot : MonoBehaviour
{
    public string playerTag = "Player"; // Tag to identify the player
    public float chaseRadius = 15f; // Radius within which the clone will chase the player
    public float updateFrequency = 0.2f; // How often the clone updates its destination

    private NavMeshAgent navMeshAgent;
    private Transform player;
    private bool isChasing = false;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        FindPlayer();

        if (player != null)
        {
            isChasing = true;
            StartChasing();
        }
        else
        {
            Debug.LogWarning("No player found with the specified tag. Clone will not chase.");
        }
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    private void StartChasing()
    {
        if (!isChasing) return;

        InvokeRepeating(nameof(UpdateChase), 0f, updateFrequency);
    }

    private void UpdateChase()
    {
        if (player == null || navMeshAgent == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= chaseRadius)
        {
            navMeshAgent.SetDestination(player.position);
        }
        else
        {
            navMeshAgent.ResetPath(); // Stop moving if the player is out of range
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);
    }

    private void OnDestroy()
    {
        CancelInvoke(nameof(UpdateChase)); // Clean up when the clone is destroyed
    }
}
