using UnityEngine;
using Sequencing;
using System.Collections;

[CreateAssetMenu(menuName ="Sequence/Time/Timescale")]
public class SequencerActionTimescale : SequencerAction
{
    [SerializeField] float timescale;

    public override IEnumerator StartSequence(Sequencer context)
    {
        Time.timeScale = timescale;
        yield return null;
    }
}
