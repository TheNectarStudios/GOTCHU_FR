using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseControls : MonoBehaviour
{
    private WASDMovement playerMovement;
    public float reverseDuration = 5f;
    private GameObject[] ghosts;




    private void OnCollisionEnter(Collision collision)
    {
        //playerMovement = collision.gameObject.GetComponent<WASDMovement>();
        ActivatePowerUp();
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

    }

    public void ActivatePowerUp()
    {
        ghosts = GameObject.FindGameObjectsWithTag("Ghost");


        foreach (GameObject ghost in ghosts)
        {
            ghost.GetComponent<WASDMovement>().moveSpeed *= -1;
        }

        StartCoroutine(UnfreezeAfterDelay());
    }

    private IEnumerator UnfreezeAfterDelay()
    {
        yield return new WaitForSeconds(reverseDuration);

        ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in ghosts)
        {
            ghost.GetComponent<WASDMovement>().moveSpeed *= -1;
        }
        Destroy(gameObject);
    }
}
