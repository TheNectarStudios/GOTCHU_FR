using System.Collections;
using UnityEngine;

public class ShaderManagerOffline : MonoBehaviour
{
    public Material freezeEffectMaterial;  // Reference to the freeze material used in URP
    public float invisibleValue = 0.0f;    // Represents invisible
    public float visibleValue = 1.57f;     // Represents visible
    public float transitionDuration = 5.0f; // Duration for the effect to transition back to invisible

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
    }

    private void Update()
    {
        if (freezeEffectMaterial == null)
        {
            Debug.LogError("FreezeEffectMaterial is still null in Update method!");
            return;
        }

        // Press F to apply freeze effect for ghosts
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("F key pressed: Applying freeze effect...");
            ApplyFreezeEffect();
        }

        // Press P to reset freeze effect
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("P key pressed: Resetting freeze effect...");
            ResetFreezeEffect();
        }
    }

    public void ApplyFreezeEffect()
    {
        Debug.Log("Applying freeze effect for ghosts");

        // Start the transition to make the effect visible and then revert
        StartCoroutine(TransitionTilingMultiplier(visibleValue, invisibleValue, transitionDuration));
    }

    public void ResetFreezeEffect()
    {
        // Reset the effect immediately
        StopAllCoroutines();  // Stop any ongoing transition
        SetTilingMultiplier(freezeEffectMaterial, invisibleValue); // Set directly to invisible
        Debug.Log("Resetting freeze effect to invisible");
    }

    // Coroutine to smoothly transition the tilingMultiplier value
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

    // Method to set the tilingMultiplier on the material
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
