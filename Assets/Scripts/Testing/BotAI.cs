using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class BotControl : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float stoppingDistance = 0.5f;
    public string playerTag = "Player";

    private Transform targetPlayer;
    private Rigidbody rb;
    private Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        FindPlayer();

        rb.freezeRotation = true; // Prevent physics-based rotation
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
            Vector3 direction = targetPlayer.position - transform.position;
            float distance = direction.magnitude;

            if (distance > stoppingDistance)
            {
                Vector3 movementDirection = direction.normalized;
                rb.velocity = movementDirection * movementSpeed;
            }
            else
            {
                rb.velocity = Vector3.zero;
            }
        }
    }

    private void UpdateTiltAnimation()
    {
        // Check the movement direction
        Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);

        if (localVelocity.z > 0.1f)
        {
            animator.SetFloat("TiltDirection", 1f); // Forward
        }
        else if (localVelocity.z < -0.1f)
        {
            animator.SetFloat("TiltDirection", -1f); // Backward
        }
        else
        {
            animator.SetFloat("TiltDirection", 0f); // Idle
        }
    }
}
