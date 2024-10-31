using UnityEngine;
using System.Collections;
using TMPro;  // Import TextMeshPro namespace

public class InvisibilityOffline : MonoBehaviour
{
    public GameObject objectToDisable;            // Assign the object to disable in the editor
    public TextMeshProUGUI textToDisable;         // Assign the TextMeshPro to disable in the editor
    public GameObject invisibilityAnimationPrefab; // Assign the animation prefab in the editor
    public float invisibilityDuration = 5f;       // Duration of invisibility
    public float cooldownTime = 15f;              // Cooldown time between uses

    private bool isInvisible = false;
    private bool isCooldown = false;

    private void Start()
    {
        if (objectToDisable == null)
        {
            Debug.LogError("Object to disable is not assigned. Please attach a GameObject to the Invisibility script.");
        }

        if (textToDisable == null)
        {
            Debug.LogError("TextMeshPro is not assigned. Please attach a TextMeshPro object to the Invisibility script.");
        }

        if (invisibilityAnimationPrefab == null)
        {
            Debug.LogError("Invisibility animation prefab is not assigned. Please assign it in the editor.");
        }
    }

    public void ActivateInvisibility()
    {
        if (isCooldown || isInvisible || objectToDisable == null || textToDisable == null || invisibilityAnimationPrefab == null) return;

        StartCoroutine(InvisibilityRoutine());
    }

    private IEnumerator InvisibilityRoutine()
    {
        PlayInvisibilityAnimation(); // Play animation when becoming invisible
        SetInvisibility(true);

        yield return new WaitForSeconds(invisibilityDuration);

        SetInvisibility(false);
        PlayInvisibilityAnimation(); // Play animation when becoming visible again

        StartCoroutine(InvisibilityCooldown());
    }

    private void SetInvisibility(bool invisible)
    {
        isInvisible = invisible;
        objectToDisable.SetActive(!invisible);
        textToDisable.gameObject.SetActive(!invisible);
    }

    private void PlayInvisibilityAnimation()
    {
        // Instantiate the animation prefab at the object's position and destroy it after 1 second
        GameObject animationInstance = Instantiate(invisibilityAnimationPrefab, transform.position, Quaternion.identity);
        Destroy(animationInstance, 1f); // Ensures the animation plays for 1 second and then stops
    }

    private IEnumerator InvisibilityCooldown()
    {
        isCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        isCooldown = false;
    }
}
