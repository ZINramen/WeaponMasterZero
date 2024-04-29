using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PhotonRoomListInfoSync : MonoBehaviourPunCallbacks
{
    public Text roomNameT;
    public Text playerNumberT;
    public Text StageNameT;
    // Start is called before the first frame update
    public void join()
    {
        if (roomNameT.text != "")
        {
            PhotonNetwork.JoinRoom(roomNameT.text);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
