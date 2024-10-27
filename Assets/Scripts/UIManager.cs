using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// write a scritp through which I take two pannesl and then I disable one and able one through a function call 

public class UIManager : MonoBehaviour
{
    public GameObject panel1;
    public GameObject panel2;

    public void EnablePanel1(){
        panel1.SetActive(true);
        panel2.SetActive(false);
    }

    public void EnablePanel2(){
        panel1.SetActive(false);
        panel2.SetActive(true);
    }
}
