using System.Collections;
using UnityEngine;

namespace Sequencing
{
    [CreateAssetMenu(menuName = "Sequence/Time/Wait", fileName = "New Wait")]
    public class SequencerActionWait : SequencerAction
    {
        [SerializeField] private float duration;

        public override IEnumerator StartSequence(Sequencer context)
        {
            yield return new WaitForSeconds(duration);
        }
    }
}
