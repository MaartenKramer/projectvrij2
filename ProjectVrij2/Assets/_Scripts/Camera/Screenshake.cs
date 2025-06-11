using System.Collections;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

[System.Serializable]
public struct NoiseItem
{
    public string id;
    [Space]
    public NoiseSettings settings;
    public float amplitude, frequency, duration;
    public AnimationCurve fadein, fadeout;
}

public class Screenshake : MonoBehaviour
{
    private CinemachineBrain brain;
    [SerializeField] NoiseItem[] noises;

    private float duration;

    private void Awake()
    {
        brain = GetComponent<CinemachineBrain>();
    }


    public void SetDuration(float duration)
    {
        this.duration = duration;
    }

    public void TriggerNoise(string id)
    {
        Debug.Log($"Triggered noise! {id}");

        NoiseItem noiseItem = noises.Single(x => x.id == id);
        if (noiseItem.id == null || noiseItem.id == string.Empty)
        {
            Debug.LogWarning($"[ScreenShake] missing noise! {id}");
            return;
        }

        var activeCamera = brain.ActiveVirtualCamera as CinemachineCamera;
        if(activeCamera == null)
        {
            Debug.LogWarning($"[ScreenShake] no active camera found!");
            return;
        }

        var noiseComponent = activeCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
        if(noiseComponent == null)
        {
            Debug.LogWarning($"[ScreenShake] no noise component on camera found!");
            return;
        }

        SetDuration(noiseItem.duration);
        StartCoroutine(ApplyNoise(noiseComponent, noiseItem, duration));
    }

    private IEnumerator ApplyNoise(CinemachineBasicMultiChannelPerlin noiseComponent, NoiseItem noiseItem, float duration)
    {
        noiseComponent.NoiseProfile = noiseItem.settings;

        float elapsed = 0f;
        float progress = 0f;

        while(elapsed < duration)
        {
            if(elapsed < duration * .5f)
            {
                progress = MyMathUtils.Remap01(elapsed, 0, duration * .5f);
                noiseComponent.AmplitudeGain = noiseItem.fadein.Evaluate(progress) * noiseItem.amplitude;
                noiseComponent.FrequencyGain = noiseItem.fadein.Evaluate(progress) * noiseItem.frequency;
            }
            else
            {
                progress = 1 - MyMathUtils.Remap01(elapsed, duration * 0.5f, duration);
                noiseComponent.AmplitudeGain = noiseItem.fadeout.Evaluate(progress) * noiseItem.amplitude;
                noiseComponent.FrequencyGain = noiseItem.fadeout.Evaluate(progress) * noiseItem.frequency;
            }


            elapsed += Time.deltaTime;

            yield return null;
        }

        noiseComponent.NoiseProfile = null;
    }
}
