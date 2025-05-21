using UnityEngine;
using TMPro;

public class ChallengeTimer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    public float timeLeft = 20f;
    private bool timerActive = false;

    public void InitializeTimer()
    {
        timerActive = true;
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
    }
}
