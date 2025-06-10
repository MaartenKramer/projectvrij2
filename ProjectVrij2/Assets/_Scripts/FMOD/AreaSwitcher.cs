using UnityEngine;

public class AreaSwitcher : MonoBehaviour
{
    [SerializeField] private int musicAreaIndex = 0;

    private SoundManager soundManager;

    void Start()
    {
        soundManager = Object.FindAnyObjectByType<SoundManager>();

        if (soundManager == null)
        {
            Debug.LogError("No Sound Manager found");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && soundManager != null)
        {
            soundManager.SwitchMusicArea(musicAreaIndex);
            Debug.Log($"Switching to track {musicAreaIndex}");
        }

    }
}