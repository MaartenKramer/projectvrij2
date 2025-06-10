using UnityEngine;
using UnityEngine.Events;

public struct CollisionData
{
    public CollisionData(Collision collision, UnityEvent collEvent = null)
    {
        this.coll = collision;
        this.collEvent = collEvent;
    }

    public Collision coll;
    public UnityEvent collEvent;
}
public class CollisionDetection : MonoBehaviour
{
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private UnityEvent<CollisionData> collisionEvent;
    [SerializeField] private UnityEvent onCollisionHandled;

    private void OnCollisionEnter(Collision collision)
    {
        if ((collisionMask.value & (1 << collision.transform.gameObject.layer)) <= 0) { return; } // return if collision is not in layermask

        CollisionData data = new CollisionData(collision, onCollisionHandled);
        collisionEvent.Invoke(data);
        Debug.Log($"[Collision Detection {gameObject.name}] collided! {data.coll.gameObject.name}");
    }
}
