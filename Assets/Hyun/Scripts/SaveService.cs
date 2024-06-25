using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveService : MonoBehaviour
{
    [Header("Medal Part")]
    public static int number_of_medals = 15;
    public List<int> medalList = new List<int>();
    public Text MedalShow;
    [Space]
    public List<GameObject> chapterButtonLock = new List<GameObject>();
    public static int currentProgress = 1;
    public int progress = 0;
    [Space]
    public Transform medalContent;

    private void Awake()
    {
        // 도전 과제 데이터 불러오기
        for(int i = 0; i < number_of_medals; i++)
        {
            int result = PlayerPrefs.GetInt("Medal_" + i.ToString(), 0);
            if(result != 0)
            {
                medalList.Add(i);
            }
        }

        if (MedalShow)
        {
            MedalShow.text = "Achievements (" + medalList.Count + "/15)";
        }

        // 챕터 데이터 저장하기
        if (progress != 0 && progress > PlayerPrefs.GetInt("cur_chapter", 1))
        {
            currentProgress = progress;
            PlayerPrefs.SetInt("cur_chapter", progress);
        }
        else
            currentProgress = PlayerPrefs.GetInt("cur_chapter", 1);

    }
    private void Start()
    {
        //PlayerPrefs.DeleteAll();
        int idx = 2;
        // 챕터 버튼 On, Off 결정
        foreach (GameObject obj in chapterButtonLock)
        {
            if (idx <= PlayerPrefs.GetInt("cur_chapter", 1))
            {
                obj.transform.parent.GetComponent<Button>().enabled = true;
                obj.SetActive(false);
            }
            else
            {
                obj.transform.parent.GetComponent<Button>().enabled = false;
                obj.SetActive(true);
            }
            idx++;
        }

        if (medalContent)
        {
            int i = 1;
            foreach (var item in medalContent.GetComponentsInChildren<Achievement>())
            {
                item.medal_Code = i;
                if (item.save.medalList.Find(x => x == item.medal_Code) == 0)
                {
                    item.gameObject.SetActive(false);
                }
                i++;
            }
        }
    }
}
