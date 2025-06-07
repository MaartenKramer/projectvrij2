using UnityEngine;

namespace VFX.Controllers
{
    public enum TimingMode { SPEED, DURATION }
    public class VFX_Controller : MonoBehaviour
    {
        [HideInInspector] public string vfxName;
        public string ObjName => gameObject.name;
        public int priority;

        [HideInInspector] public bool isLooping;
        protected bool isPlaying;

        protected virtual void Start()
        {
        }
        protected virtual void OnDestroy()
        {

        }
        protected virtual void Update()
        {
        }

        public virtual void Trigger()
        {
            isPlaying = true;
        }

        public virtual void Stop()
        {
            isPlaying = false;
        }

        public virtual void Reset()
        {
        }

        public virtual void SetLoop()
        {
            isLooping = !isLooping;
        }
    }
}
