using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonInstant : MonoBehaviour, IPointerDownHandler
{
    public TalkManager talkM;
    public bool StartText = false;
    public bool NextText = false;
    public int StartTextNumber = 0;
    public void OnPointerDown(PointerEventData eventData)
    {
        NextTextStart();
    }
    void NextTextStart()
    {
        if (StartText == true && talkM)
        {
            talkM.gameObject.SetActive(true);
            if (talkM.name_.Length > StartTextNumber)
                talkM.i = StartTextNumber;

            if (talkM.ContentList.Length > StartTextNumber)
            {
                talkM.Content.text = null;
                talkM.j = StartTextNumber;
            }
            if (talkM.illust.Length > StartTextNumber)
                talkM.k = StartTextNumber;


            if (talkM.name_.Length > StartTextNumber)
                talkM.View.SetActive(true);
        }
        if (NextText == true && talkM)
        {
            talkM.TextNext();
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            NextTextStart();
        }
    }
    public void Restart_Text(int start)
    {
        Debug.Log("DD");
        StartTextNumber = start;

        talkM.previousDirection = -1;
        talkM.gameObject.SetActive(true);
        if (talkM.name_.Length > StartTextNumber)
            talkM.i = StartTextNumber;

        if (talkM.ContentList.Length > StartTextNumber)
        {
            talkM.Content.text = null;
            talkM.j = StartTextNumber;
        }
        if (talkM.illust.Length > StartTextNumber)
            talkM.k = StartTextNumber;


        if (talkM.name_.Length > StartTextNumber)
            talkM.View.SetActive(true);
    }
}
