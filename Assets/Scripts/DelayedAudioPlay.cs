using System.Collections;
using UnityEngine;
using Photon.Pun; // Photon support

public class DelayedAudioPlay : MonoBehaviourPun
{
    public AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component attached to the same GameObject
        if (audioSource != null)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                // Only the MasterClient should start the coroutine, then synchronize via RPC
                StartCoroutine(PlayAudioAfterDelay(6.5f)); // Start the coroutine to play after a delay
            }
        }
        else
        {
            Debug.LogError("AudioSource component not found!");
        }
    }

    // Coroutine to wait and then synchronize the audio playback across the network
    private IEnumerator PlayAudioAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the delay time

        // Call the RPC to play the audio on all clients
        photonView.RPC("PlayAudioAcrossNetwork", RpcTarget.All);
    }

    // RPC method that will be called across the network to play the audio
    [PunRPC]
    private void PlayAudioAcrossNetwork()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play(); // Play the audio
            Debug.Log("Audio played across the network.");
        }
    }
}
