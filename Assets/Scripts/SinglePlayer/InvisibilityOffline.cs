using UnityEngine;
using System.Collections;
using TMPro;  // Import TextMeshPro namespace

public class InvisibilityOffline : MonoBehaviour
{
    public GameObject objectToDisable;  // Assign the object to disable in the editor
    public TextMeshProUGUI textToDisable;  // Assign the TextMeshPro to disable in the editor
    public float invisibilityDuration = 5f;  // Duration of invisibility
    public float cooldownTime = 15f;  // Cooldown time between uses

    private bool isInvisible = false;
    private bool isCooldown = false;

    private void Start()
    {
        // Ensure the object to disable is assigned
        if (objectToDisable == null)
        {
            Debug.LogError("Object to disable is not assigned. Please attach a GameObject to the Invisibility script.");
        }

        // Ensure the TextMeshPro field is assigned
        if (textToDisable == null)
        {
            Debug.LogError("TextMeshPro is not assigned. Please attach a TextMeshPro object to the Invisibility script.");
        }
    }

    public void ActivateInvisibility()
    {
        if (isCooldown || objectToDisable == null || textToDisable == null) return;  // Prevent usage during cooldown or if objects not assigned

        StartCoroutine(InvisibilityRoutine());
    }

    private IEnumerator InvisibilityRoutine()
    {
        // Make the player invisible locally
        SetInvisibility(true);

        // Invisibility lasts for a defined duration
        yield return new WaitForSeconds(invisibilityDuration);

        // Make the player visible again
        SetInvisibility(false);

        // Start cooldown
        StartCoroutine(InvisibilityCooldown());
    }

    private void SetInvisibility(bool invisible)
    {
        // Disable or enable the object and the TextMeshPro based on the invisibility state
        objectToDisable.SetActive(!invisible);
        textToDisable.gameObject.SetActive(!invisible);  // Disable or enable the TextMeshPro
    }

    private IEnumerator InvisibilityCooldown()
    {
        isCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        isCooldown = false;
    }
}
