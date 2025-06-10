using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using System.Linq;

namespace Sequencing
{
    public class Sequencer : MonoBehaviour
    {
        [System.Serializable]
        private struct EventItem
        {
            public string id;
            public UnityEvent action;
        }

        private Coroutine sequence;
        public bool SequenceActive => sequence != null;

        [SerializeField] private int channel;
        [Space]
        [SerializeField] private List<SequencerAction> sequenceActions = new List<SequencerAction>();
        [SerializeField] private List<EventItem> actionEvents = new List<EventItem>();
        [Space]
        [SerializeField] public UnityEvent onComplete;

        private bool isPaused = false;

        private void Awake()
        {
            for (int i = 0; i < sequenceActions.Count; i++)
            {
                sequenceActions[i].Initialize(gameObject);
            }
        }

        public void StartSequence()
        {
            if(sequence != null) { Kill(); }
            sequence = StartCoroutine(ExecuteSequence());
        }

        private IEnumerator ExecuteSequence()
        {

            for(int i = 0;i < sequenceActions.Count;i++)
            {
                var action = sequenceActions[i];

                while (isPaused)
                {
                    yield return null;
                }

                yield return StartCoroutine(action.StartSequence(this));
            }

            Kill();
            onComplete?.Invoke();
        }

        public void Pause() { isPaused = true; }
        public void Continue() { isPaused = false; }
        public void Kill()
        {
            if(sequence == null) { return; }

            StopCoroutine(sequence);
            sequence = null;
        }

        public bool GetEvent(string id, out UnityEvent action)
        {
            action = actionEvents.Single(x => x.id == id).action;
            if (action == null) { return false; }
            return true;
        }
        public bool GetEvents(out UnityEvent[] events)
        {
            var items = this.actionEvents;
            List<UnityEvent> contr = new List<UnityEvent>();
            foreach (var item in items)
            {
                contr.Add(item.action);
            }
            events = contr.ToArray();

            if (events.Length == 0) { return false; }
            return true;
        }

        public bool CompareChannel(int input)
        {
            return input == channel;
        }
    }
}
