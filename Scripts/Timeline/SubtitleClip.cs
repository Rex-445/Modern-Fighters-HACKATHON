using UnityEngine;
using UnityEngine.Playables;

public class SubtitleClip : PlayableAsset
{
    [TextArea(3, 7)]
    public string subtitleText;
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<SubtitleBehaviour>.Create(graph);

        SubtitleBehaviour subtitleBehaviour = playable.GetBehaviour();
        subtitleBehaviour.subtitleText = subtitleText;

        return playable;
    }
}
