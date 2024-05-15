using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0f, 0.2971698f, 0.5943396f)]
[TrackClipType(typeof(DialogueBubbleClip))]
public class DialogueBubbleTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<DialogueBubbleMixerBehaviour>.Create (graph, inputCount);
    }
}
