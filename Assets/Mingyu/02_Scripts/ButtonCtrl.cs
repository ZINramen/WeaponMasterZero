using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ButtonCtrl : MonoBehaviourPunCallbacks
{
    public Sprite On;
    public Sprite Off;

    // 버튼이 스크린을 가지고 있다.
    public GameObject OK_Screen;
    public GameObject RoomCtrl;

    private PhotonView pv;

    private bool is_Coll = false;               // tv와 trigger했는가?
    public bool Is_CollPlayer { get => is_Coll; }

    private void Start()
    {
        pv = this.GetComponent<PhotonView>();
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        GetComponent<SpriteRenderer>().sprite = On;
        if (coll.gameObject.tag == "Player")
        {
            pv.RPC("Set_CollPlayer", RpcTarget.All, true);
        }
    }

    private void OnTriggerExit2D(Collider2D coll)
    {
        GetComponent<SpriteRenderer>().sprite = Off;
        if (coll.gameObject.tag == "Player")
        {
            pv.RPC("Set_CollPlayer", RpcTarget.All, false);
        }
    }

    // 다른 플레이어가 방에 들어왔는지 체크한 후, RPC로 동기화
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        if(PhotonNetwork.IsMasterClient == true)
            pv.RPC("Set_CollPlayer", RpcTarget.All, is_Coll);

        Debug.Log("Enter Another User");
    }

    // 플레이어가 나가면, false로 동기화
    private void OnApplicationQuit()
    {
        pv.RPC("Set_CollPlayer", RpcTarget.All, false);
    }

    [PunRPC]
    public void Set_CollPlayer(bool isColl_Player)
    {
        OK_Screen.SetActive(isColl_Player);
        is_Coll = isColl_Player;

        if(is_Coll)
            GetComponent<SpriteRenderer>().sprite = On;
        else
            GetComponent<SpriteRenderer>().sprite = Off;
    }
}
