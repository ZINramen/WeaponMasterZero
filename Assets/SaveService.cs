using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveService : MonoBehaviour
{
    public List<GameObject> chapterButtonLock;
    public static int currentProgress = 1;
    public int progress = 0;
    private void Awake()
    {
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
    }
}
