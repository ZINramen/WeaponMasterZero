using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DK_Ctrl : MonoBehaviour
{
    public bool Is_Roll;

    private Animator myAnim;
    public  GameObject barrelPref;
    public  Transform barrel_SponPos;

    // Start is called before the first frame update
    void Start()
    {
        myAnim = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Is_Roll)
            myAnim.SetBool("Is_Roll", true);
        else
            myAnim.SetBool("Is_Roll", false);
    }

    void Instance_Barrel()
    {
        Instantiate(barrelPref, barrel_SponPos);
    }
}
