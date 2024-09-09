using UnityEngine;
using System.Collections;

public class Trap : MonoBehaviour
{
    public float slowDownFactor = 0.5f;
    public float trapDuration = 10f;
    public float slowDownDuration = 3f;
    private WASDMovement playerMovement;


    void Start()
    {
        Destroy(gameObject, trapDuration);
    }

    
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.name =="Prot") {
            playerMovement = collision.gameObject.GetComponent<WASDMovement>();
            ActivatePowerUp();
            GetComponent<Renderer>().enabled = false;
            GetComponent<Collider>().enabled = false;
        }
        
    }
    public void ActivatePowerUp()
    {
        StartCoroutine(ApplySlowDown());
    }

    private IEnumerator ApplySlowDown()
    {
        playerMovement.moveSpeed *= slowDownFactor;
        yield return new WaitForSeconds(slowDownDuration);
        playerMovement.moveSpeed /= slowDownFactor;
        Destroy(gameObject);
    }
}
