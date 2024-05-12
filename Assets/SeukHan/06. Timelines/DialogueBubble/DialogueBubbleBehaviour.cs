using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class DialogueBubbleBehaviour : PlayableBehaviour
{
    public string text;
    public string objectName;
    private GameObject fancySpeechBubble;

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        fancySpeechBubble = GameObject.Find(objectName);

        if (fancySpeechBubble != null)
        {
            // Enable the fancySpeechBubble GameObject
            fancySpeechBubble.SetActive(true);

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
            //Debug.LogError("FancySpeechBubble is not found in the scene");
        }
    }
}
