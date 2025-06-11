using FMODUnity;
using FMOD.Studio;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private EventInstance ringStartEvent;
    private EventInstance ringPassEvent;
    private EventInstance ringSuccessEvent;

    void Start()
    {
        ringStartEvent = RuntimeManager.CreateInstance("event:/Ring Start");
        ringPassEvent = RuntimeManager.CreateInstance("event:/Ring Pass");
        ringSuccessEvent = RuntimeManager.CreateInstance("event:/Ring Success");
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

}