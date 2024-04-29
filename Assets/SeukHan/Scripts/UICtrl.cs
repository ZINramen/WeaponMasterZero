using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class UICtrl : MonoBehaviour
{
    public Image HP_Bar1P;
    public Image HP_Bar2P;
    
    public Image SpecialGauge_1P;
    public Image SpecialGauge_2P;

    public Image[] SkillIcons1P;
    public Image[] SkillIcons2P;

    public Entity Player1;
    public Entity Player2;

    private void Start()
    {
        StartCoroutine(CheckHP());
        if(SpecialGauge_1P)
            StartCoroutine(CheckMP());
    }

    private void Update()
    {
        for (int i = 0; i < 5; i++)
        {
            if(Player1.network)
                SkillIcons1P[i].gameObject.SetActive(Player1.network.SkillActive[i]);
            else 
            {
                if(Player1.aManager.skillManager.skills[i] != "")
                    SkillIcons1P[i].gameObject.SetActive(true);
                else
                    SkillIcons1P[i].gameObject.SetActive(false);
            }

            if (Player2.network)
                SkillIcons2P[i].gameObject.SetActive(Player2.network.SkillActive[i]);
            else
            {
                if (Player2.aManager && Player2.aManager.skillManager.skills[i] != "")
                    SkillIcons2P[i].gameObject.SetActive(true);
                else
                    SkillIcons2P[i].gameObject.SetActive(false);
            }
        }
    }

    //함수로 바꿔서 호출 가능하게 할 예정
    IEnumerator CheckHP()
    {
        var wait = new WaitForSecondsRealtime(0.5f);
        while(true)
        {
            HP_Bar1P.fillAmount = Player1.GetHp() / Player1.maxHP;
            HP_Bar2P.fillAmount = Player2.GetHp() / Player2.maxHP;
            yield return wait;
        }
    }

    IEnumerator CheckMP()
    {
        var wait = new WaitForSecondsRealtime(0.5f);
        while (true)
        {
            SpecialGauge_1P.fillAmount = (float)Player1.GetMp() / Player1.maxMp;
            SpecialGauge_2P.fillAmount = (float)Player2.GetMp() / Player2.maxMp;
            yield return wait;
        }
    }
}
