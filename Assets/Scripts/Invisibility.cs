using UnityEngine;
using Photon.Pun;
using System.Collections;
using TMPro;  // Import TextMeshPro namespace

public class Invisibility : MonoBehaviourPun
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
        // Make the player invisible for others but not for themselves
        if (photonView.IsMine)
        {
            // Make the player invisible to others
            photonView.RPC("SetInvisibleForOthers", RpcTarget.Others, true);
        }

        // Disable the assigned object and TextMeshPro for others but keep them visible for the player themselves
        SetInvisibility(true);

        // Invisibility lasts for a defined duration
        yield return new WaitForSeconds(invisibilityDuration);

        // Make the player visible again
        if (photonView.IsMine)
        {
            // Notify others that the player is now visible
            photonView.RPC("SetInvisibleForOthers", RpcTarget.Others, false);
        }

        SetInvisibility(false);

        // Start cooldown
        StartCoroutine(InvisibilityCooldown());
    }

    [PunRPC]
    private void SetInvisibleForOthers(bool invisible)
    {
        // Disable or enable the object and the TextMeshPro based on the invisibility state
        objectToDisable.SetActive(!invisible);
        textToDisable.gameObject.SetActive(!invisible);  // Disable or enable the TextMeshPro
    }

    private void SetInvisibility(bool invisible)
    {
        if (photonView.IsMine)
        {
            // Keep the object and TextMeshPro active for the player themselves
            objectToDisable.SetActive(true);
            textToDisable.gameObject.SetActive(true);
        }
        else
        {
            // Set visibility for other players
            objectToDisable.SetActive(!invisible);
            textToDisable.gameObject.SetActive(!invisible);
        }
    }

    private IEnumerator InvisibilityCooldown()
    {
        isCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        isCooldown = false;
    }
}
