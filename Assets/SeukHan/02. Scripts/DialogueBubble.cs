using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueBubble : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private SpriteRenderer dialogueBubbleSprite; // The SpriteRenderer of the dialogue bubble
    public Transform transformToFollow; // The object that the dialogue bubble should follow

    private void Start()
    {
        DisplayText("안녕하세요 코리안 플레이어");
    }

    void Update()
    {
        if (transformToFollow != null)
        {
            // Set the position of the dialogue bubble to the position of the object it should follow
            transform.position = transformToFollow.position;
        }
    }

    public void DisplayText(string text)
    {
        StartCoroutine(TypeText(text));
    }

    private IEnumerator TypeText(string text)
    {
        dialogueText.text = "";
        foreach (char letter in text.ToCharArray())
        {
            dialogueText.text += letter;
            // Get the sprite's border size
            Vector4 borderSize = dialogueBubbleSprite.sprite.border;
            // Set the size of the dialogue bubble to match the preferred width and height of the text, plus the border size
            dialogueBubbleSprite.size = new Vector2(dialogueText.preferredWidth + borderSize.x + borderSize.z, dialogueText.preferredHeight + borderSize.y + borderSize.w);
            yield return new WaitForSeconds(0.2f);
        }
    }
}
