using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using HashTable = ExitGames.Client.Photon.Hashtable;

enum ButtonNumber
{
    PreviousNumber = -2,
    NextBtnNumber = -1
};

enum RoomIndex
{
    ZeroRoom    = 0,
    FirstRoom   = 1,
    SecondRoom  = 2,
    ThirdRoom   = 3
};

public class PhotonInit : MonoBehaviourPunCallbacks
{
    // 싱글톤 패턴을 활용하여, 전역 변수 & 파괴x
    public static PhotonInit instance;
    
    private bool isGameStart    = false;
    private bool isLogin        = false;
    
    private string  playerName      = "";
    private string  connectionState = "";
    public  string  chatMessage;

    public  InputField  playerInput;
    private ScrollRect  scroll_rect = null;
    private PhotonView  pv;

    private Text        chatText;
    private Text        connectionInfo_Text;

    [Header("LobbyCanvas")]
    public GameObject LobbyCanvas;
    
    public GameObject LobbyPanel;
    public GameObject RoomPanel;
    public GameObject MakeRoomPanel;

    public InputField RoomInput;
    public InputField RoomPwInput;

    public Toggle PwToggle;
    public GameObject PwPanel;
    public GameObject PwErrorLog;
    public GameObject PwConfirmBtn;
    public GameObject PwPanelClose_Btn;
    public InputField Pw_CheckInput;

    public bool LockSate = false;
    public string privateRoom;
    
    public Button[] CellBtn;

    public Button PreviousBtn;
    public Button NextBtn;
    public Button CreateRoomBtn;

    public int hashTalbeCount;

    private List<RoomInfo> myList = new List<RoomInfo>();
    private int currPage    = 1;
    private int maxPage     = 0;
    private int multiple    = 0;
    private int roomNumber  = 0;

    private void Awake()
    {
        PhotonNetwork.GameVersion = "MyFps 1.0";
        PhotonNetwork.ConnectUsingSettings();

        if (GameObject.Find("ChatText") != null)
            chatText =  GameObject.Find("ChatText").GetComponent<Text>();

        if (GameObject.Find("Scroll View") != null)
            scroll_rect = GameObject.Find("Scroll View").GetComponent<ScrollRect>();

        if (GameObject.Find("ConnectionInfoText") != null)
            connectionInfo_Text = GameObject.Find("ConnectionInfoText").GetComponent<Text>();

        connectionState = "마스터 서버에 접속 중...";

        if (connectionInfo_Text)
            connectionInfo_Text.text = connectionState;

        PlayerPrefs.SetInt("LogIn", 0);

        //아래의 함수를 사용하여 씬이 전환 되더라도 선언 되었던 인스턴스가 파괴되지 않는다.
        //DontDestroyOnLoad(gameObject);
    }

    // 룸이 업데이트 될 때만 호출되는 함수
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("OnRoomListUpdate" + roomList.Count);
        int roomCount = roomList.Count;

        for (int i = 0; i < roomCount; i++) 
        {
            // 리스트에 존재한다면
            if (!roomList[i].RemovedFromList)
            {
                // 내 리스트에 존재하지 않는다면, 추가해준다.
                if (!myList.Contains(roomList[i])) 
                    myList.Add(roomList[i]);

                // 내 리스트에 존재한다면, roomList가 존재하는 리스트의 인덱스를 가져와서 데이터를 넣어준다.
                else
                    myList[myList.IndexOf(roomList[i])] = roomList[i];
            }

            // 리스트에 존재하지 않고, 내 리스트에만 존재한다면, 삭제된 방으로 간주하여, 삭제
            else if (myList.IndexOf(roomList[i]) != -1) 
                myList.RemoveAt(myList.IndexOf(roomList[i]));
        }

        MyListRenewal();
    }

    public void MyListClick(int btn_Num)
    {
        if(btn_Num == (int)ButtonNumber.PreviousNumber)
        {
            --currPage;
            MyListRenewal();
        }

        else if (btn_Num == (int)ButtonNumber.NextBtnNumber)
        {
            ++currPage;
            MyListRenewal();
        }

        // 클릭한 방이 패스워드가 있다면, 패드워드 창 띄우기
        else if (myList[multiple + btn_Num].CustomProperties["password"] != null)
        {
            PwPanel.SetActive(true);
        }

        // 없다면, 방에 들어가기
        else
        {
            PhotonNetwork.JoinRoom(myList[multiple + btn_Num].Name);
            MyListRenewal();
        }
    }

    // 방 인덱스를 반환하는 함수
    public void RoomPw(int roomIndex)
    {
        switch(roomIndex)
        {
            case (int)RoomIndex.ZeroRoom:
                roomIndex = (int)RoomIndex.ZeroRoom;
                break;

            case (int)RoomIndex.FirstRoom:
                roomIndex = (int)RoomIndex.FirstRoom;
                break;

            case (int)RoomIndex.SecondRoom:
                roomIndex = (int)RoomIndex.SecondRoom;
                break;

            case (int)RoomIndex.ThirdRoom:
                roomIndex = (int)RoomIndex.ThirdRoom;
                break;

            default:
                break;
        }
    }

    // 패스 워드가 있는 방에 들어갔을 때, 비번이 맞고 틀린 유무에 따라
    public void EnterRoomWithPW()
    {
        if ((string)myList[multiple + roomNumber].CustomProperties["password"] == Pw_CheckInput.text)
        {
            PhotonNetwork.JoinRoom(myList[multiple + roomNumber].Name);
            MyListRenewal();
            PwPanel.SetActive(false);
        }
        else
            StartCoroutine("ShowPwWrongMsg");
    }

    IEnumerator ShowPwWrongMsg()
    {
        if (PwErrorLog.activeSelf)
        {
            PwErrorLog.SetActive(true);
            yield return new WaitForSeconds(3.0f);
            PwErrorLog.SetActive(false);
        }
    }

    public void MyListRenewal()
    {
        maxPage = (myList.Count % CellBtn.Length == 0) ? 
            myList.Count / CellBtn.Length : 
            myList.Count / CellBtn.Length + 1;

        Debug.Log("Max Page = " + maxPage + "\n" + "CellBtn : " + CellBtn.Length);

        PreviousBtn.interactable = (currPage <= 1) ? false : true;
        NextBtn.interactable = (currPage >= maxPage) ? false : true;

        multiple = (currPage - 1) * CellBtn.Length;

        for (int i = 0; i < CellBtn.Length; i++)
        {
            CellBtn[i].interactable = (multiple + i < myList.Count) ? true : false;
            CellBtn[i].transform.GetChild(0).GetComponent<Text>().text =
                (multiple + i < myList.Count ? myList[multiple + i].Name : "");

            CellBtn[i].transform.GetChild(1).GetComponent<Text>().text =
                (multiple + i < myList.Count) ?
                myList[multiple + i].PlayerCount + "/" + myList[multiple + i].MaxPlayers : "";
        }
    }

    //public override void OnJoinedLobby()
    //{
    //    base.OnJoinedLobby();
    //    Debug.Log("Joined Lobby");
    //    //PhotonNetwork.CreateRoom("MyRoom");
    //    PhotonNetwork.JoinRandomRoom();
    //    //PhotonNetwork.JoinRoom("MyRoom");
    //}

    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            connectionState = "룸에 접속...";

            if (connectionInfo_Text)
                connectionInfo_Text.text = connectionState;

            LobbyPanel.SetActive(false);
            RoomPanel.SetActive(true);
            PhotonNetwork.JoinLobby();
            //PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            connectionState = "오프라인 : 마스터 서버와 연결되지 않음\n접속 재시도중...";

            if (connectionInfo_Text)
                connectionInfo_Text.text = connectionState;

            PhotonNetwork.ConnectUsingSettings();
        }

    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        connectionState = "No Room";

        if (connectionInfo_Text)
            connectionInfo_Text.text = connectionState;

        Debug.Log("No Room");
        //PhotonNetwork.CreateRoom("MyRoom");
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        connectionState = "Finish make a room";

        if (connectionInfo_Text)
            connectionInfo_Text.text = connectionState;

        Debug.Log("Finish make a room");
        
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log("OnCreateRoomFailed:"+returnCode + "-"+message);
    }
   
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        connectionState = "Joined Room";

        if (connectionInfo_Text)
            connectionInfo_Text.text = connectionState;

        Debug.Log("Joined Room");
        isLogin = true;
        PlayerPrefs.SetInt("LogIn", 1);

        //StartCoroutine(CreatePlayer());
        PhotonNetwork.LoadLevel("WaitingRoom");
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("LogIn", 0); 
    }

    private void Update()
    {
        if (PlayerPrefs.GetInt("LogIn") == 1)
            isLogin = true;

        if(isGameStart == false && SceneManager.GetActiveScene().name == "SampleScene" && isLogin == true)
        {
            isGameStart = true;
            if (GameObject.Find("ChatText") != null)
                chatText = GameObject.Find("ChatText").GetComponent<Text>();

            if (GameObject.Find("Scroll View") != null)
                scroll_rect = GameObject.Find("Scroll View").GetComponent<ScrollRect>();

            StartCoroutine(CreatePlayer());
        }
    }

    IEnumerator CreatePlayer()
    {
        while(!isGameStart)
        {
            yield return new WaitForSeconds(0.5f);
        }

        GameObject tempPlayer = PhotonNetwork.Instantiate("Human",
                                    new Vector3(0, 0.15f, 5.75443172f),
                                    Quaternion.identity,
                                    0);

        if(tempPlayer)
        {
            Debug.Log("플레이어 생성");
        }

        pv = GetComponent<PhotonView>();

        yield return null;
    }

    private void OnGUI()
    {
        GUILayout.Label(connectionState);
    }

    public void SetPlayerName()
    {
        Debug.Log(playerInput.text + "를 입력 하셨습니다!" + 
            "\n Game Start : " + isGameStart +
            "\n Log In : " + isLogin);

        // Lobby실
        if (isGameStart == false && isLogin == false)
        {
            playerName = playerInput.text;
            playerInput.text = string.Empty;
            Debug.Log("Connect 시도!" + isGameStart + ", " + isLogin);
            Connect();
        }

        //Set Push에 사용
        else
        {
            chatMessage = playerInput.text;
            playerInput.text = string.Empty;
            //ShowChat(chatMessage);
            pv.RPC("ChatInfo", RpcTarget.All, chatMessage);
        }
        
    }

    public void ShowChat(string chat)
    {
        chatText.text += chat + "\n";
        scroll_rect.verticalNormalizedPosition = 0.0f;
    }

    [PunRPC]
    public void ChatInfo(string sChat)
    {
        ShowChat(sChat);
    }

    public void CreateRoom_BtnClick()
    {
        Debug.Log("방 생성 버튼 클릭");

        RoomPanel.SetActive(false);
        MakeRoomPanel.SetActive(true);
    }

    public void MakeOK_BtnOnClick()
    {
        MakeRoomPanel.SetActive(false);
    }

    public void DisConnect_BtnClick()
    {
        PhotonNetwork.Disconnect();

        RoomPanel.SetActive(false);
        LobbyPanel.SetActive(true);

        connectionState = "마스터 서버에 접속 중...";
        if (connectionInfo_Text)
            connectionInfo_Text.text = connectionState;

        isGameStart = false;
        isLogin = false;
        PlayerPrefs.SetInt("LogIn", 0);
    }

    public void CreateNewRoom()
    {
        Debug.Log("방 생성 버튼 클릭");
        RoomOptions roomOptions = new RoomOptions();

        roomOptions.MaxPlayers = 20;
        roomOptions.CustomRoomProperties = new HashTable()
        {
            { "password", RoomInput.text}
        };

        roomOptions.CustomRoomPropertiesForLobby = new string[] { "password" };

        if(PwToggle.isOn)
            PhotonNetwork.CreateRoom(RoomInput.text == "" ? "Game" + Random.Range(0, 80) : "*" + RoomInput.text, 
                roomOptions);

        else
            PhotonNetwork.CreateRoom(RoomInput.text == "" ? "Game" + Random.Range(0, 80) : RoomInput.text, 
                new RoomOptions { MaxPlayers = 20 });

        MakeRoomPanel.SetActive(false);
    }
}
