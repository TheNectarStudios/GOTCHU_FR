using UnityEngine;

public class CheeringAnimation : MonoBehaviour
{
    public Animator animator;

    [SerializeField] private float cheeringDuration = 7f; // Duration of cheering (in seconds)
    [SerializeField] private float idleDuration = 3f;     // Duration of idle (in seconds)

    private bool IsCheering = false;

    private void Start()
    {
        // Initially set the character to Idle
        animator.SetBool("IsCheering", false);

        // Start the cycle of cheering and idling
        StartCoroutine(CheeringCycle());
    }

    private System.Collections.IEnumerator CheeringCycle()
    {
        while (true) // Infinite loop to keep the cycle running
        {
            // Set IsCheering to true, start the cheering animation
            IsCheering = true;
            animator.SetBool("IsCheering", true);
            // Debug.Log("Cheering for " + cheeringDuration + " seconds...");

            // Wait for the specified cheering duration
            yield return new WaitForSeconds(cheeringDuration);

            // Set IsCheering to false, go back to idle
            IsCheering = false;
            animator.SetBool("IsCheering", false);
            // Debug.Log("Idling for " + idleDuration + " seconds...");

            // Wait for the specified idle duration
            yield return new WaitForSeconds(idleDuration);
        }
    }
}
