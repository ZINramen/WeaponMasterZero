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

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        fancySpeechBubbleCanvas = GameObject.Find(objectName).transform.GetChild(0).gameObject;
        
        if (fancySpeechBubbleCanvas != null)
        {
            // Enable the fancySpeechBubble GameObject
            fancySpeechBubbleCanvas.SetActive(true);
            
            GameObject fancySpeechBubble = fancySpeechBubbleCanvas.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;

            if(fancySpeechBubble.GetComponent<FancySpeechBubble>() == null)
            {
                Debug.LogError("FancySpeechBubble does not have FancySpeechBubble component");
                return;
            }
            else
            {
                fancySpeechBubble.GetComponent<FancySpeechBubble>().Set(text);
            }
        }
        else
        {
            Debug.LogError("FancySpeechBubble is not found in the scene");
        }
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (fancySpeechBubbleCanvas != null)
        {
            // Disable the fancySpeechBubble GameObject
            fancySpeechBubbleCanvas.SetActive(false);
        }
    }
}