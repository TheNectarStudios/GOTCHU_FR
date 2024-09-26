using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderManager : MonoBehaviour
{
    public Material freezeEffectMaterial;  // Reference to the freeze material used in URP
    public float visibleOpacity = 1.0f;     // Fully visible
    public float invisibleOpacity = 0.0f;   // Fully invisible

    private void Start()
    {
        // Set the initial opacity of the freeze effect material to 0 (invisible)
        SetMaterialOpacity(freezeEffectMaterial, invisibleOpacity);
    }

    // Apply the freeze effect by adjusting the opacity of the material for all antagonists (Ghosts)
    public void SetFreezeEffectForAntagonists(bool isVisible)
    {
        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");
        foreach (GameObject ghost in ghosts)
        {
            Renderer renderer = ghost.GetComponent<Renderer>();
            if (renderer != null)
            {
                AdjustMaterialOpacity(renderer, isVisible);
            }
        }
    }

    // Apply the freeze effect by adjusting the opacity of the material for the protagonist (Player)
    public void SetFreezeEffectForProtagonist(bool isVisible)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Renderer renderer = player.GetComponent<Renderer>();
            if (renderer != null)
            {
                AdjustMaterialOpacity(renderer, isVisible);
            }
        }
    }

    // Sets the opacity of the specified material
    private void SetMaterialOpacity(Material material, float opacity)
    {
        if (material != null)
        {
            if (material.HasProperty("_BaseColor"))  // URP uses _BaseColor for HDRP and URP shaders
            {
                Color color = material.GetColor("_BaseColor");
                color.a = opacity;  // Set the alpha value
                material.SetColor("_BaseColor", color);
            }
            else if (material.HasProperty("_Color"))  // Standard shader might use _Color
            {
                Color color = material.GetColor("_Color");
                color.a = opacity;  // Set the alpha value
                material.SetColor("_Color", color);
            }
            else
            {
                Debug.LogError("Material does not have _BaseColor or _Color property for opacity control.");
            }
        }
    }

    // Adjusts the opacity of the material
    private void AdjustMaterialOpacity(Renderer renderer, bool isVisible)
    {
        Material material = renderer.material;
        if (material.HasProperty("_BaseColor"))  // URP uses _BaseColor for HDRP and URP shaders
        {
            Color color = material.GetColor("_BaseColor");
            color.a = isVisible ? visibleOpacity : invisibleOpacity;  // Adjust the alpha value
            material.SetColor("_BaseColor", color);
        }
        else if (material.HasProperty("_Color"))  // Standard shader might use _Color
        {
            Color color = material.GetColor("_Color");
            color.a = isVisible ? visibleOpacity : invisibleOpacity;  // Adjust the alpha value
            material.SetColor("_Color", color);
        }
        else
        {
            Debug.LogError("Material does not have _BaseColor or _Color property for opacity control.");
        }
    }
}
