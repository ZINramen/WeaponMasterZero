using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public Entity Player;
    public Image HP_Bar1P;
    public Image HP_Bar1P_Damaged;
    [Space]
    public Image[] SkillIcons1P;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CheckHP());
    }

    // Update is called once per frame
    void Update()
    {
        if (HP_Bar1P_Damaged.fillAmount != HP_Bar1P.fillAmount)
        {
            HP_Bar1P_Damaged.fillAmount = Mathf.Lerp(HP_Bar1P_Damaged.fillAmount, HP_Bar1P.fillAmount, Time.deltaTime);
        }
        for (int i = 0; i < SkillIcons1P.Length; i++)
        {
            if (Player.aManager.skillManager.skills[i] != "")
                SkillIcons1P[i].gameObject.SetActive(true);
            else
                SkillIcons1P[i].gameObject.SetActive(false);
        }
    }

    //함수로 바꿔서 호출 가능하게 할 예정이 바뀌었다.
    IEnumerator CheckHP()
    {
        var wait = new WaitForSecondsRealtime(0.2f);
        while (true)
        {
            HP_Bar1P.fillAmount = Player.GetHp() / Player.maxHP;
            yield return wait;
        }
    }
}
