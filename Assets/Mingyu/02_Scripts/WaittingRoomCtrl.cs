using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class WaittingRoomCtrl : MonoBehaviourPun
{
    public GameObject L_Button;
    public GameObject R_Button;

    public string stageName = "";

    public bool isColl_LButton = false;
    public bool isColl_RButton = false;
    public bool isGameStart = false;

    private PhotonView pv;

    private string select_StageName = "피치 성 외각";
    private Color brightColor = new Color(1.0f, 1.0f, 1.0f);
    private Color darkColor = new Color(0.8f, 0.8f, 0.8f);

    private void Start()
    {
        pv = this.GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        isColl_LButton = L_Button.GetComponent<ButtonCtrl>().Is_CollPlayer;
        isColl_RButton = R_Button.GetComponent<ButtonCtrl>().Is_CollPlayer;

        if (isGameStart == false && isColl_LButton && isColl_RButton)
        {
            isGameStart = true;

            if (PhotonNetwork.IsMasterClient)
            {
                if(select_StageName == "피치 성 외각")
                    PhotonNetwork.LoadLevel("Peach Castle");
            }
        }
    }

    #region 스테이지 선택 버튼 클릭 코드

    public void BtnClick_BrightBtn(GameObject buttonImage)
    {
        buttonImage.GetComponent<Image>().color = brightColor;
    }

    public void BtnClick_DarkBtn(GameObject buttonImage)
    {
        buttonImage.GetComponent<Image>().color = darkColor;
    }

    public void BtnClick_StageSelect()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties["stageName"] != null)
        {
            ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable();
            customRoomProperties.Add("stageName", select_StageName);
            PhotonNetwork.CurrentRoom.SetCustomProperties(customRoomProperties);
        }
    }

    public void BtnClick_Set_StageName(string stage_Name)
    {
        select_StageName = stage_Name;
    }

    #endregion
}
