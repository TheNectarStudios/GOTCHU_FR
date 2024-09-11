using UnityEngine;
using System.Collections;

public class Trap : MonoBehaviour
{
    public float slowDownFactor = 0.5f;
    public float trapDuration = 10f;
    public float slowDownDuration = 3f;
    private PacMan3DMovement playerMovement;


    void Start()
    {
        Destroy(gameObject, trapDuration);
    }

    
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.name =="Hero(Clone)") {
            playerMovement = collision.gameObject.GetComponent<PacMan3DMovement>();
            ActivatePowerUp();
            Debug.Log("Fell in trap");
        }
        
    }
    public void ActivatePowerUp()
    {
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        StartCoroutine(ApplySlowDown());
    }

    private IEnumerator ApplySlowDown()
    {
        playerMovement.speed *= slowDownFactor;
        yield return new WaitForSeconds(slowDownDuration);
        playerMovement.speed /= slowDownFactor;
        Destroy(gameObject);
    }
}
