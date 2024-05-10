using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public Entity Player;
    public Image HP_Bar1P;
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
        for (int i = 0; i < SkillIcons1P.Length; i++)
        {
            if (Player.aManager.skillManager.skills[i] != "")
                SkillIcons1P[i].gameObject.SetActive(true);
            else
                SkillIcons1P[i].gameObject.SetActive(false);
        }
    }

    //함수로 바꿔서 호출 가능하게 할 예정
    IEnumerator CheckHP()
    {
        var wait = new WaitForSecondsRealtime(0.5f);
        while (true)
        {
            HP_Bar1P.fillAmount = Player.GetHp() / Player.maxHP;
            yield return wait;
        }
    }
}
