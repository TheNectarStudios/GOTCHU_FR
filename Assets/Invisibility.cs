using UnityEngine;
using Photon.Pun;
using System.Collections;

public class Invisibility : MonoBehaviourPun
{
    public Renderer playerRenderer;  // Assign the player's renderer
    public float invisibilityDuration = 5f;  // Duration of invisibility
    public float cooldownTime = 15f;  // Cooldown time between uses

    private bool isInvisible = false;
    private bool isCooldown = false;

    private void Start()
    {
        // Ensure the playerRenderer is assigned, otherwise try to get it from the object
        if (playerRenderer == null)
        {
            playerRenderer = GetComponentInChildren<Renderer>();
        }

        if (playerRenderer == null)
        {
            Debug.LogError("Renderer not found. Please assign a Renderer to the Invisibility script.");
        }
    }

    public void ActivateInvisibility()
    {
        if (isCooldown || playerRenderer == null) return;  // Prevent usage during cooldown

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

        // Disable renderer for others but keep it visible for the player themselves
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
        playerRenderer.enabled = !invisible;
    }

    private void SetInvisibility(bool invisible)
    {
        if (photonView.IsMine)
        {
            // Keep the renderer active for the player themselves
            playerRenderer.enabled = true;
        }
        else
        {
            // Set visibility for other players
            playerRenderer.enabled = !invisible;
        }
    }

    private IEnumerator InvisibilityCooldown()
    {
        isCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        isCooldown = false;
    }
}
