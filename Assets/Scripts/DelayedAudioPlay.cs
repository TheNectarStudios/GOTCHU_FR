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
            StartCoroutine(PlayAudioAfterDelay(6.5f)); // Start the coroutine to play after 7 seconds
        }
        else
        {
            Debug.LogError("AudioSource component not found!");
        }
    }

    private IEnumerator PlayAudioAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the delay time
        audioSource.Play(); // Play the audio
    }
}