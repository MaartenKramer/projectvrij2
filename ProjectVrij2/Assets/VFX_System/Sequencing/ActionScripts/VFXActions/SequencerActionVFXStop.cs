using UnityEngine;
using VFX.Controllers;
using System.Collections;

namespace Sequencing
{
    [CreateAssetMenu(menuName = "Sequence/VFX/Stop", fileName = "New VFXStop")]
    public class SequencerActionVFXStop : SequencerAction
    {
        [SerializeField] private string id;

        public override IEnumerator StartSequence(Sequencer context)
        {
            var sequencer = context as VFX_Sequencer;

            VFX_Controller vfx;
            if (!sequencer.GetController(id, out vfx))
            {
                yield return null;
            }
            else
            {
                vfx.Stop();
            }
        }
    }
}
