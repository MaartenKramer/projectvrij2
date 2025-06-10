using UnityEngine;
using System.Collections;
using VFX.Controllers;

namespace Sequencing
{
    [CreateAssetMenu(menuName = "Sequence/VFX/Reset", fileName = "New VFXReset")]
    public class SequencerActionVFXReset : SequencerAction
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
                vfx.Reset();
            }
        }
    }
}
