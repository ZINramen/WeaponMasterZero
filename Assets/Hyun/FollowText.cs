using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FollowText : MonoBehaviour
{
    int playerNumber = 0;
    TextMesh textM;
    public Entity owner;
    string NicknameSave;
    private void Start()
    {
        textM = GetComponent<TextMesh>();

        if (owner.network.pv.IsMine)
        {
            NicknameSave = owner.network.pv.Owner.NickName;
            NameUpdate(NicknameSave);
        }
    }
    public void NameUpdate(string name)
    {
        if(name != "" && textM)
            textM.text = name;
    }
    private void Update()
    {
        if (owner.network.pv.IsMine)
        {
            owner.network.NameChange(NicknameSave);
        }

        if (transform.eulerAngles.y != 0)
            transform.eulerAngles = new Vector3(0, 0, 0);
    }
}
