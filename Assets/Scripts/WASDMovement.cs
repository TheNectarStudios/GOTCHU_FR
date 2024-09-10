using UnityEngine;
using Photon.Pun;
using System.Collections;

public class WASDMovement : MonoBehaviourPun
{
    public float moveSpeed = 5f;  // Speed at which the object moves

    private PhotonView photonView;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();

        // Disable movement for non-local players
        if (!photonView.IsMine)
        {
            enabled = false;
        }
    }

    void Update()
    {
        // Only allow movement if this is the local player's object
        if (photonView.IsMine)
        {
            MovePlayer();
        }
    }

    void MovePlayer()
    {
        // Get the input from the WASD or arrow keys
        float moveHorizontal = Input.GetAxis("Horizontal");  // A/D or Left/Right arrow keys
        float moveVertical = Input.GetAxis("Vertical");      // W/S or Up/Down arrow keys

        // Create a movement vector based on the input
        Vector3 movement = new Vector3(moveHorizontal, 0f, moveVertical);

        // Move the object using the movement vector and speed
        transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);
    }

    
}
