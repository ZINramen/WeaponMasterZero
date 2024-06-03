using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        float destroyTime = this.gameObject.GetComponent<ParticleSystem>().duration;
        Destroy(this.gameObject, destroyTime);
    }
}
