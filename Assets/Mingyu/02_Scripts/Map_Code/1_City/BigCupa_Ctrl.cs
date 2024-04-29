using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigCupa_Ctrl : MonoBehaviour
{
    public bool is_Fire;

    private Animator Anim_FireMotion;
    public GameObject Fire;

    // Start is called before the first frame update
    void Start()
    {
        Anim_FireMotion = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (is_Fire)
            Anim_FireMotion.SetBool("Is_FireMotion", true);
        else
            Anim_FireMotion.SetBool("Is_FireMotion", false);
    }

    public void On_FireBreath()
    {
        Fire.GetComponent<Animator>().SetBool("Is_Fire", true);
    }

    public void Off_FireBreath()
    {
        Fire.GetComponent<Animator>().SetBool("Is_Fire", false);
    }
}
