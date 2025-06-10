using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Sequencing
{
    [CreateAssetMenu(menuName = "Sequence/Event", fileName = "New Event")]
    public class SequencerActionEvent : SequencerAction
    {
        [SerializeField] private string id;

        public override IEnumerator StartSequence(Sequencer context)
        {
            UnityEvent uEvent;
            if (!context.GetEvent(id, out uEvent))
            {
                yield return null;
            }
            else
            {
                uEvent?.Invoke();
            }
        }
    }
}
