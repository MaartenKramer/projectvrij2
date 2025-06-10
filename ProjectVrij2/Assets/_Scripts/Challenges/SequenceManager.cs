using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SequenceManager : MonoBehaviour
{
    public List<GameObject> ringSequence = new List<GameObject>();
    public GameObject timer;
    public float ringBuff = 2f;
    private bool challengeBeaten = false;
    private ChallengeTimer challengeTimer;
    private SoundManager soundManager;

    [SerializeField]
    private UnityEvent<RingData> onRingPass;

    [SerializeField]
    private UnityEvent onRingPassAudio;

    [SerializeField]
    private UnityEvent onRingStartAudio;

    [SerializeField]
    private UnityEvent onRingSuccessAudio;

    void Start()
    {
        soundManager = Object.FindAnyObjectByType<SoundManager>();
        if (soundManager != null)
        {
            onRingPassAudio.AddListener(soundManager.PlayRingPass);
            onRingStartAudio.AddListener(soundManager.PlayRingStart);
            onRingSuccessAudio.AddListener(soundManager.PlayRingSuccess);
        }
        else
        {
            Debug.LogError("No Sound Manager found");
        }

        InitializeRings();
        ResetChallenge();
    }

    void InitializeRings()
    {
        for (int i = 0; i < ringSequence.Count; i++)
        {
            Ring ringComponent = ringSequence[i].GetComponent<Ring>();
            challengeTimer = timer.GetComponent<ChallengeTimer>();
            if (ringComponent != null)
            {
                ringComponent.sequenceNumber = i;
                ringComponent.sequenceManager = this;
            }
            else
            {
                Debug.LogError("Missing Ring Component");
            }
        }
    }

    public void RingPassed(int sequenceNumber)
    {
        Debug.Log($"Player has gone through ring #{sequenceNumber}");
        if (sequenceNumber != ringSequence.Count - 1)
        {
            var currentRing = ringSequence[sequenceNumber];
            var nextRing = ringSequence[sequenceNumber + 1];

            Vector3 startPos = currentRing.transform.position;
            Vector3 endPos = nextRing.transform.position;
            var data = new RingData(sequenceNumber, startPos, endPos);
            onRingPass.Invoke(data);
        }

        if (sequenceNumber == 0)
        {
            Debug.Log("Ring Challenge Started!");
            StartChallenge();
            challengeTimer.InitializeTimer();
            onRingStartAudio.Invoke();
        }
        else if (sequenceNumber == ringSequence.Count - 1)
        {
            Debug.Log("Ring Challenge Completed!");
            challengeBeaten = true;
            ResetChallenge();
            challengeTimer.EndTimer();
            onRingSuccessAudio.Invoke();
        }

        else
        {
            challengeTimer.timeLeft += ringBuff;
            onRingPassAudio.Invoke();
        }
    }

    public void StartChallenge()
    {
        for (int i = 1; i < ringSequence.Count; i++)
        {
            ringSequence[i].SetActive(true);
        }
    }
    public void ResetChallenge()
    {
        for (int i = 1; i < ringSequence.Count; i++)
        {
            ringSequence[i].SetActive(false);
        }
        if (challengeBeaten == true)
        {
            ringSequence[0].SetActive(false);
            challengeBeaten = false;
        }
    }
}