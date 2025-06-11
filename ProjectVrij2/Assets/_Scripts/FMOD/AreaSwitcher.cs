using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using FMOD;

public class AreaSwitcher : MonoBehaviour
{
    [SerializeField] private int musicAreaIndex = 0;
    [SerializeField] private float transitionDuration = 1.0f;

    private SoundManager soundManager;
    private static int currentGlobalArea = -1;

    void Start()
    {
        soundManager = SoundManager.Instance;
        if (soundManager == null)
        {
            UnityEngine.Debug.LogError("No Sound Manager found");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && soundManager != null)
        {
                soundManager.SwitchMusicArea(musicAreaIndex, transitionDuration);

                currentGlobalArea = musicAreaIndex;
        }
    }
}