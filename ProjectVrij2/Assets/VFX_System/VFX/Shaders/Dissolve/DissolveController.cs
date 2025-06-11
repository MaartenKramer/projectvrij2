using System.Collections;
using UnityEngine;
using VFX.Controllers;
using VFX.Controllers.Shaders;

public class DissolveController : VFX_ShaderController
{
    [SerializeField] private float noiseStrength;
    [SerializeField] private float objectHeight = 7.75f;
    [Space]
    [SerializeField] private float edgeWidth = .2f;
    [ColorUsageAttribute(true, true)]
    [SerializeField] private Color edgeColor = Color.red;
    [SerializeField] private Texture2D mainTex;
    [Space]
    [SerializeField] private TimingMode timingMode;
    [SerializeField] private float timing = 5f;          // seconds if on duration mode, units per second if on speed mode


    private float cuttoff;

    private bool visible = true;
    private bool dissolving = false;
    private Coroutine lerp;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();

        if(mainTex != null) { SetTexture("_MainTex", mainTex); }
        else { Debug.LogWarning($"[{gameObject.name}] no main texture assigned!"); }
        SetColor("_EdgeColor", edgeColor);
        SetFloat("_EdgeWidth", edgeWidth);

        cuttoff = objectHeight;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        //Debug.Log($"[Dissolve] updating");

        if (!isPlaying) { return; }

        //Debug.Log($"[Dissolve] is playing");
        
        if (dissolving) { return; }

        //Debug.Log($"[Dissolve] is not dissolving");

        StartDissolve();
    }

    public override void Reset()
    {
        base.Reset();
        cuttoff = objectHeight;
        visible = true;
    }

    private void StartDissolve()
    {
        if (visible)
        {
            TriggerLerp(cuttoff, 0 - edgeWidth, timing);
            visible = false;
        }
        else if (!visible)
        {
            TriggerLerp(cuttoff, objectHeight + edgeWidth, timing);

            visible = true;
        }
    }

    private void TriggerLerp(float start, float end, float time)
    {
        switch (timingMode)
        {
            case TimingMode.DURATION:

                if (lerp != null) { StopCoroutine(lerp); }
                lerp = StartCoroutine(LerpFloatOverDuration(start, end, time));

                break;
            case TimingMode.SPEED:

                if (lerp != null) { StopCoroutine(lerp); }
                lerp = StartCoroutine(LerpFloatAtSpeed(start, end, time));

                break;
        }
    }

    private IEnumerator LerpFloatOverDuration(float start, float end, float duration)
    {
        float elapsed = 0f;
        dissolving = true;
        Debug.Log($"[Lerp] lerp started");

        while (elapsed < duration)
        {
            cuttoff = Mathf.Lerp(start, end, elapsed / duration);
            SetHeight(cuttoff);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Debug.Log($"[Lerp] lerp ended");
        cuttoff = end;
        dissolving = false;
    }

    private IEnumerator LerpFloatAtSpeed(float start, float end, float speed)
    {
        dissolving = true;
        Debug.Log($"[Lerp] lerp started");

        while (Mathf.Abs(cuttoff - end) > 0.01f)  // Use a small tolerance to prevent overshooting
        {
            // Calculate the distance to move per frame based on speed
            cuttoff = Mathf.MoveTowards(cuttoff, end, speed * Time.deltaTime);
            SetHeight(cuttoff);

            yield return null;
        }

        // Ensure it ends exactly at the target value
        cuttoff = end;
        dissolving = false;

        Debug.Log($"[Lerp] lerp ended");
    }



    private void SetHeight(float height)
    {
        Debug.Log($"[Material] Setting height! {height}");
        SetFloat("_CuttoffHeight", height);
        SetFloat("_NoiseStrength", noiseStrength);
    }
}
