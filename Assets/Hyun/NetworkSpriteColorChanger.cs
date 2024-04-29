using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkSpriteColorChanger : MonoBehaviour
{
    PhotonView pv;
    SpriteRenderer sp;
    public Color p1;
    public Color p2;

    private void Awake()
    {
        sp = GetComponent<SpriteRenderer>();
        pv = transform.root.GetComponent<PhotonView>();
        Change();
    }

    // Update is called once per frame
    void Update()
    {
        Change();
    }

    void Change() 
    {
        if (pv.IsMine)
            if (PhotonNetwork.IsMasterClient)
                sp.color = p1;
            else
                sp.color = p2;
        else
               if (PhotonNetwork.IsMasterClient)
            sp.color = p2;
        else
            sp.color = p1;
    }
}
