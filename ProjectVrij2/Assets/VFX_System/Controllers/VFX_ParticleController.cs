using UnityEngine;

namespace VFX.Controllers.Particles
{
    public class VFX_ParticleController : VFX_Controller
    {
        private ParticleSystem particles;

        protected override void Start()
        {
            base.Start();
            vfxName = this.GetType().Name;

            particles = gameObject.GetComponent<ParticleSystem>();

            Reset();
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
        }

        public override void Trigger()
        {
            base.Trigger();

            particles.Play();
        }
        public override void Stop()
        {
            base.Stop();

            particles.Stop();
        }
        public override void Reset()
        {
            base.Reset();

            Stop();

            particles.Clear();
        }
        public override void SetLoop()
        {
            base.SetLoop();

            ParticleSystem[] allParticles = gameObject.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem particle in allParticles)
            {
                var main = particle.main;
                main.loop = isLooping;
            }
        }
    }
}
