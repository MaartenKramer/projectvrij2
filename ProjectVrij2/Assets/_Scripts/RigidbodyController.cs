using DG.Tweening;
using UnityEngine;
using UnityEngine.Windows;

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

    //private float desiredGravity;
    //private float currentGravity;
    //private Tween gTween;
    //private bool customGravity = true;

    private float desiredDrag;
    private float currentDrag;
    private float startDrag;
    private float dragLerpSpeed;
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

    #region custom_forces

    private float accumulatedForce; 
    private float accumulatedRelativeForce; 
    public void AddForce(float forceMagnitude) { accumulatedForce += forceMagnitude; }
    public void ApplyForce(Vector3 dir, ForceMode mode) 
    { 
        rigidbody.AddForce(dir * accumulatedForce, mode);
        accumulatedForce = 0f;
    }

    public void AddRelativeForce(float forceMagnitude) { accumulatedRelativeForce += forceMagnitude; }
    public void ApplyRelativeForce(Vector3 dir, ForceMode mode)
    {
        rigidbody.AddRelativeForce(dir * accumulatedRelativeForce, mode);
        accumulatedForce = 0f;
    }

    #endregion

    public void EnableGravity() { rigidbody.useGravity = true; }
    public void DisableGravity() { rigidbody.useGravity = false; }

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

    public void SetDesiredDrag(float target, float speed) 
    {
        if(target == desiredDrag) { return; }
        Debug.Log($"[Drag] Setting desired drag: {target}, speed: {speed}");
        desiredDrag = target; 
        dragLerpSpeed = speed; 
    }
    public void SetDesiredDragSpeed(float speed)
    {
        dragLerpSpeed = speed;
    }
    public void LerpDrag()
    {
        Debug.Log($"[Lerping drag] current: {currentDrag} | desired: {desiredDrag} | speed: {dragLerpSpeed} | rigidbody: {LinearDrag}");
        if(currentDrag > desiredDrag - .05f && currentDrag < desiredDrag + .05f)
        {
            if(currentDrag != desiredDrag) { currentDrag = desiredDrag; }
        }
        else
        {
            currentDrag = Mathf.Lerp(currentDrag, desiredDrag, dragLerpSpeed * Time.deltaTime);
        }

        rigidbody.linearDamping = currentDrag;
    }

    /// <summary>
    /// Rotates object in specified direction relative to object
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="speed"></param>
    public void Rotate(Vector3 direction, float speed, bool clamped = false)
    {
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
}
