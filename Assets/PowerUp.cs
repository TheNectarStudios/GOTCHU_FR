using UnityEngine;

public class PowerUp : MonoBehaviour
{
    // Define an event to notify when the power-up is picked up
    public delegate void PowerUpPickedUp();
    public event PowerUpPickedUp onPickedUp;

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the player has collided with the power-up
        if (collision.gameObject.CompareTag("Player"))  
        {
            // Trigger the picked-up event
            Debug.Log("Power-up picked up!");
            onPickedUp?.Invoke();
            // Hide or disable the power-up
            GetComponent<Renderer>().enabled = false;
            GetComponent<Collider>().enabled = false;
            // Notify the spawner
            Destroy(gameObject); // Remove the power-up from the scene
        }
    }
}
