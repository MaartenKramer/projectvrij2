using UnityEngine;
using Sequencing;

namespace VFX.Controllers
{
    public class VFX_SequenceController : VFX_Controller
    {
        private VFX_Sequencer sequencer;

        protected override void Start()
        {
            base.Start();
            vfxName = this.GetType().Name;

            sequencer = GetComponentInChildren<VFX_Sequencer>();

            sequencer.onComplete.AddListener(OnComplete);

        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            sequencer.onComplete.RemoveListener(TryLoop);
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
        }

        public override void Trigger()
        {
            base.Trigger();

            if (sequencer.SequenceActive) { sequencer.Continue(); }
            else { sequencer.StartSequence(); }
        }
        public override void Stop()
        {
            base.Stop();

            sequencer.Pause();
        }
        public override void Reset()
        {
            base.Reset();
            sequencer.Kill();

            // reset all vfx inside sequence
            sequencer.GetControllers(out VFX_Controller[] c);
            foreach(VFX_Controller controller in c)
            {
                controller.Reset();
            }
        }

        private void OnComplete()
        {
            isPlaying = false;
            TryLoop();
        }

        private void TryLoop()
        {
            Debug.Log($"trying loop!");
            if (isLooping)
            {
                Trigger();
            }
        }
    }
}
