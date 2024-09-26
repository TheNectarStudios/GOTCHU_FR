using System.Collections;
using UnityEngine;
using Photon.Pun;

public class ShaderManager : MonoBehaviour
{
    public Material freezeEffectMaterial;  // Reference to the freeze material used in URP
    public float invisibleValue = 0.0f;    // Represents invisible
    public float visibleValue = 1.57f;     // Represents visible
    public float transitionDuration = 5.0f; // Duration for the effect to transition back to invisible

    private PhotonView photonView;         // PhotonView reference

    private void Start()
    {
        if (freezeEffectMaterial == null)
        {
            Debug.LogError("FreezeEffectMaterial is null in ShaderManager! Make sure the material is assigned in the inspector.");
        }
        else
        {
            Debug.Log("FreezeEffectMaterial assigned successfully.");
            SetTilingMultiplier(freezeEffectMaterial, invisibleValue); // Initial state: invisible
        }

        photonView = GetComponent<PhotonView>();
        if (photonView == null)
        {
            Debug.LogError("PhotonView component is missing from the GameObject. Add a PhotonView component to the GameObject.");
        }
    }

    private void Update()
    {
        if (freezeEffectMaterial == null)
        {
            Debug.LogError("FreezeEffectMaterial is still null in Update method!");
            return;
        }

        if (photonView == null)
        {
            Debug.LogError("PhotonView is null in ShaderManager Update. Cannot call RPC.");
            return;
        }

        // Press F to apply freeze effect for ghosts
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("F key pressed: Applying freeze effect...");
            photonView.RPC("ApplyFreezeEffectForAntagonists", RpcTarget.AllBuffered);
        }

        // Press P to reset freeze effect
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("P key pressed: Resetting freeze effect...");
            photonView.RPC("ResetFreezeEffect", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    public void ApplyFreezeEffectForAntagonists()
    {
        bool isLocalPlayerAntagonist = PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("IsAntagonist", out object isAntagonist) && (bool)isAntagonist;

        if (!isLocalPlayerAntagonist) // Only apply freeze effect for non-antagonists (ghosts)
        {
            Debug.Log("Applying freeze effect for ghosts");

            // Start the transition to make the effect visible and then revert
            StartCoroutine(TransitionTilingMultiplier(visibleValue, invisibleValue, transitionDuration));
        }
    }

    [PunRPC]
    public void ResetFreezeEffect()
    {
        // Reset the effect immediately
        StopAllCoroutines();  // Stop any ongoing transition
        SetTilingMultiplier(freezeEffectMaterial, invisibleValue); // Set directly to invisible
        Debug.Log("Resetting freeze effect to invisible");
    }

    // Coroutine to smoothly transition the tillingMultiplier value
    private IEnumerator TransitionTilingMultiplier(float fromValue, float toValue, float duration)
    {
        float timeElapsed = 0f;
        while (timeElapsed < duration)
        {
            // Gradually change the value over time
            float currentValue = Mathf.Lerp(fromValue, toValue, timeElapsed / duration);
            SetTilingMultiplier(freezeEffectMaterial, currentValue);

            timeElapsed += Time.deltaTime;  // Increment elapsed time
            yield return null;              // Wait for the next frame
        }

        // Ensure the final value is set to 'toValue' after the loop finishes
        SetTilingMultiplier(freezeEffectMaterial, toValue);
        Debug.Log("Transition complete. Final tiling multiplier set to: " + toValue);
    }

    // Method to set the tillingMultiplier on the material
    public void SetTilingMultiplier(Material material, float value)
    {
        if (material != null)
        {
            if (material.HasProperty("_tillingMultiplier"))
            {
                material.SetFloat("_tillingMultiplier", value); // Set the value of _tillingMultiplier
                Debug.Log($"_tillingMultiplier set to {value}");
            }
            else
            {
                Debug.LogError("Material does not have _tillingMultiplier property.");
            }
        }
        else
        {
            Debug.LogError("Material is null when trying to set _tillingMultiplier.");
        }
    }
}
