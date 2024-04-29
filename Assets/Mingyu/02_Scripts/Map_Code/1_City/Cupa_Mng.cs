using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cupa_Mng : MonoBehaviour
{
    public bool is_CupaAttack = false;
    public bool is_Check = false;

    public GameObject Cupa;


    // Update is called once per frame
    void Update()
    {
        if (is_CupaAttack && !is_Check)
        {
            Cupa.gameObject.SetActive(true);
            Cupa.gameObject.GetComponent<Cupa_Ctrl>().is_Fire = true;
            is_Check = true;
        }

        else if (! is_CupaAttack)
        {
            Cupa.gameObject.SetActive(false);
        }
    }

    public void End_CupaAttack()
    {
        is_CupaAttack = false;
    }
}
