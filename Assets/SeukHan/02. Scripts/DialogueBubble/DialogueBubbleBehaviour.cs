using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class DialogueBubbleBehaviour : PlayableBehaviour
{
    public string text;
    public string objectName;
    private GameObject fancySpeechBubbleCanvas;
    private GameObject fancySpeechBubble;
    private GameObject fancySpeechBubbleText;
    
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        fancySpeechBubbleCanvas = GameObject.Find(objectName).transform.GetChild(0).gameObject;
        
        if (fancySpeechBubbleCanvas != null)
        {
            fancySpeechBubble = fancySpeechBubbleCanvas.transform.GetChild(0).gameObject;
            fancySpeechBubbleText = fancySpeechBubble.transform.GetChild(0).gameObject;
            
            // Enable the fancySpeechBubble GameObject
            fancySpeechBubble.SetActive(true);
            fancySpeechBubbleText.SetActive(true);

            if(fancySpeechBubbleText.GetComponent<FancySpeechBubble>() == null)
            {
                Debug.LogError("FancySpeechBubble does not have FancySpeechBubble component");
                return;
            }
            else
            {
                fancySpeechBubbleText.GetComponent<FancySpeechBubble>().Set(text);
            }
        }
        else
        {
            Debug.LogError("FancySpeechBubble is not found in the scene");
        }
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (fancySpeechBubbleText != null)
        {
            // Disable the fancySpeechBubble GameObject
            fancySpeechBubble.SetActive(false);
            fancySpeechBubbleText.SetActive(false);
        }
    }
}