using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;
using HashTable = ExitGames.Client.Photon.Hashtable;

public class Mingyu_Photon_Lobby : MonoBehaviourPunCallbacks
{
    [SerializeField]List<Mingyu_RoomCtrl> roomItems;        // 사용자에게 보여지는 룸 정보

    private int roomCount = 0;              // 룸 갯수
    public GameObject roomListView;
    public GameObject roomListItem;
    public Animation simpleAnim;

    // 민규가 코딩한 부분
    #region
    //[SerializeField] GameObject PlayerLobby;
    //DefaultPool pool;

    private PhotonView pv;
    private static Mingyu_Photon_Lobby instance;
    RoomOptions roomOptions;

    // 내부 룸 데이터 관리 리스트
    private List<RoomInfo> roomData_List = new List<RoomInfo>();
    public int a = 0;

    // 채팅 관련 변수
    private ScrollRect  scRect;
    private Text        chat_Text;
    private string      ChatMessage = "";

    // 타이틀
    private GameObject title_UI;

    // 게임 패널 + 로비
    private GameObject lobby;
    private GameObject makeRoom_Panel;
    private GameObject pw_Panel;
    private GameObject pw_ErrorLog;
    private InputField pw_CheckInput;

    private Toggle pw_Toggle;                   // 토글

    public string roomName;                // 만들 때, 방 이름

    // 방을 만들었는지 확인, 이는 나중에 로비에서
    // 방을 만들었으면,          방을 생성하는 코루틴으로 넘어가고
    // 방을 만들지 않았으면,     방으로 들어가는 코루틴으로 들어감
    private bool isCreateRoom = false;                  
    #endregion

    //public static Mingyu_Photon_Lobby Instance
    //{
    //    get
    //    {
    //        if(!instance)
    //        {
    //            instance = FindObjectOfType(typeof(Mingyu_Photon_Lobby)) as Mingyu_Photon_Lobby;

    //            if (instance == null)
    //                Debug.Log("No SingleTon OBJ");
    //        }

    //        return instance;
    //    }
    //}

    private void Awake()
    {
        PhotonNetwork.GameVersion = "FierceFight 1.0";
        PhotonNetwork.ConnectUsingSettings();

        Debug.Log("마스터 서버에 접속중입니다.");
    }

    private void OnApplicationQuit()
    {
        PhotonNetwork.Disconnect();
    }

    private void Start()
    {
        // TitleUI 참조 (비활성화 목적)
        if (GameObject.Find("TitleUI") != null)
        {
            title_UI = GameObject.Find("TitleUI");
            Debug.Log("타이틀 참조" + title_UI.name);
        }

        // 채팅창 참조 (Scroll Rect)
        if (GameObject.Find("Chat_Box_ScrollVersion") != null)
        {
            scRect = GameObject.Find("Chat_Box_ScrollVersion").GetComponent<ScrollRect>();
            Debug.Log("채팅창 참조" + scRect.name);
        }

        // 채팅 참조 (text)
        if (GameObject.Find("Chat_Text") != null)
        {
            chat_Text = GameObject.Find("Chat_Text").GetComponent<Text>();
            Debug.Log("채팅(text) 참조" + chat_Text.name);
        }

        //  방 만들기 패널 참조 (GameObject)
        if (GameObject.Find("MakeRoomPanel") != null)
        {
            makeRoom_Panel = GameObject.Find("MakeRoomPanel");
            Debug.Log("방 패널 참조" + makeRoom_Panel.name);
        }
        
        if (makeRoom_Panel.transform.Find("Toggle"))
        {
            pw_Toggle = makeRoom_Panel.transform.Find("Toggle").GetComponent<Toggle>();
            Debug.Log("방 만들 때, 패스워드 입력 참조" + pw_Toggle.name);

            makeRoom_Panel.SetActive(false);
        }

        //  패스워드 패널 참조 (GameObject)
        if (GameObject.Find("Pw_Panel") != null)
        {
            pw_Panel = GameObject.Find("Pw_Panel");
            Debug.Log("방 비번 참조 패널" + pw_Panel.name);
        }

        if (pw_Panel.transform.Find("Error_Log") != null)
        {
            pw_ErrorLog = pw_Panel.transform.Find("Error_Log").gameObject;
            Debug.Log("패스워드 오류 참조" + pw_ErrorLog.name);

            pw_ErrorLog.SetActive(false);
        }

        // 패스워드 토글 참조 (Toggle)
        if (pw_Panel.transform.Find("Pw_CheckInput") != null)
        {
            pw_CheckInput = pw_Panel.transform.Find("Pw_CheckInput").
                            GetComponent<InputField>();

            Debug.Log("패스워드 토글 참조" + pw_CheckInput.name);

            pw_Panel.SetActive(false);
        }

        // 로비 참조 (GameObject, 비활성화 목적) <이 코드는 start 함수 맨 아래 있어야함>
        if (GameObject.Find("Lobby") != null)
        {
            lobby = GameObject.Find("Lobby");
            lobby.SetActive(false);
        }

        PhotonNetwork.AutomaticallySyncScene = true;

        //pool = PhotonNetwork.PrefabPool as DefaultPool;
        //pool.ResourceCache.Add(PlayerLobby.name, PlayerLobby);
    }

    #region 방 리스트를 업데이트 하는 부분 (호현이쪽 코드)
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo item in roomList) 
        {
            if (!item.RemovedFromList)
            {
                Debug.Log("방 생성");

                #region 룸 리스트 데이터 업데이트
                // 룸 리스트에 존재하지 않는다면, 추가해준다.
                if (!roomData_List.Contains(item))
                    roomData_List.Add(item);

                // 룸 리스트에 존재한다면, roomList가 존재하는 리스트의 인덱스를 가져와서 데이터를 넣어준다.
                else
                    roomData_List[roomData_List.IndexOf(item)] = item;
                #endregion

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
                Mingyu_RoomCtrl sync = roomTemp.GetComponent<Mingyu_RoomCtrl>();
                if (sync)
                {
                    sync.name = item.Name;
                    sync.roomNameT.text = item.Name;
                    sync.playerNumberT.text = item.PlayerCount.ToString();

                    sync.Set_RoomIndex = roomData_List.IndexOf(item);

                    if (item.CustomProperties["stageName"] != null)
                    {
                        Debug.Log(item.CustomProperties["stageName"].ToString());
                        sync.StageNameT.text = item.CustomProperties["stageName"].ToString();
                        Debug.Log("AA");
                    }
                    else
                        Debug.Log("이름 없음");
                }
                if (item != null)
                    roomItems.Add(sync);
            }
            else 
            {
                Debug.Log("방 삭제");

                roomData_List.RemoveAt(roomData_List.IndexOf(item));

                foreach (Mingyu_RoomCtrl room in roomItems) 
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
        roomCount = roomList.Count;
        Debug.Log(roomList.Count);
    }
    #endregion

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, HashTable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        Debug.Log("바뀜");
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

    // 유저가 아이디를 입력하고 들어가면, Lobby라는 방으로 입장한다.
    // -> 방에 들어가야, 채팅이 되기 때문
    public void SetPlayerName(InputField inputName) 
    {
        PhotonNetwork.NickName = inputName.text;

        //// lobby라는 방이 있다면, 들어가고 + 없다면, 생성해서 들어간다.
        //photonnetwork.joinorcreateroom("lobby", new roomoptions { maxplayers = 20 }, 
        //    new typedlobby("lobby", lobbytype.default));
    }

    #region 로비 채팅 부분 코딩
    public void Chatting_Lobby(InputField inputChatting)
    {
        ChatMessage = PhotonNetwork.NickName + ": " + inputChatting.text;
        inputChatting.text = string.Empty;

        pv = this.GetComponent<PhotonView>();
        pv.RPC("ChatInfo", RpcTarget.All, ChatMessage);
    }

    public void ShowChat(string chat)
    {
        chat_Text.text += chat + "\n";
        scRect.verticalNormalizedPosition = 1.0f;
    }

    [PunRPC]
    public void ChatInfo(string sChat)
    {
        ShowChat(sChat);
    }
    #endregion

    #region 버튼 클릭 함수들

    // 로비에서 나가기 버튼을 눌렀을 경우
    public void BtnEvent_ExitGame(GameObject blackScreen)
    {
        blackScreen.SetActive(true);
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // 방 만들기 입력
    public void BtnEvent_MakeRoomBtn()
    {
        Debug.Log("방 생성 버튼 클릭");

        makeRoom_Panel.SetActive(true);
    }

    // 방 만들기를 끝낸 후
    public void BtnEvent_MakeOk()
    {
        roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        Debug.Log(pw_Toggle.isOn);

        string default_StageName = "피치 성 외각 ";

        if (pw_Toggle.isOn)
        {
            string InputPWord = makeRoom_Panel.transform.Find("RoomPassword_InputField").
                            GetComponent<InputField>().text;

            roomOptions.CustomRoomPropertiesForLobby = new string[] { "password" };
            roomOptions.CustomRoomPropertiesForLobby = new string[] { "stageName" };

            roomOptions.CustomRoomProperties = new HashTable()
            {
                { "password", InputPWord },
                { "stageName", default_StageName }
            };
        }
        else
        {
            roomOptions.CustomRoomPropertiesForLobby = new string[] { "stageName" };
            roomOptions.CustomRoomProperties = new HashTable()
            {
                { "stageName", default_StageName }
            };
        }

        roomName = makeRoom_Panel.transform.Find("RoomName_InputField").
                            GetComponent<InputField>().text;

        isCreateRoom = true;
        makeRoom_Panel.SetActive(false);
        //PhotonNetwork.LeaveRoom();

        // 추가한 부분
        StartCoroutine(Enter_WaitRoom(roomName, isCreateRoom));
    }

    public void BtnEvent_JoinRoom(int roomIndex)
    {
        if (roomData_List[roomIndex].CustomProperties["password"] != null)
        {
            Debug.Log("비번o : " + roomData_List[roomIndex].Name);
            lobby.SetActive(false);
            title_UI.SetActive(false);

            pw_Panel.SetActive(true);
        }
        else
        {
            Debug.Log("비번x" + roomIndex);

            roomName = roomData_List[roomIndex].Name;

            StartCoroutine(Enter_WaitRoom(roomName, isCreateRoom));
        }
    }

    // 패스 워드가 있는 방에 들어갔을 때, 비번이 맞고 틀린 유무에 따라
    public void EnterRoomWithPW(int roomIndex)
    {
        if ((string)roomData_List[roomIndex].CustomProperties["password"] == pw_CheckInput.text)
        {
            pw_Panel.SetActive(false);

            roomName = roomData_List[roomIndex].Name;

            StartCoroutine(Enter_WaitRoom(roomName, isCreateRoom));
        }
        else
            StartCoroutine("ShowPwWrongMsg");
    }

    IEnumerator ShowPwWrongMsg()
    {
        Debug.Log("틀림");

        if (pw_ErrorLog.activeSelf == false)
        {
            pw_ErrorLog.SetActive(true);
            yield return new WaitForSeconds(3.0f);
            pw_ErrorLog.SetActive(false);
        }
    }

    IEnumerator Enter_WaitRoom(string roomName, bool isCreateRoom)
    {
        Debug.Log("Room Name : " + roomName);

        yield return new WaitUntil(() => PhotonNetwork.IsConnected);
        PhotonNetwork.JoinLobby();

        if (isCreateRoom)
        {
            // 로비에 들어갈 때까지 대기
            yield return new WaitUntil(() => PhotonNetwork.InLobby);
            PhotonNetwork.CreateRoom(roomName, roomOptions);

            Debug.Log("룸 만듬");
        }
        else
        {
            yield return new WaitUntil(() => PhotonNetwork.InLobby);
            PhotonNetwork.JoinRoom(roomName);
        }

        // 방에 들어갈 때까지 대기
        yield return new WaitUntil(() => PhotonNetwork.InRoom);
        PhotonNetwork.LoadLevel("WaitingRoom");
    }

    //public void BtnEvent_EnterRoom()
    //{
    // 클릭한 방이 패스워드가 있다면, 패드워드 창 띄우기
    //if (is_OnPw)
    //{
    //    makeRoom_Panel.SetActive(false);
    //    pw_Panel.SetActive(true);
    //}

    //// 없다면, 방에 들어가기
    //else
    //{
    //    PhotonNetwork.Disconnect();
    //    PhotonNetwork.JoinRoom(make_RoomName);
    //    PhotonNetwork.LoadLevel("WaitingRoom");
    //}
    //}

    // 취소 클릭
    public void BtnEvent_Exit()
    {
        Debug.Log("취소 입력");

        title_UI.SetActive(true);
        lobby.SetActive(true);
        
        pw_Panel.SetActive(false);
        makeRoom_Panel.SetActive(false);
    }

    #endregion
}
