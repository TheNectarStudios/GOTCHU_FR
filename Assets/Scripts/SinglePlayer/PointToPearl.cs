using UnityEngine;

public class PointToPearl : MonoBehaviour
{
    public Transform arrow;           // The arrow object that will point towards the pearl
    public float rotationSpeed = 5f;  // Speed at which the arrow rotates to face the pearl

    private Transform targetPearl;
    private float fixedXRotation = 90f;   // Fixed X rotation
    private float fixedZAdjustment = -90f; // Adjust Z rotation by -90 degrees for correction

    private void Update()
    {
        // Find the closest pearl if there's no target or if the target is destroyed
        if (targetPearl == null || !targetPearl.gameObject.activeInHierarchy)
        {
            FindClosestPearl();
        }

        if (targetPearl != null)
        {
            // Calculate the direction to the pearl
            Vector3 directionToPearl = targetPearl.position - arrow.position;

            // Calculate the rotation needed to look at the pearl
            Quaternion targetRotation = Quaternion.LookRotation(directionToPearl);

            // Apply fixed x-axis rotation (90 degrees) and subtract 90 degrees on the Z-axis for correction
            Quaternion finalRotation = Quaternion.Euler(fixedXRotation, targetRotation.eulerAngles.y, fixedZAdjustment);

            // Smoothly transition to the new rotation
            arrow.rotation = Quaternion.Slerp(arrow.rotation, finalRotation, Time.deltaTime * rotationSpeed);
        }
    }

    private void FindClosestPearl()
    {
        float closestDistance = Mathf.Infinity;
        GameObject[] pearls = GameObject.FindGameObjectsWithTag("Pearl");

        foreach (GameObject pearl in pearls)
        {
            float distance = Vector3.Distance(transform.position, pearl.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                targetPearl = pearl.transform;
            }
        }
    }
}
