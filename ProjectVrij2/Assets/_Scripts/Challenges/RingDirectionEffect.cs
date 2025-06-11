using DG.Tweening;
using UnityEngine;

[System.Serializable]
public struct RingData
{
    public RingData(int ringId, Vector3 startPosition, Vector3 endPosition)
    {
        this.ringId = ringId;
        this.startPosition = startPosition;
        this.endPosition = endPosition;
    }

    public int ringId;
    public Vector3 startPosition;
    public Vector3 endPosition;
}
public class RingDirectionEffect : MonoBehaviour
{
    private int lastRingId;
    private Vector3 startPos;
    private Vector3 endPos;

    [SerializeField] private float speed;

    public void UpdatePosition(RingData data)
    {
        Debug.Log("[RingDirectionEffect] updating position");
        lastRingId = data.ringId;
        startPos = data.startPosition;
        endPos = data.endPosition;

        transform.position = startPos;

        var rotation = Quaternion.LookRotation(endPos - startPos);
        Debug.Log($"[RingDirectionEffect] updating rotation, {rotation}");
        transform.rotation = Quaternion.LookRotation(endPos - startPos);
    }

    public void MoveEffect()
    {
        transform.DOMove(endPos, Vector3.Distance(endPos, startPos) / speed);
    }
}
