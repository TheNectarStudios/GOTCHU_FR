using UnityEngine;

public class UpDownRevolve : MonoBehaviour
{
    public float moveSpeed = 2f;        // Speed of vertical movement
    public float moveHeight = 0.7f;       // Height of vertical movement
    public float rotateSpeed = 50f;     // Speed of rotation around Z-axis

    private Vector3 startPos;           // Store the initial position of the object

    void Start()
    {
        // Store the starting position of the object
        startPos = transform.position;
    }

    void Update()
    {
        // Move the object up and down along the Y-axis
        float newY = startPos.y + Mathf.Sin(Time.time * moveSpeed) * moveHeight;
        transform.position = new Vector3(startPos.x, newY, startPos.z);

        // Rotate the object around the Z-axis
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }
}
