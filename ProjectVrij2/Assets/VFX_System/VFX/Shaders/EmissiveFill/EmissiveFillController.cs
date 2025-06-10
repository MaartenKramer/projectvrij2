using System.Collections;
using UnityEngine;

namespace VFX.Controllers.Shaders
{
    public class EmissiveFillController : VFX_ShaderController
    {
        [Header("Interaction")]
        [Range(0f, 1f)]
        private float fillAmount;

        [Header("Settings")]
        [SerializeField] private Texture2D baseMap;
        [Space]
        [Range(0f, 1f)]
        [SerializeField] private float edgeWidth;
        [ColorUsageAttribute(true, true)]
        [SerializeField] private Color edgeColor;
        [ColorUsageAttribute(true, true)]
        [SerializeField] private Color fillColor;
        [Space]
        [SerializeField] private TimingMode timingMode;
        [SerializeField] private float timing = 5f;
        [Space]
        [SerializeField] private Vector2 fillRange = Vector2.up;

        private bool filled = false;
        private Coroutine lerp;

        protected override void Start()
        {
            base.Start();
            vfxName = this.GetType().Name;

            if(baseMap != null) { SetTexture("_BaseMap", baseMap); }

            SetFloat("_FillAmount", fillAmount);
            SetColor("_FillColor", fillColor);

            SetFloat("_EdgeWidth", edgeWidth);
            SetColor("_EdgeColor", edgeColor);

            Reset();
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
        }

        public override void Trigger()
        {
            base.Stop();
            if (filled)
            {
                TriggerLerp(fillAmount, 0f, timing);
            }
            else if (!filled)
            {
                TriggerLerp(fillAmount, 1.1f, timing);
            }
        }
        public override void Stop()
        {
            base.Stop();

            if(lerp == null) { return; }
            StopCoroutine(lerp);
        }
        public override void Reset()
        {
            base.Reset();

            if (lerp == null) { return; }
            StopCoroutine(lerp);
            fillAmount = 0f;
            filled = false;
            SetFloat("_FillAmount", fillAmount);
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
            Debug.Log($"[Lerp] lerp started");
            float elapsed = 0f;

            while (elapsed < duration)
            {
                fillAmount = Mathf.Lerp(start, end, elapsed / duration);
                SetFloat("_FillAmount", fillAmount);
                elapsed += Time.deltaTime;
                yield return null;
            }

            fillAmount = end;
            filled = !filled;
            TryLoop(duration);

            Debug.Log($"[Lerp] lerp ended");
        }
        private IEnumerator LerpFloatAtSpeed(float start, float end, float speed)
        {
            Debug.Log($"[Lerp] lerp started");

            while (Mathf.Abs(fillAmount - end) > 0.01f)  // Use a small tolerance to prevent overshooting
            {
                // Calculate the distance to move per frame based on speed
                fillAmount = Mathf.MoveTowards(fillAmount, end, speed * Time.deltaTime);
                SetFloat("_FillAmount", fillAmount);

                yield return null;
            }

            // Ensure it ends exactly at the target value
            fillAmount = end;
            filled = !filled;
            TryLoop(speed);

            Debug.Log($"[Lerp] lerp ended");
        }

        private void TryLoop(float time)
        {
            if (isLooping)
            {
                Stop();
                if (filled) { TriggerLerp(fillRange.y, fillRange.x, time); }
                else { TriggerLerp(fillRange.x, fillRange.y, time); }
            }
        }
    }
}
