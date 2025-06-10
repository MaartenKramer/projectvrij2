using UnityEngine;

public class Ring : MonoBehaviour
{
    public SequenceManager sequenceManager;
    public int sequenceNumber;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (sequenceManager != null)
            {
                sequenceManager.RingPassed(sequenceNumber);
            }
            else
            {
                Debug.LogError("No Sequence Manager has been assigned.");
            }
        }
    }
}