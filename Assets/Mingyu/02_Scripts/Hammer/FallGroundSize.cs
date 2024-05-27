using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallGroundSize : MonoBehaviour
{
    private int index = 0;
    private float[] FullBoxColl_Siz;
    private BoxCollider2D myBoxColl;

    private void Start()
    {
        myBoxColl = this.gameObject.GetComponent<BoxCollider2D>();

        FullBoxColl_Siz = new float[3];
        
        FullBoxColl_Siz[0] = 11.93f;
        FullBoxColl_Siz[1] = 10.57f;
        FullBoxColl_Siz[2] = 8.7f;
    }

    public void FallGround()
    {
        if (index >= 3) 
            return;
        else
        {
            myBoxColl.size = new Vector2(FullBoxColl_Siz[index], myBoxColl.size.y);
            index++;
        }
    }
}
