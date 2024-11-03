using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public Entity Player;
    public SkillManager skills;

    public Image HP_Bar1P;
    public Image HP_Bar1P_Damaged;

    public Image MP_Bar;

    [Space]
    public GameObject[] Weapon1Icons;
    public GameObject[] Weapon2Icons;
    public GameObject[] Weapon3Icons;
    public GameObject[] GaugeIcons;

    public Animation currentWeaponUIAnimation;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CheckHP());
        StartCoroutine(CheckMP());

        if (PlayerPrefs.GetInt("key1", (int)KeyCode.Q) != (int)KeyCode.Q)
            transform.GetChild(1).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = "";

        if (PlayerPrefs.GetInt("key2", (int)KeyCode.E) != (int)KeyCode.E)
            transform.GetChild(1).GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (HP_Bar1P_Damaged.fillAmount != HP_Bar1P.fillAmount)
        {
            HP_Bar1P_Damaged.fillAmount = Mathf.Lerp(HP_Bar1P_Damaged.fillAmount, HP_Bar1P.fillAmount, Time.deltaTime);
        }
    }

    public void ChangeCurrentWeapon(int weapon)
    {
        if (weapon == 0) return;
        else
        {
            currentWeaponUIAnimation.clip = currentWeaponUIAnimation.GetClip("ChangeWeapon");
            currentWeaponUIAnimation.Play();
        }
        int weapon2 = 0;

        for (int i = 0; i < 3; i++)
        {
            Weapon1Icons[i].SetActive(false);
            Weapon2Icons[i].SetActive(false);
            Weapon3Icons[i].SetActive(false);
        }
        Weapon1Icons[weapon - 1].SetActive(true);
        for (int i = 0; i < 3; i++)
        {
            if (i != (weapon - 1))
            {
                if (skills && skills.haveWeaponNum-1 >= i)
                {
                    Weapon2Icons[i].SetActive(true);
                }
                weapon2 = i;
                break;
            }
        }
        for (int i = 0; i < 3; i++)
        {
            if ((i != (weapon - 1)) && (i != weapon2))
            {
                if (skills && skills.haveWeaponNum-1 >= i)
                {
                    Weapon3Icons[i].SetActive(true);
                }
                break;
            }
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
    IEnumerator CheckMP()
    {
        var wait = new WaitForSecondsRealtime(0.2f);
        while (true)
        {
            MP_Bar.fillAmount = (float)Player.GetMp() / Player.maxMp;
            yield return wait;
        }
    }
}
