using UnityEngine;

public class dronerotation : MonoBehaviour
{
    public float rotationSpeed = 100f;  // Speed of rotation

    void Update()
    {
        // Rotate the object around the Y axis
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}
