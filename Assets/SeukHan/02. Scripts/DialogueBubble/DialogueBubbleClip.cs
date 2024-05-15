using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class DialogueBubbleClip : PlayableAsset, ITimelineClipAsset
{
    public DialogueBubbleBehaviour template = new DialogueBubbleBehaviour ();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<DialogueBubbleBehaviour>.Create(graph, template);
        return playable;
    }
}
