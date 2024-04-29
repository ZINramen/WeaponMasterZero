using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cupa_Ctrl : MonoBehaviour
{
    public bool is_Fire = true;

    private Animator Anim_FireMotion;
    public GameObject Fire;
    public GameObject CupaController;

    // Start is called before the first frame update
    void Start()
    {
        Anim_FireMotion = this.GetComponent<Animator>();
    }

    private void Update()
    {
        if (!is_Fire)
        {
            Anim_FireMotion.SetBool("Is_FireMotion", false);
            Fire.GetComponent<Animator>().SetBool("Is_Fire", false);
        }
    }

    public void On_FireMotion()
    {
        Anim_FireMotion.SetBool("Is_FireMotion", true);
    }

    public void On_FireBreath()
    {
        Fire.GetComponent<Animator>().SetBool("Is_Fire", true);
    }

    public void Off_FireMotion()
    {
        Anim_FireMotion.SetBool("Is_FireMotion", false);
        Anim_FireMotion.SetBool("Is_Bright", false);

        CupaController.GetComponent<Cupa_Mng>().is_Check = false;
        CupaController.GetComponent<Cupa_Mng>().End_CupaAttack();
    }
}
