using UnityEngine;
using System.Collections;
public class SpeedBoost : MonoBehaviour
{
    public float speedMultiplier = 1.5f;
    public float duration = 5f;
    private PacMan3DMovement playerMovement;


    private void OnTriggerEnter(Collider collision)
    {
        playerMovement = collision.gameObject.GetComponent<PacMan3DMovement>();
        ActivatePowerUp();

    }
    public void ActivatePowerUp()
    {
        StartCoroutine(SpeedBoostRoutine());
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
    }

    private IEnumerator SpeedBoostRoutine()
    {
        playerMovement.speed *= speedMultiplier;
        yield return new WaitForSeconds(duration);
        playerMovement.speed /= speedMultiplier;
        Destroy(gameObject);
    }
}
