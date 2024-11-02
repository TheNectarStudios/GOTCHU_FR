using UnityEngine;
using System.Collections;

public class DisableObjectOffline : MonoBehaviour
{
    public SphereCollider sphereCollider; // Reference to the sphere collider
    private float disableDuration = 20f;  // Duration to disable the collider

    private void Start()
    {
        if (sphereCollider == null)
        {
            Debug.LogError("SphereCollider reference is missing. Please assign it in the inspector.");
            return;
        }

        StartCoroutine(DisableColliderTemporarily());
    }

    private IEnumerator DisableColliderTemporarily()
    {
        sphereCollider.enabled = false;  // Disable the collider
        float timeLeft = disableDuration;

        while (timeLeft > 0)
        {
            // Debug.Log("Time left to re-enable collider: " + timeLeft + " seconds");
            yield return new WaitForSeconds(1f);
            timeLeft--;
        }

        sphereCollider.enabled = true;  // Re-enable the collider
        Debug.Log("Collider re-enabled.");
    }
}
