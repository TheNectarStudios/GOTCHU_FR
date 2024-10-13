using System.Collections;
using UnityEngine;

public class DelayedAudioPlay : MonoBehaviour
{
    public AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component attached to the same GameObject
        if (audioSource != null)
        {
            // Each player will individually start the coroutine to play the audio after a delay
            StartCoroutine(PlayAudioAfterDelay(6.5f)); // Start the coroutine to play after a delay
        }
        else
        {
            Debug.LogError("AudioSource component not found!");
        }
    }

    // Coroutine to wait and then play the audio locally for each player
    private IEnumerator PlayAudioAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the delay time

        // Play the audio locally for each player
        PlayAudio();
    }

    // Method to play audio locally
    private void PlayAudio()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play(); // Play the audio
            Debug.Log("Audio played locally for this player.");
        }
    }
}
