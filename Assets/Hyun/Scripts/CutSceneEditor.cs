using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CutSceneEditor : MonoBehaviour
{
    public UnityEvent[] dialogueEndActions;
    public GameObject dialogue;
    public ButtonInstant dialogueStartButton;

    public Animator[] character;



    [SerializeField]
    int currentActionNum = -1;

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
        dialogueStartButton.Restart_Text(pos);
    }

    public void DialogueShowUP() 
    {
        dialogue.SetActive(true);
    }

    private void Update()
    {
        if (dialogue && !dialogue.activeSelf && currentActionNum != -1) 
        {
            dialogueEndActions[currentActionNum].Invoke();
            currentActionNum = -1;
        }
    }
}
