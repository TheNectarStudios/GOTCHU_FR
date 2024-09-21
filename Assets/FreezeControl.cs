using System.Collections;
using UnityEngine;
using Photon.Pun;

public class FreezeControl : MonoBehaviourPun
{
    public Material freezeMaterial;
    public float freezeDuration = 5f;
    public float tillingMultiplier = 1.56f;

    private bool isFrozen = false;
    private Material originalMaterial;
    private float initialtilling;

    private void Start()
    {
        if (freezeMaterial != null)
        {
            initialtilling = freezeMaterial.GetFloat("_tillingMultiplier");
        }
    }

    public void FreezeAntagonists()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("FreezeAntagonistsRPC", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    private void FreezeAntagonistsRPC()
    {
        if (!isFrozen)
        {
            GameObject[] antagonists = GameObject.FindGameObjectsWithTag("Ghost");

            foreach (GameObject antagonist in antagonists)
            {
                PhotonView pv = antagonist.GetComponent<PhotonView>();
                if (pv != null && pv.IsMine)
                {
                    StartCoroutine(ApplyFreezeEffect(antagonist));
                }
            }
        }
    }

    private IEnumerator ApplyFreezeEffect(GameObject antagonist)
    {
        isFrozen = true;
        
        // Get the antagonist's renderer and original material
        Renderer renderer = antagonist.GetComponent<Renderer>();
        if (renderer != null)
        {
            originalMaterial = renderer.material;
            renderer.material = freezeMaterial;
            freezeMaterial.SetFloat("_tillingMultiplier", 0f);

            // Animate the tilling multiplier from 0 to 1.56
            float elapsedTime = 0f;
            while (elapsedTime < freezeDuration)
            {
                float t = elapsedTime / freezeDuration;
                freezeMaterial.SetFloat("_tillingMultiplier", Mathf.Lerp(0f, tillingMultiplier, t));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            // Freeze the antagonist movement
            PacMan3DMovement antagonistMovement = antagonist.GetComponent<PacMan3DMovement>();
            if (antagonistMovement != null)
            {
                antagonistMovement.enabled = false;
            }

            yield return new WaitForSeconds(freezeDuration);

            // Re-enable movement and revert the material
            if (antagonistMovement != null)
            {
                antagonistMovement.enabled = true;
            }

            freezeMaterial.SetFloat("_tillingMultiplier", 0f);
            renderer.material = originalMaterial;
        }

        isFrozen = false;
    }
}
