using UnityEngine;
using UnityEngine.InputSystem;

public class RotateOnDirectionalInput : MonoBehaviour
{
    [SerializeField] InputActionAsset actionAsset;
    InputAction moveAction;

    Vector3 direction;
    [SerializeField] Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = actionAsset.FindActionMap("Player_Bird").FindAction("Move");
    }

    // Update is called once per frame
    void Update()
    {
        direction = moveAction.ReadValue<Vector2>();
        float xDir = direction.x;
        direction.x = direction.y;
        direction.y = xDir;
    }

    private void FixedUpdate()
    {
        Rotate();
    }

    public void Rotate()
    {
        transform.rotation = Quaternion.Euler(direction * 90);
    }
}
