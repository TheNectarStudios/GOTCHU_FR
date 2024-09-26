using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ShaderManager : MonoBehaviour
{
    public Material freezeEffectMaterial;  // Reference to the freeze material used in URP
    public float invisibleValue = 0.0f;     // Represents invisible
    public float visibleValue = 1.57f;       // Represents visible

    private void Start()
    {
        SetTilingMultiplier(freezeEffectMaterial, invisibleValue);
    }

    private void Update()
    {
        // Test with keyboard input
        if (Input.GetKeyDown(KeyCode.F)) // Press F to apply freeze effect for ghosts
        {
            ApplyFreezeEffectForAntagonists();
        }

        if (Input.GetKeyDown(KeyCode.P)) // Press P to reset effect
        {
            ResetFreezeEffect();
        }
    }

    // Applies the freeze effect for non-antagonist players tagged as "Ghost"
    [PunRPC]
    public void ApplyFreezeEffectForAntagonists()
    {
        bool isLocalPlayerAntagonist = PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("IsAntagonist", out object isAntagonist) && (bool)isAntagonist;

        if (!isLocalPlayerAntagonist) // Only apply freeze effect for non-antagonists (ghosts)
        {
            Debug.Log("Applying freeze effect for ghosts");

            // Find all objects tagged as "Ghost"
            GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");

            foreach (GameObject ghost in ghosts)
            {
                // Use ShaderManager to apply the freeze effect
                SetTilingMultiplier(freezeEffectMaterial, visibleValue); // Apply visible value to the full-screen shader
                Debug.Log("Freeze effect applied to Ghost: " + ghost.name);
            }

            // Optionally, reset the effect back to invisible after 5 seconds
            StartCoroutine(ResetFreezeEffectAfterDelay(5.0f));
        }
    }

    // Resets the freeze effect for all ghosts
    public void ResetFreezeEffect()
    {
        SetTilingMultiplier(freezeEffectMaterial, invisibleValue); // Reset to invisible
        Debug.Log("Resetting freeze effect to invisible");
    }

    // Coroutine to reset the freeze effect after a delay
    private IEnumerator ResetFreezeEffectAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SetTilingMultiplier(freezeEffectMaterial, invisibleValue); // Reset to invisible
        Debug.Log("Resetting freeze effect to invisible after delay");
    }

    // Sets the value of the _tillingMultiplier property
    public void SetTilingMultiplier(Material material, float value)
    {
        if (material != null)
        {
            if (material.HasProperty("_tillingMultiplier"))
            {
                material.SetFloat("_tillingMultiplier", value); // Set the value of _tillingMultiplier
            }
            else
            {
                Debug.LogError("Material does not have _tillingMultiplier property.");
            }
        }
    }
}
