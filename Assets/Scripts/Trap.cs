using UnityEngine;
using Photon.Pun;
using System.Collections;
using UnityEngine.UI;
using System.Collections; 

public class Trap : MonoBehaviourPun
{
    public GameObject trapPrefab;
    public float trapLifetime = 15f;
    public float cooldownTime = 15f;
    private bool isOnCooldown = false;

    public Button trapButton;

    void Start()
    {
        if (photonView.IsMine)
        {
            trapButton.onClick.AddListener(PlaceTrap);
            trapButton.interactable = true;
        }
    }

    public void PlaceTrap()
    {
        if (!isOnCooldown)
        {
            Vector3 spawnPosition = transform.position - transform.forward * 1f;
            GameObject trap = PhotonNetwork.Instantiate(trapPrefab.name, spawnPosition, Quaternion.identity);
            StartCoroutine(TrapCooldown(trap));
        }
    }

    private IEnumerator TrapCooldown(GameObject trap)
    {
        isOnCooldown = true;
        trapButton.interactable = false;

        yield return new WaitForSeconds(trapLifetime);
        PhotonNetwork.Destroy(trap);

        yield return new WaitForSeconds(cooldownTime);
        isOnCooldown = false;
        trapButton.interactable = true;
    }
}
