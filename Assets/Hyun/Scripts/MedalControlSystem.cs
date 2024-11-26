using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedalControlSystem : MonoBehaviour
{
    public static int hpBeforeChange = -1;
    public int medalNum = 0;
    public GameObject medalPrefab;
    public Entity target;
    bool eventEnd = false;
    
    void Update()
    {
        if(eventEnd) return;
        int result = PlayerPrefs.GetInt("Medal_" + medalNum.ToString(), 0);
        if (!Entity.Player || result != 0)
            return;
        switch (medalNum)
        {
            case 1:
                if (target && Entity.Player.GetHp() <= Entity.Player.maxHP/2 && target.GetHp() <= 0)
                {
                    eventEnd = true;
                }
                break;

            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 8:
            case 9:
            case 10:
            case 12:
            case 13:
            case 14:
            case 15:
            case 16:
            case 17:
            case 18:
                eventEnd = true;
                break;

            case 7:
                if (target && target.GetHp() <= target.maxHP / 2 && Entity.Player.GetHp() <= 0)
                {
                    eventEnd = true;
                }
                break;

            case 11:
                if(hpBeforeChange != -1 && hpBeforeChange <= Entity.Player.maxHP / 2)
                {
                    hpBeforeChange = -1;
                    eventEnd = true;
                }
                break;
        }
        if (eventEnd)
        {
            PlayerPrefs.SetInt("Medal_" + medalNum.ToString(), 1);

            var obj = Instantiate(medalPrefab);
            obj.GetComponentInChildren<Achievement>().medal_Code = medalNum;
        }
    }
}
