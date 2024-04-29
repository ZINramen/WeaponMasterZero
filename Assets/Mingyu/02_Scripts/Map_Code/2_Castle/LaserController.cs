using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;


public class LaserController : MonoBehaviour
{
    public bool is_ShootLaser;

    public GameObject Lhf_Laser;
    public GameObject Rhf_Laser;

    private void Start()
    {
        Lhf_Laser.SetActive(false);
        Rhf_Laser.SetActive(false);
    }

    private void Update()
    {
        if (is_ShootLaser)
        {
            Camera.main.GetComponent<PostProcessVolume>().enabled = true;

            Lhf_Laser.SetActive(true);
            Rhf_Laser.SetActive(true);
        }
        else
        {
            Camera.main.GetComponent<PostProcessVolume>().enabled = false;

            Lhf_Laser.SetActive(false);
            Rhf_Laser.SetActive(false);
        }
    }
}
