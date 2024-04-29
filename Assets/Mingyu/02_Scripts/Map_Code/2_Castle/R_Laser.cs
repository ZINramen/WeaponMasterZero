using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R_Laser : MonoBehaviour
{
    public GameObject PCastle_Ctrl;

    void End_Laser()
    {
        PCastle_Ctrl.gameObject.GetComponent<PCastle_Ctrl>().EndLaser_Setting();
    }
}
