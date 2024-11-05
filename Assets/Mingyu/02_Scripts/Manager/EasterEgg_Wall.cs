using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EasterEgg_Wall : MonoBehaviour
{
    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.root.gameObject.tag == "Player")
        {
            if (other.gameObject.GetComponent<HitColider>().attType
                == HitColider.AttackType.Player_FinishdAtt)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
