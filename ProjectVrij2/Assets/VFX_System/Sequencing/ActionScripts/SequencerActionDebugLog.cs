using System.Collections;
using UnityEngine;


namespace Sequencing
{
    [CreateAssetMenu(menuName = "Sequence/Debug/Log", fileName = "New DebugLog")]
    public class SequencerActionDebugLog : SequencerAction
    {
        [SerializeField] private string message;

        public override IEnumerator StartSequence(Sequencer context)
        {
            string msg = $"[{context.gameObject}] {message}";
            Debug.Log(msg);
            yield return null;
        }
    }
}
