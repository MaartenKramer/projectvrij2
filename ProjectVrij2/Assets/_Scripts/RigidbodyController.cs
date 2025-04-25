using UnityEngine;

[System.Serializable]
public class RigidbodyController
{
    [SerializeField] private Transform transform;
    public Rigidbody rigidbody;

    public Vector3 Position { get { return transform.position; } set { transform.position = value; } }
    public Vector3 LocalPosition { get { return transform.localPosition; } set { transform.localPosition = value; } }
    public Quaternion Rotation { get { return transform.rotation; } set { transform.rotation = value; } }
    public Vector3 LocalScale { get { return transform.localScale; } set { transform.localScale = value; } }

    public void EnableGravity() { rigidbody.useGravity = true; }
    public void DisableGravity() {  rigidbody.useGravity = false; }
}
