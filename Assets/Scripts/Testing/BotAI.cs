using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class BotControl : MonoBehaviour
{
    public float stoppingDistance = 0.5f;
    public string playerTag = "Player";
    public float tiltSensitivity = 2f; // Sensitivity for animations

    private Transform targetPlayer;
    private NavMeshAgent navMeshAgent;
    private Animator animator;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        FindPlayer();

        navMeshAgent.stoppingDistance = stoppingDistance; // Set stopping distance for NavMeshAgent
    }

    private void Update()
    {
        MoveTowardsTarget();
        UpdateTiltAnimation();
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObject != null)
        {
            targetPlayer = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("No player found with the specified tag.");
        }
    }

    private void MoveTowardsTarget()
    {
        if (targetPlayer != null)
        {
            navMeshAgent.SetDestination(targetPlayer.position); // Navigate toward the player
        }
    }

    private void UpdateTiltAnimation()
    {
        Vector3 velocity = navMeshAgent.velocity;

        // No movement, reset animations
        if (velocity.magnitude < 0.1f)
        {
            animator.SetFloat("TiltSide", 0f);
            animator.SetFloat("TiltDirection", 0f);
            return;
        }

        // Calculate the angle in the XZ plane
        float angle = Mathf.Atan2(velocity.z, velocity.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f; // Ensure angle is in the range [0, 360]

        // Determine the quadrant and apply the corresponding tilt
        if (angle >= 0f && angle < 90f)
        {
            // Quadrant 1: Forward tilt
            animator.SetFloat("TiltDirection", 1f * tiltSensitivity);
            animator.SetFloat("TiltSide", 0f);
        }
        else if (angle >= 90f && angle < 180f)
        {
            // Quadrant 2: Left tilt
            animator.SetFloat("TiltSide", -1f * tiltSensitivity);
            animator.SetFloat("TiltDirection", 0f);
        }
        else if (angle >= 180f && angle < 270f)
        {
            // Quadrant 3: Backward tilt
            animator.SetFloat("TiltDirection", -1f * tiltSensitivity);
            animator.SetFloat("TiltSide", 0f);
        }
        else if (angle >= 270f && angle < 360f)
        {
            // Quadrant 4: Right tilt
            animator.SetFloat("TiltSide", 1f * tiltSensitivity);
            animator.SetFloat("TiltDirection", 0f);
        }
    }
}
