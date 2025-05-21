using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SequenceManager : MonoBehaviour
{
    public List<GameObject> ringSequence = new List<GameObject>();
    public GameObject timer;
    public float ringBuff = 2f;
    private ChallengeTimer challengeTimer;  
    
    void Start()
    {
        InitializeRings();
    }
    
    void InitializeRings()
    {
        for (int i = 0; i < ringSequence.Count; i++)
        {
            Ring ringComponent = ringSequence[i].GetComponent<Ring>();
            challengeTimer = timer.GetComponent<ChallengeTimer>();

            if (ringComponent != null)
            {
                ringComponent.sequenceNumber = i + 1;
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
        Debug.LogWarning($"Player has gone through ring #{sequenceNumber}");

        if (sequenceNumber == 1)
        {
            Debug.LogWarning("Ring Challenge Started!");
            challengeTimer.InitializeTimer();

        }
        
                        
        else if (sequenceNumber == ringSequence.Count)
        {
            Debug.LogWarning("Ring Challenge Completed!");
            challengeTimer.EndTimer();
        }


        else
        {
            challengeTimer.timeLeft += ringBuff;
        }
    }
}