using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Mingyu_RoomCtrl : MonoBehaviourPunCallbacks
{
    private Mingyu_Photon_Lobby photonCtrl;

    public Text roomNameT;
    public Text playerNumberT;
    public Text StageNameT;

    private int roomIndex;

    // 방 리스트를 업데이트 할때, 인덱스를 업데이트함
    public int Set_RoomIndex { set => roomIndex = value; }

    private void Start()
    {
        photonCtrl = GameObject.Find("Canvas").
            gameObject.GetComponent<Mingyu_Photon_Lobby>();

        if (photonCtrl == null)
            return;
    }

    public void Btn_EnterButton()
    {
        photonCtrl.BtnEvent_JoinRoom(roomIndex);
    }

    public void Btn_EnterPWRoom()
    {
        photonCtrl.EnterRoomWithPW(roomIndex);
    }

    public void joinRoom()
    {
        PhotonNetwork.JoinRoom(roomNameT.text);
    }
}
