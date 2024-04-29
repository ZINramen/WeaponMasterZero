using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class StageCtrl : MonoBehaviour
{
    public bool isOnline = true;

    private bool stageSettingEnd = false; // 스테이지 설정 완료 여부
    private int playerNumber; //살아 있는 player 수
    private Entity superPlayer; //가장 HP가 많은 player

    public Entity[] PlayerList; //player 목록
    public Transform[] playerFirstLocations; //player 시작 위치

    // [Mingyu Version]
    private bool isDie = false;
    [SerializeField] GameObject GameResult_UI;

    [SerializeField] Text WinnerName_Text;
    [SerializeField] Text LoserName_Text;

    [SerializeField] Image Winner_UI;
    [SerializeField] Image Loser_UI;

    [Space]
    public Image HP_Bar1P;
    public Image HP_Bar2P;

    [Space]
    public Image SpecialGauge_1P;
    public Image SpecialGauge_2P;

    [Space]
    public Image[] SkillIcons1P;
    public Image[] SkillIcons2P;

    public enum GameResult { Win, Draw, Lose, None};
    public static GameResult result = GameResult.None;

    public DynamicCamera dCam;

    public CastleCtrl castle;

    public GameObject end;

    //public void addPlayerList(Entity player)
    //{
    //    playerList[]
    //}

    private void Awake()
    {
        ResultManager.KillCount = 0;
        if (!isOnline)
        {
            PlayerList[0].transform.position = playerFirstLocations[0].position;
            PlayerList[1].transform.position = playerFirstLocations[1].position;
            StartCoroutine(ReadyGO());
        }
    }

    void Update()
    {
        if (PlayerList == null || PlayerList.Length < 2)
        {
            PlayerList = FindObjectsOfType<Entity>(); //Entity를 가진 객체로 리스트를 만듬
        }
        if (PlayerList != null)
        {
            if (PlayerList.Length > 1)
            {
                if (!stageSettingEnd)
                {
                    stageSettingEnd = true; // 다중 실행 방지
                    if (castle)
                    {
                        castle.enabled = true;
                    }
                    dCam.enabled = true;
                    playerNumber = PlayerList.Length; //처음은 살아있으므로 리스트에 넣음
                    superPlayer = PlayerList[0]; //임시로 superPlayer 설정

                    CreateUIManager(); //UIManager 생성
                                       //Debug.Log("플레이어 수" + playerNumber);
                                       //Debug.Log("플레이어1" + PlayerList[0].name);
                                       //Debug.Log("플레이어2" + PlayerList[1].name);

                    StartCoroutine(PlayerCheck());
                }
            } 
        }
    }

    public void PlayerLock(bool LockMode) 
    {
        PlayerList[0].movement.PlayerType = LockMode;
        PlayerList[0].aManager.isPlayer = LockMode;

        if(PlayerList[1].movement)
            PlayerList[1].movement.PlayerType = LockMode;
        if (PlayerList[1].aManager)
            PlayerList[1].aManager.isPlayer = LockMode;
    }

    IEnumerator ReadyGO()
    {
        yield return new WaitForSeconds(3);
        PlayerLock(true);
    }

    //player 체력 체크 후 승패 체크
    IEnumerator PlayerCheck()
    {
        while (playerNumber > 0 && !isDie )
        {
            for (int i = 0; i < PlayerList.Length; i++)
            {
                if (PlayerList[i].GetHp() >= superPlayer.GetHp())
                    superPlayer = PlayerList[i];
            }

            //승리 조건
            if (superPlayer.isDie == true)
            {
                result = GameResult.Draw;
                Debug.Log("Draw");
            }
            else if (PlayerList[1].GetHp() <= 0)
            {
                result = GameResult.Win;
                Debug.Log(PlayerList[0].name + " Win");

                StartCoroutine(View_GameResult(PlayerList[0]));
            }
            else if (PlayerList[0].GetHp() <= 0)
            {
                result = GameResult.Lose;
                Debug.Log(PlayerList[1].name + " Win");

                StartCoroutine(View_GameResult(PlayerList[1]));
            }

            //Debug.Log("수뻐 플레이어 : " + superPlayer.name);
            //Debug.Log("Player1 HP :" + PlayerList[0].GetHp() + "Player2 HP :" + PlayerList[1].GetHp());
            yield return new WaitForSecondsRealtime(0.5f);
        }
    }

    IEnumerator View_GameResult(Entity winner)
    {
        Debug.Log("끝!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        isDie = true;

        if (isOnline)
        {
            if (GameResult_UI)
            {
                GameResult_UI.SetActive(true);
                Set_ResultData(winner.gameObject.GetComponent<PhotonView>().Owner.NickName);
            }
            DefaultPool pool;
            pool = PhotonNetwork.PrefabPool as DefaultPool;
            Debug.Log("실행");

            pool.ResourceCache.Clear();

            yield return new WaitForSecondsRealtime(3.0f);
            PhotonNetwork.LoadLevel("WaitingRoom");
        }
        else 
        {
            if (end)
            {
                if(winner.movement)
                    ResultManager.isWinner1P = !winner.movement.is2P;
                end.SetActive(true);
                PlayerLock(false);
            }
        }
    }

    private void Set_ResultData(string winnerName)
    {
        if (PhotonNetwork.MasterClient.NickName == winnerName)
        {
            WinnerName_Text.text = PhotonNetwork.MasterClient.NickName;
            LoserName_Text.text = PhotonNetwork.PlayerList[1].NickName;

            Loser_UI.color = new Color(1.0f, 127 / 255.0f, 39 / 255.0f);
        }
        else
        {
            LoserName_Text.text = PhotonNetwork.MasterClient.NickName;
            WinnerName_Text.text = PhotonNetwork.PlayerList[1].NickName;

            Winner_UI.color = new Color(1.0f, 127 / 255.0f, 39 / 255.0f);
        }
    }

    void CreateUIManager()
    {
        GameObject UIObj = new GameObject("UIManager");
        UICtrl myUICtrl = UIObj.AddComponent<UICtrl>();

        myUICtrl.HP_Bar1P = HP_Bar1P;
        myUICtrl.HP_Bar2P = HP_Bar2P;
        myUICtrl.SpecialGauge_1P = SpecialGauge_1P;
        myUICtrl.SpecialGauge_2P = SpecialGauge_2P;
        myUICtrl.SkillIcons1P = SkillIcons1P;
        myUICtrl.SkillIcons2P = SkillIcons2P;

        if (isOnline)
        {
            foreach (Entity player in PlayerList)
            {
                if (player.network)
                {
                    if (player.network.pv.IsMine)
                    {
                        if (PhotonNetwork.IsMasterClient)
                            myUICtrl.Player1 = player;
                        else
                            myUICtrl.Player2 = player;
                    }
                    else
                    {
                        if (PhotonNetwork.IsMasterClient)
                            myUICtrl.Player2 = player;
                        else
                            myUICtrl.Player1 = player;
                    }
                }
            }
        }
        else
        {
            myUICtrl.Player1 = PlayerList[0];
            myUICtrl.Player2 = PlayerList[1];
        }
    }
}
