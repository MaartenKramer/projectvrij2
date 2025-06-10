using System.Collections;
using UnityEngine;


namespace Sequencing
{
    [CreateAssetMenu(menuName = "Sequence/Debug/Warning", fileName = "New DebugWarning")]
    public class SequencerActionDebugWarning : SequencerAction
    {
        [SerializeField] private string message;

        public override IEnumerator StartSequence(Sequencer context)
        {
            string msg = $"[{context.gameObject}] {message}";
            Debug.LogWarning(msg);
            yield return null;
        }
    }
}
