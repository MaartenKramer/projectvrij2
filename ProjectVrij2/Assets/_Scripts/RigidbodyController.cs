using DG.Tweening;
using UnityEngine;

[System.Serializable]
public class RigidbodyController
{
    [SerializeField] private Transform transform;
    public Rigidbody rigidbody;

    public Vector3 Forward { get { return transform.forward; } }
    public Vector3 Right { get { return transform.right; } }
    public Vector3 Up { get { return transform.up; } }
    public Vector3 LinearVelocity { get { return rigidbody.linearVelocity; } }
    public Vector3 Position { get { return transform.position; } set { transform.position = value; } }
    public Vector3 LocalPosition { get { return transform.localPosition; } set { transform.localPosition = value; } }
    public Quaternion Rotation { get { return transform.rotation; } set { transform.rotation = value; } }
    public Vector3 LocalScale { get { return transform.localScale; } set { transform.localScale = value; } }

    public void EnableGravity() { rigidbody.useGravity = true; }
    public void DisableGravity() {  rigidbody.useGravity = false; }

    public void RotateOverTime(float inputX, float inputY, float speed)
    {
        transform.Rotate(Vector3.up * inputX * speed * Time.deltaTime);

        float pitchAmount = inputY * speed * Time.deltaTime;
        Quaternion pitchRotation = Quaternion.AngleAxis(pitchAmount, Vector3.right);
        transform.rotation = pitchRotation * transform.rotation;
    }
    public void RotateTowards(Vector3 direction)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 4f);
    }
    public void Rotate(Vector2 direction, float speed)
    {
        transform.Rotate(direction.y * speed * Time.deltaTime, direction.x * speed * Time.deltaTime, 0f);
    }
}
