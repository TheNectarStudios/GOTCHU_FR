using UnityEngine;
using Photon.Pun;

public class Invisibility : MonoBehaviourPun
{
    public float invisibilityDuration = 5f;  // Duration of invisibility
    private bool isInvisible = false;

    
    public void ActivateInvisibility()
    {
        if (photonView.IsMine)
        {
            // Find the hero player and make the ghost invisible only for that player
            GameObject heroPlayer = FindHeroPlayer();
            if (heroPlayer != null)
            {
                photonView.RPC("RPC_SetInvisibilityForHero", RpcTarget.All, heroPlayer.GetComponent<PhotonView>().ViewID, false);
                Invoke("DeactivateInvisibility", invisibilityDuration);
            }
        }
    }

    void DeactivateInvisibility()
    {
        if (photonView.IsMine)
        {
            // Find the hero player and make the ghost visible again for that player
            GameObject heroPlayer = GameObject.FindGameObjectWithTag("Player");
            if (heroPlayer != null)
            {
                photonView.RPC("RPC_SetInvisibilityForHero", RpcTarget.All, heroPlayer.GetComponent<PhotonView>().ViewID, true);
            }
        }
    }

    [PunRPC]
    void RPC_SetInvisibilityForHero(int heroViewID, bool isVisible)
    {
        GameObject heroPlayer = PhotonView.Find(heroViewID).gameObject;
        if (heroPlayer != null && heroPlayer.CompareTag("Player"))
        {
            if (photonView.IsMine)
            {
                GetComponent<Renderer>().enabled = isVisible;
            }
        }
    }

    GameObject FindHeroPlayer()
    {
        return GameObject.FindGameObjectWithTag("Player");
    }
}
