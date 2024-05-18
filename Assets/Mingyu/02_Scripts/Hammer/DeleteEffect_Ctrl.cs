using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteEffect_Ctrl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        float delayTime = this.gameObject.GetComponent<ParticleSystem>().duration;
        Destroy(this.gameObject, delayTime);
    }
}
