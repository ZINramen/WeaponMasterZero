using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore.Text;

public class CutSceneEditor : MonoBehaviour
{
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

    public void DialogueShowUP(string text) 
    {
        dialogue.transform.parent.gameObject.SetActive(true);
        dialogue.Set(text);
    }
    private void Start()
    {
        if(Entity.Player)
            player = Entity.Player.aManager.ani;
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
