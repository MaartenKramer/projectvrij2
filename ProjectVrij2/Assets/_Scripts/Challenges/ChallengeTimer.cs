using UnityEngine;
using TMPro;

public class ChallengeTimer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    public float initialTime = 20f;
    public float timeLeft;
    private bool timerActive = false;

    [SerializeField] private SequenceManager sequenceManager;


    public void InitializeTimer()
    {
        timerActive = true;
        timeLeft = initialTime;
    }

    public void EndTimer()
    {
        timerActive = false;
    }

    void Update()
    {
        if (!timerActive) return;
        timeLeft -= Time.deltaTime;
        int minutes = Mathf.FloorToInt(timeLeft / 60);
        int seconds = Mathf.FloorToInt(timeLeft % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (timeLeft <= 0)
        {
            EndTimer();
            sequenceManager.ResetChallenge();
        }
    }
}
