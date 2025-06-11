using UnityEngine;
using VFX.Controllers;
using static UnityEngine.ParticleSystem;

public class VFX_TrailController : VFX_Controller
{
    private TrailRenderer trailRenderer;

    protected override void Start()
    {
        base.Start();
        vfxName = this.GetType().Name;

        trailRenderer = gameObject.GetComponent<TrailRenderer>();

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

        trailRenderer.enabled = true;
    }
    public override void Stop()
    {
        base.Stop();

        trailRenderer.enabled = false;
    }
    public override void Reset()
    {
        base.Reset();

        Stop();

        trailRenderer.Clear();
    }
}
