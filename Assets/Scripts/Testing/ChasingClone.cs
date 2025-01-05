using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CloneBot : MonoBehaviour
{
    public string playerTag = "Player"; // Tag to identify the player
    public float chaseRadius = 15f; // Radius within which the clone will chase the player
    public float updateFrequency = 0.2f; // How often the clone updates its destination
    public float tiltSensitivity = 2f; // Sensitivity for animations

    private NavMeshAgent navMeshAgent;
    private Transform player;
    private bool isChasing = false;
    private Animator animator;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

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
            UpdateTiltAnimation();
        }
        else
        {
            navMeshAgent.ResetPath(); // Stop moving if the player is out of range
        }
    }

    private void UpdateTiltAnimation()
    {
        if (animator == null) return;

        Vector3 velocity = navMeshAgent.velocity;

        if (velocity.magnitude < 0.1f)
        {
            animator.SetFloat("TiltSide", 0f);
            animator.SetFloat("TiltDirection", 0f);
            return;
        }

        float angle = Mathf.Atan2(velocity.z, velocity.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;

        if (angle >= 0f && angle < 90f)
        {
            animator.SetFloat("TiltDirection", 1f * tiltSensitivity);
            animator.SetFloat("TiltSide", 0f);
        }
        else if (angle >= 90f && angle < 180f)
        {
            animator.SetFloat("TiltSide", -1f * tiltSensitivity);
            animator.SetFloat("TiltDirection", 0f);
        }
        else if (angle >= 180f && angle < 270f)
        {
            animator.SetFloat("TiltDirection", -1f * tiltSensitivity);
            animator.SetFloat("TiltSide", 0f);
        }
        else if (angle >= 270f && angle < 360f)
        {
            animator.SetFloat("TiltSide", 1f * tiltSensitivity);
            animator.SetFloat("TiltDirection", 0f);
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
