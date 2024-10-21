using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore.Text;

public class CutSceneEditor : MonoBehaviour
{
    CsvReader csv;
    public UnityEvent[] dialogueEndActions;
    public FancySpeechBubble dialogue;
    // public ButtonInstant dialogueStartButton;
    public Animator player; 

    [SerializeField]
    int currentActionNum = -1;
    public void Start_Player_Animator()
    {
        player.speed = 1;
        Destroy(gameObject);
    }

    public void Character_SetTriggerAnim(int index, string name) 
    {
    }

    public void Character_SetBollAnim(int index, string name, bool con) 
    {
    }

    public void ReserveDialogueEndAction(int actionNum)
    {
        currentActionNum = actionNum;
    }

    public void DialogueShowUP_NewPos(int pos) 
    {
        // dialogueStartButton.Restart_Text(pos);
    }

    public void DialogueShowUP(int textNum) 
    {
        string text = csv.lines[textNum][PlayerPrefs.GetInt("Country_Code", 0)];
        dialogue.transform.parent.gameObject.SetActive(true);
        dialogue.Set(text);
    }
    private void Start()
    {
        if(Entity.Player)
            player = Entity.Player.aManager.ani;
        csv = GetComponent<CsvReader>();
    }
    private void Update()
    {
        if (currentActionNum != -1)
        {
            dialogueEndActions[currentActionNum].Invoke();
            currentActionNum = -1;
        }
    }
}
