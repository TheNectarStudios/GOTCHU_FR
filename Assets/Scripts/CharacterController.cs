using UnityEngine;
using System.Collections;

public class CharacterControllerWithCheer : MonoBehaviour
{
    public Transform targetPoint;  // The point to reach
    public float speed = 5f;       // Speed of the character
    public Animator animator;      // Reference to the Animator component
    private bool hasReachedTarget = false;
    
    public bool IsRunning = true;  // Bool to control running animation
    public bool IsCheering = false; // Bool to control cheering animation

    private bool bufferCompleted = false; // Track if the buffer period is completed

    void Start()
    {
        // Ensure no animations are running by default during buffer
        animator.enabled = false;

        // Start the coroutine to wait for 7 seconds before enabling movement and animation
        StartCoroutine(StartAfterBuffer(6f));  // 7-second buffer
    }

    IEnumerator StartAfterBuffer(float waitTime)
    {
        yield return new WaitForSeconds(waitTime); // Wait for the buffer time
        bufferCompleted = true;                   // After 7 seconds, logic can start
        animator.enabled = true;                  // Enable animations after buffer
    }

    void Update()
    {
        // Only run the movement and animation logic if the buffer is completed
        if (bufferCompleted)
        {
            if (!hasReachedTarget)
            {
                // Move towards the target
                float step = speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, step);

                // Check if the character has reached the target
                if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
                {
                    hasReachedTarget = true;  // Stop moving
                    IsRunning = false;        // Stop running animation
                    IsCheering = true;        // Start cheering animation
                }
            }

            // Update animator parameters
            animator.SetBool("IsRunning", IsRunning);

            // Only set IsCheering true if hasReachedTarget is true
            if (hasReachedTarget)
            {
                animator.SetBool("IsCheering", IsCheering);
            }
        }
    }
}
