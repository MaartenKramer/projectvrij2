using UnityEngine;
using UnityEngine.Events;

public struct CollisionData
{
    public CollisionData(Collision collision)
    {
        this.collision = collision;
    }

    public Collision collision;
}
public class CollisionDetection : MonoBehaviour
{
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private UnityEvent<CollisionData> collisionEvent;

    private void OnCollisionEnter(Collision collision)
    {
        if ((collisionMask.value & (1 << collision.transform.gameObject.layer)) <= 0) { return; } // return if collision is not in layermask

        CollisionData data = new CollisionData(collision);
        collisionEvent.Invoke(data);
        Debug.Log($"[Collision Detection {gameObject.name}] collided! {data.collision.gameObject.name}");
    }
}
