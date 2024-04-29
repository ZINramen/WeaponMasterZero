using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TalkManager : MonoBehaviour
{
    public GameObject PhoneO;
    public GameObject PhoneX;
    /// 증거 화면 표시
    public bool SelectHide = false;
    public GameObject Menu;
    public GameObject SelectDirect;
    public GameObject Select3;
    public GameObject Select4;

    public Text Select1text;
    public Text Select2text;
    public Text Select3text;
    public Text Select4text;

    public int selectAdd;
    public string[] SelectShow;

    int[] repeat = new int[15];
    /// 위는 전부 선택지 관련 ///////////////////////////////////////////////////////////////////


    public bool sceneLoad = false;
    public bool Action = false;
    public int LoadSceneNumber;
    public int[] actionNumber;

    public AudioSource typingsfx;
    public bool animeFirst = false;
    public Animation illustAni;
    public GameObject View;


    public bool TextStop = false;
    public float Speed = 0.2f;

    public int i = 0;
    public int j = 0;
    public int k = 0;
    public int previousDirection = -1;

    public int[] nameDirection;
    float nx;
    float ny;
    float ix;
    float iy;

    public string[] name_ = new string[1];
    public string[] ContentList = new string[1];

    public Sprite[] illust;
    public Image illustView;

    public Text Name;
    public Text Content;
    public GameObject Button;
    public GameObject nameDirect;
    public GameObject illustDirect;


    Dictionary<int, Talk> Dialogue;
    Dictionary<int, Talk> Dialogue2;

    public void showText(int StartTextNumber)
    {
        if (PhoneO != null)
        {
            PhoneO.SetActive(false);
            PhoneX.SetActive(false);
        }
        StartTextNumber -= 1;
        Menu.SetActive(false);
        if (name_.Length > StartTextNumber)
            i = StartTextNumber;

        if (ContentList.Length > StartTextNumber)
        {
            Content.text = null;
            j = StartTextNumber;
        }
        if (illust.Length > StartTextNumber)
            k = StartTextNumber;


        if (name_.Length > StartTextNumber)
            View.SetActive(true);
        TextNext();
    }

    public void ShiftScene(int speed)
    {
        Thread.Sleep(1000 * speed);
        SceneManager.LoadScene(LoadSceneNumber, LoadSceneMode.Single);
    }
    void NameText()
    {
        if(Dialogue[i]._text != null)
        Name.text = Dialogue[i]._text;
    }
    void contextText()
    {
        if (TextStop == false)
        {
            Debug.Log("Well done.");
            StartCoroutine(Typing(Content,Speed));
            TextStop = true;
        }
    }
    public void TextNext()
    {
        Button.SetActive(false);
        TextStop = false;
        if (i + 1 < name_.Length && ContentList[j + 1] != "B")
            i++;
        else
            i += 2;
        Direction();

        if (ContentList.Length > j+1)
        {
            if (ContentList[j + 1] != "")
            {
                j++;
                if (actionNumber.Length == ContentList.Length)
                {
                    if (actionNumber[j] == 2)
                    {
                        gameObject.transform.parent.gameObject.GetComponent<Animation>().Play("Picture1");
                    }
                }

            }
            else
            {
                View.SetActive(false);
            }
            if (actionNumber.Length == ContentList.Length)
            {
                Action = true;
            }
        }
        else
        {
            View.SetActive(false);
            if (actionNumber.Length == ContentList.Length)
            {
               Action = true;
            }
        }

        if(illust.Length > k+1 && ContentList.Length > j + 1 && ContentList[j + 1] != "B")
        {
            k++;
        }
        else
        {
            k += 2;
        }
    }
    void Direction()
    {
        if (nameDirect && nameDirection.Length > (i))
        {
            if (previousDirection != nameDirection[i])
            {
                previousDirection = nameDirection[i];

                if (nameDirection[i] == 0)
                {
                    if (illustAni && animeFirst == false || nameDirection[i - 1] != 0 && i != 0)
                    {
                        illustAni.Play("Show1");
                        animeFirst = true;
                    }
                    illustDirect.GetComponent<RectTransform>().anchoredPosition = new Vector3(ix, iy);
                    illustDirect.transform.localEulerAngles = new Vector3(0, 0, 0);

                    nameDirect.GetComponent<RectTransform>().anchoredPosition = new Vector3(nx, ny);
                }
                else if (nameDirection[i] != 0)
                {
                    if (illustAni && animeFirst == false || nameDirection[i - 1] == 0 && i != 0)
                    {
                        illustAni.Play("Show2");
                        animeFirst = true;
                    }
                    illustDirect.GetComponent<RectTransform>().anchoredPosition = new Vector3(-ix, iy);
                    illustDirect.transform.localEulerAngles = new Vector3(0, 180, 0);

                    nameDirect.GetComponent<RectTransform>().anchoredPosition = new Vector3(-nx, ny);
                }
            }
        }
    }
    public void animPlay()
    {
        if (illustAni)
        {
            if (nameDirection.Length > i && nameDirection[i] == 0 || nameDirection[nameDirection.Length - 1] == 0)
            {
                illustAni.Play("Show1");
            }
            else
            {

                illustAni.Play("Show2");
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        illustAni = illustDirect.GetComponent<Animation>();
        nx = nameDirect.GetComponent<RectTransform>().anchoredPosition.x;
        ny = nameDirect.GetComponent<RectTransform>().anchoredPosition.y;

        ix = illustDirect.GetComponent<RectTransform>().anchoredPosition.x;
        iy = illustDirect.GetComponent<RectTransform>().anchoredPosition.y;

        Direction();
    }
    // Update is called once per frame
    void Update()
    {
        if (!View.activeSelf) return;
        if (name_.Length == 0 || name_.Length > i && name_[i] == "")
        {
            nameDirect.SetActive(false);
        }
        else
        {
            nameDirect.SetActive(true);
        }
        if (illust.Length == 0 || k < illust.Length && illust[k] == null)
        {
            illustDirect.SetActive(false);
        }
        else
        {
            illustDirect.SetActive(true);
        }
        if (Action == true)
        {
            if (actionNumber[i] == 1) // 프롤로그 애니메이션 엔드
            {
                gameObject.transform.parent.gameObject.GetComponent<Animation>().Play("End");

                Action = false;
            }
            else if (actionNumber[i] == 3) // 메뉴 사용 허가
            {
                Action = false;
                Debug.Log("z");
                Menu.SetActive(true);
            }
        }
        if (sceneLoad == true) // 씬 이동
        {
            ShiftScene(2);
            sceneLoad = false;
        }
        Dialogue = new Dictionary<int, Talk>();
        if (i < name_.Length)
        {
            Dialogue.Add(i, new Talk(name_[i]));
            NameText();
        }

        Dialogue2 = new Dictionary<int, Talk>();
        if (j < ContentList.Length)
        {
            Dialogue2.Add(j, new Talk(ContentList[j]));
            contextText();
        }
        if(k < illust.Length && illust[k] != null)
        {
            illustView.sprite = illust[k];
        }

    }
    IEnumerator Typing(Text typingText, float speed)
    {
        for (int i = 0; i < ContentList[j].Length; i++)
        {
            if(typingsfx != null && i % 2 < 0.02f && View.activeSelf == true)
            {
                typingsfx.Play();
            }
            typingText.text = ContentList[j].Substring(0, i + 1);
            yield return new WaitForSeconds(speed);
        }
        Button.SetActive(true);
    }

}

public class Talk
{
    public string _text;

    public Talk(string text)
    {
        _text = text;
    }

}