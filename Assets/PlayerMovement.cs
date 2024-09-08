using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    public float speed = 5f;
    private Vector3 movement;

    void Update()
    {
        if (!IsOwner) return;  // Only control the local player

        // Get input from WASD or Arrow Keys
        movement.x = Input.GetAxisRaw("Horizontal");  // A/D or Left/Right Arrow for x-axis
        movement.z = Input.GetAxisRaw("Vertical");    // W/S or Up/Down Arrow for z-axis

        // Move the player on the local client
        MovePlayerServerRpc(movement * speed * Time.deltaTime);
    }

    [ServerRpc]
    private void MovePlayerServerRpc(Vector3 moveDelta)
    {
        // Update player position on the server and all clients
        transform.Translate(moveDelta, Space.World);
    }
}
