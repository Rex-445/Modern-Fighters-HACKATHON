using UnityEngine.UI;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityEngine;

[TrackBindingType(typeof(Text))]
[TrackClipType(typeof(SubtitleClip))]
public class SubtitleTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<SubtitleTrackMixer>.Create(graph, inputCount);
    }
}
