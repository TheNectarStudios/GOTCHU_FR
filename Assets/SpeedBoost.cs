using UnityEngine;
using System.Collections;
public class SpeedBoost : MonoBehaviour
{
    public float speedMultiplier = 1.5f;
    public float duration = 5f;
    private WASDMovement playerMovement;


    private void OnCollisionEnter(Collision collision)
    {
        playerMovement=collision.gameObject.GetComponent<WASDMovement>();
        ActivatePowerUp();
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

    }
    public void ActivatePowerUp()
    {
        StartCoroutine(SpeedBoostRoutine());
    }

    private IEnumerator SpeedBoostRoutine()
    {
        playerMovement.moveSpeed *= speedMultiplier;
        yield return new WaitForSeconds(duration);
        playerMovement.moveSpeed /= speedMultiplier;
        Destroy(gameObject);
    }
}
