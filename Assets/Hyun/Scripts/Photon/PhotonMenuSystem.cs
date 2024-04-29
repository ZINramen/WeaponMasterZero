using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PhotonMenuSystem : MonoBehaviourPunCallbacks
{
    [SerializeField]List<PhotonRoomListInfoSync> roomItems;
    int roomCount = 0;
    public GameObject roomListView;
    public GameObject roomListItem;
    public Animation simpleAnim;
    private void Awake()
    {
        PhotonNetwork.GameVersion = "FierceFight 1.0";
        PhotonNetwork.ConnectUsingSettings();

        Debug.Log("마스터 서버에 접속중입니다.");
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("ddd");
        foreach (RoomInfo item in roomList) 
        {
            if (!item.RemovedFromList)
            {
                Transform earlyRoom = roomListView.transform.Find(item.Name);
                RectTransform roomTemp;
                if (earlyRoom != null)
                {
                    roomTemp = earlyRoom.GetComponent<RectTransform>();
                }
                else
                {
                    roomTemp = GameObject.Instantiate(roomListItem).GetComponent<RectTransform>();
                    roomTemp.SetParent(roomListView.transform);
                    roomTemp.localScale = new Vector3(1, 1, 1);
                }
                PhotonRoomListInfoSync sync = roomTemp.GetComponent<PhotonRoomListInfoSync>();
                sync.name = item.Name;
                sync.roomNameT.text = item.Name;
                sync.playerNumberT.text = item.PlayerCount.ToString();
                if (item != null)
                    roomItems.Add(sync);
            }
            else 
            {
                foreach (PhotonRoomListInfoSync room in roomItems) 
                {
                    if (room.name == item.Name)
                    {
                        roomItems.Remove(room);
                        Destroy(room.gameObject);
                        break;
                    }
                }
            }
        }
        roomCount = roomList.Count; Debug.Log(roomList.Count);

    }
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("마스터 서버 접속 성공");
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        simpleAnim.Play();
        Debug.Log("로비에 접속하였습니다.");
    }
    public void ObjectSet(GameObject obj, bool value) 
    {
        obj.SetActive(value);
    }

    public void SetPlayerName(InputField inputName) 
    {
        PhotonNetwork.NickName = inputName.text;
    }
    public void CreateRoomBtn()
    {
        // 테스트 용입니다.
        PhotonNetwork.CreateRoom("TestRoom" + roomCount, new RoomOptions { MaxPlayers = 2 });
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel("WaitingRoom TEST");
    }
}
