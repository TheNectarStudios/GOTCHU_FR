using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System.Collections; 
public class Dash : MonoBehaviourPun
{
    public float dashSpeed = 10f;
    public float dashDuration = 10f;
    public float cooldownTime = 15f;
    private bool isOnCooldown = false;

    private float originalSpeed;
    public PacMan3DMovement movementScript;
    public Button dashButton;

    void Start()
    {
        if (photonView.IsMine)
        {
            dashButton.onClick.AddListener(ActivateDash);
            dashButton.interactable = true;
            originalSpeed = movementScript.speed;
        }
    }

    public void ActivateDash()
    {
        if (!isOnCooldown)
        {
            StartCoroutine(DashCooldown());
        }
    }

    private IEnumerator DashCooldown()
    {
        isOnCooldown = true;
        dashButton.interactable = false;

        movementScript.speed = dashSpeed;
        yield return new WaitForSeconds(dashDuration);
        movementScript.speed = originalSpeed;

        yield return new WaitForSeconds(cooldownTime);
        isOnCooldown = false;
        dashButton.interactable = true;
    }
}
