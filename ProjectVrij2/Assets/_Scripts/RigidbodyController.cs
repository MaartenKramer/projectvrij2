using DG.Tweening;
using UnityEngine;
using System.Collections;

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
    
    public Vector3 lastRelativeVelocity = Vector3.zero;

    //private float desiredGravity;
    //private float currentGravity;
    //private Tween gTween;
    //private bool customGravity = true;
    private Tween dragTween;
    private float currentPitch = 0f; // This represents the up/down angle in degrees

    #region custom_gravity
    //public void ApplyGravity(float gMultiplier)
    //{
    //    if (customGravity)
    //    {
    //        Vector3 totalGravity = Vector3.down * currentGravity * gMultiplier;
    //        rigidbody.AddForce(totalGravity, ForceMode.Acceleration);
    //    }
    //}
    //public void SetGravity(float gravity)
    //{
    //    desiredGravity = gravity;
    //    currentGravity = gravity;
    //}
    //public void TweenGravity(float gravity, float speed)
    //{
    //    if(gTween != null) { dragTween.Kill(); }

    //    desiredGravity = gravity;

    //    gTween = DOTween.To(() => currentGravity, x => currentGravity = x, desiredGravity, Mathf.Abs(currentGravity - desiredGravity) / speed);
    //}

    //public void EnableCustomGravity() { customGravity = true; }
    //public void DisableCustomGravity() {  customGravity = false; }
    #endregion

    public void SlashVelocity(float multiplier)
    {
        rigidbody.linearVelocity = LinearVelocity * multiplier;
    }

    public void EnableGravity() { rigidbody.useGravity = true; }
    public void DisableGravity() { rigidbody.useGravity = false; }

    public void ResetPitch() 
    {
        currentPitch = 0f;
    }
    public void FreezeRotation() { rigidbody.freezeRotation = true; }
    public void UnfreezeRotation() 
    {  
        rigidbody.rotation = Quaternion.Euler(Rotation.x, 0, Rotation.z);
        rigidbody.freezeRotation = false; 
    }

    public void SetPosition(Vector3 position, bool x, bool y, bool z)
    {
        Vector3 newPos = Vector3.zero;

        if(x) { newPos.x = position.x; }
        else { newPos.x = Position.x; }
        if (y) { newPos.y = position.y; }
        else { newPos.y = Position.y; }
        if (z) { newPos.z = position.z; }
        else { newPos.z = Position.z; }

        transform.position = newPos;
    }

    public void SetDrag(float value) 
    {
        Debug.Log($"Setting drag, {value}");
        if(dragTween != null) { dragTween.Kill(); dragTween = null; }

        if (rigidbody.linearDamping == value) { return; }
        rigidbody.linearDamping = value; 
    }

    /// <summary>
    /// Rotates object in specified direction relative to object
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="speed"></param>
    public void Rotate(Vector3 direction, float speed, bool clamped = false)
    {
        //Debug.Log("Rotating!"); Trophy of Daniel's hubris.
        if (!clamped)
        {
            transform.Rotate(direction.y * speed * Time.deltaTime, direction.x * speed * Time.deltaTime, 0f);
            return;
        }

        // Yaw (left/right) — rotate around Y
        transform.Rotate(0f, direction.x * speed * Time.deltaTime, 0f, Space.Self);

        // Pitch (up/down) — track and clamp manually
        currentPitch += direction.y * speed * Time.deltaTime; // Note the minus: up is usually negative in Unity
        currentPitch = Mathf.Clamp(currentPitch, -80f, 80f);

        // Apply pitch — must isolate it so it doesn't get overridden by yaw
        Vector3 currentEuler = transform.localEulerAngles;
        currentEuler.x = currentPitch < 0 ? currentPitch + 360f : currentPitch; // Convert -80 to 280 etc.
        transform.localEulerAngles = new Vector3(currentEuler.x, transform.localEulerAngles.y, 0f);
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

    /// <summary>
    /// Smoothly tweens the Rigidbody's linear drag to the target value at a given speed.
    /// </summary>
    /// <param name="targetDrag">Target linear drag value.</param>
    /// <param name="speed">Speed in units per second.</param>
    public void TweenLinearDrag(float targetDrag, float speed)
    {
        // Stop any existing tween
        if (dragTween != null && dragTween.IsActive())
            dragTween.Kill();

        float currentDrag = rigidbody.linearDamping;
        float distance = Mathf.Abs(currentDrag - targetDrag);
        float duration = distance / speed;

        // Don't tween if already at target
        if (distance < 0.01f)
        {
            rigidbody.linearDamping = targetDrag;
            return;
        }

        dragTween = DOTween.To(
            () => rigidbody.linearDamping,
            x => rigidbody.linearDamping = x,
            targetDrag,
            duration
        ).SetEase(Ease.Linear);
    }

    public void StopTween()
    {
        if (dragTween != null && dragTween.IsActive())
            dragTween.Kill();
    }

    public bool IsTweening => dragTween != null && dragTween.IsActive();
}
