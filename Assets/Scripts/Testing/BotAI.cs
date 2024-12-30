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
                rb.velocity = movementDirection * movementSpeed; // Apply velocity in both x and z
            }
            else
            {
                rb.velocity = Vector3.zero; // Stop movement when within stopping distance
            }
        }
    }

    private void UpdateTiltAnimation()
    {
        Vector3 velocity = rb.velocity;

        // No movement, reset animations
        if (velocity.magnitude < 0.01f)
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
            animator.SetFloat("TiltDirection", 1f); // Forward tilt
            animator.SetFloat("TiltSide", 0f); // No sideways tilt
        }
        else if (angle >= 90f && angle < 180f)
        {
            // Quadrant 2: Left tilt
            animator.SetFloat("TiltSide", -1f); // Left tilt
            animator.SetFloat("TiltDirection", 0f); // No forward/backward tilt
        }
        else if (angle >= 180f && angle < 270f)
        {
            // Quadrant 3: Backward tilt
            animator.SetFloat("TiltDirection", -1f); // Backward tilt
            animator.SetFloat("TiltSide", 0f); // No sideways tilt
        }
        else if (angle >= 270f && angle < 360f)
        {
            // Quadrant 4: Right tilt
            animator.SetFloat("TiltSide", 1f); // Right tilt
            animator.SetFloat("TiltDirection", 0f); // No forward/backward tilt
        }
    }
}
