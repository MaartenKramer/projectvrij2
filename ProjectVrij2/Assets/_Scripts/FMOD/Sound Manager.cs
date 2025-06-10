using FMODUnity;
using FMOD.Studio;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private EventInstance ringStartEvent;
    private EventInstance ringPassEvent;
    private EventInstance ringSuccessEvent;
    private EventInstance transformEvent;
    private EventInstance musicEvent;

    void Start()
    {
        ringStartEvent = RuntimeManager.CreateInstance("event:/Ring Start");
        ringPassEvent = RuntimeManager.CreateInstance("event:/Ring Pass");
        ringSuccessEvent = RuntimeManager.CreateInstance("event:/Ring Success");
        transformEvent = RuntimeManager.CreateInstance("event:/Transform");

        musicEvent = RuntimeManager.CreateInstance("event:/Music");
        musicEvent.start();
    }

    public void PlayRingStart()
    {
        ringStartEvent.start();
    }

    public void PlayRingPass()
    {
        ringPassEvent.start();
    }

    public void PlayRingSuccess()
    {
        ringSuccessEvent.start();
    }

    public void PlayTransform()
    {
        transformEvent.start();
    }

    public void SwitchMusicArea(int areaIndex)
    {
        float parameterValue = areaIndex == 0 ? 0f : 1f;
        musicEvent.setParameterByName("Area", parameterValue);
    }

    public void SetMusicParameter(string parameterName, float value)
    {
        musicEvent.setParameterByName(parameterName, value);
    }

}