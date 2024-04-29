using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LakeCtrl : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D coll)
    {
        GameObject temp = coll.gameObject;

        if (temp.layer == LayerMask.NameToLayer("Entity"))
        {
            temp.GetComponent<Entity>().SetHp(0.0f);
        }
    }
}
