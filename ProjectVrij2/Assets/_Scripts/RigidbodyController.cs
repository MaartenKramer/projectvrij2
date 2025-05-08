using DG.Tweening;
using UnityEngine;

[System.Serializable]
public class RigidbodyController
{
    [SerializeField] private Transform transform;
    public Transform orientation;
    public Rigidbody rigidbody;

    public Vector3 Forward { get { return transform.forward; } }
    public Vector3 Right { get { return transform.right; } }
    public Vector3 Up { get { return transform.up; } }
    public Vector3 LinearVelocity { get { return rigidbody.linearVelocity; } }
    public float LinearDrag { get { return rigidbody.linearDamping; } }
    public float Mass { get { return rigidbody.mass; } }
    public Vector3 Position { get { return transform.position; } set { transform.position = value; } }
    public Vector3 LocalPosition { get { return transform.localPosition; } set { transform.localPosition = value; } }
    public Quaternion Rotation { get { return transform.rotation; } set { transform.rotation = value; } }
    public Vector3 LocalScale { get { return transform.localScale; } set { transform.localScale = value; } }

    private float desiredDrag;
    private float currentDrag;
    private Tween dragTween;

    public void EnableGravity() { rigidbody.useGravity = true; }
    public void DisableGravity() {  rigidbody.useGravity = false; }

    public void FreezeRotation() { rigidbody.freezeRotation = true; }
    public void UnfreezeRotation() 
    {  
        rigidbody.rotation = Quaternion.Euler(Rotation.x, 0, Rotation.z);
        rigidbody.freezeRotation = false; 
    }

    public void SetDrag(float value) 
    {
        if(dragTween != null) { dragTween.Kill(); }

        if (rigidbody.linearDamping == value) { return; }
        rigidbody.linearDamping = value; 
    }
    public void TweenDrag(float endValue, float speed)
    {
        if(dragTween != null) { dragTween.Kill(); }

        desiredDrag = endValue;

        dragTween = DOTween.To(() => currentDrag, x => currentDrag = x, desiredDrag, Mathf.Abs(currentDrag - desiredDrag) / speed);

        rigidbody.linearDamping = currentDrag;
    }

    /// <summary>
    /// Rotates object in specified direction relative to object
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="speed"></param>
    public void Rotate(Vector3 direction, float speed)
    {
        transform.Rotate(direction.y * speed * Time.deltaTime, direction.x * speed * Time.deltaTime, 0f);
    }
    /// <summary>
    /// Rotates object in specified direction
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="speed"></param>
    public void RotateTowards(Vector3 direction, float speed)
    {
        Vector3 newForward = Vector3.Slerp(transform.forward, direction.normalized, Time.deltaTime * speed);
        transform.forward = new Vector3(newForward.x, 0, newForward.z);
    }
}
