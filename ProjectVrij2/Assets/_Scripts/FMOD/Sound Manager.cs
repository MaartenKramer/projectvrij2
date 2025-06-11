using FMODUnity;
using FMOD.Studio;
using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    [Header("Wind Control")]
    public Rigidbody playerRigidbody;
    public float maxWindSpeed = 20f;
    public float windSmoothSpeed = 2f;

    private EventInstance ringStartEvent;
    private EventInstance ringPassEvent;
    private EventInstance ringSuccessEvent;
    private EventInstance transformEvent;
    private EventInstance musicEvent;
    private EventInstance forestEvent;

    private Coroutine currentTransition;
    private float currentAreaValue = 0f;
    private bool musicStarted = false;
    private bool forestStarted = false;
    private float currentWindValue = 0f;

    private static SoundManager instance;
    public static SoundManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Object.FindAnyObjectByType<SoundManager>();
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        InitializeAudio();
    }

    void Update()
    {
        UpdateWind();
    }

    void UpdateWind()
    {
        if (playerRigidbody != null && forestEvent.isValid())
        {
            float speed = playerRigidbody.linearVelocity.magnitude;
            float targetWindValue = Mathf.Clamp01(speed / maxWindSpeed);

            currentWindValue = Mathf.MoveTowards(currentWindValue, targetWindValue, windSmoothSpeed * Time.deltaTime);

            forestEvent.setParameterByName("Wind", currentWindValue);
        }
    }

    void InitializeAudio()
    {

        ringStartEvent = RuntimeManager.CreateInstance("event:/Ring Start");
        ringPassEvent = RuntimeManager.CreateInstance("event:/Ring Pass");
        ringSuccessEvent = RuntimeManager.CreateInstance("event:/Ring Success");
        transformEvent = RuntimeManager.CreateInstance("event:/Transform");

        if (!musicEvent.isValid())
        {
            musicEvent = RuntimeManager.CreateInstance("event:/Music");

            if (musicEvent.isValid())
            {
                musicEvent.setParameterByName("Area", 0f);
                currentAreaValue = 0f;

                if (!musicStarted)
                {
                    var result = musicEvent.start();
                    musicStarted = true;
                }
            }
        }

        if (!forestEvent.isValid())
        {
            forestEvent = RuntimeManager.CreateInstance("event:/Forest");

            if (forestEvent.isValid())
            {
                if (!forestStarted)
                {
                    var result = forestEvent.start();
                    forestStarted = true;
                }
            }
        }
    }

    public void PlayRingStart()
    {
        if (ringStartEvent.isValid())
        {
            var result = ringStartEvent.start();
        }
        else
        {
            Debug.LogError("Ring Start Event is not valid!");
        }
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

    public void SwitchMusicArea(int areaIndex, float transitionDuration = 1.0f)
    {
        if (!musicEvent.isValid())
        {
            InitializeAudio();
        }

        float targetValue = areaIndex == 0 ? 0f : 1f;

        currentTransition = StartCoroutine(TransitionAreaParameter(targetValue, transitionDuration));
    }

    private IEnumerator TransitionAreaParameter(float targetValue, float duration)
    {
        float startValue = currentAreaValue;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / duration;

            float smoothStep = normalizedTime * normalizedTime * (3f - 2f * normalizedTime);
            currentAreaValue = Mathf.Lerp(startValue, targetValue, smoothStep);

            musicEvent.setParameterByName("Area", currentAreaValue);

            yield return null;
        }

        currentAreaValue = targetValue;
        musicEvent.setParameterByName("Area", currentAreaValue);
        currentTransition = null;
    }

    public void SetMusicParameter(string parameterName, float value)
    {
        musicEvent.setParameterByName(parameterName, value);
    }

    void OnDestroy()
    {
        if (ringStartEvent.isValid())
        {
            ringStartEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            ringStartEvent.release();
        }

        if (ringPassEvent.isValid())
        {
            ringPassEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            ringPassEvent.release();
        }

        if (ringSuccessEvent.isValid())
        {
            ringSuccessEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            ringSuccessEvent.release();
        }

        if (transformEvent.isValid())
        {
            transformEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            transformEvent.release();
        }

        if (musicEvent.isValid())
        {
            musicEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            musicEvent.release();
        }

        if (forestEvent.isValid())
        {
            forestEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            forestEvent.release();
        }
    }
}