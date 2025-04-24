using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class BirdBehaviour : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] private float baseSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float drag = 1f;
    [SerializeField] private float gravity = 9.81f;

    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private InputActionAsset actionAsset;

    InputAction moveAction;
    Vector3 direction;

    [Header("Debug")]
    [SerializeField] private Vector3 totalForce;
    [HideInInspector] public Vector3 TotalForce => totalForce;

    [HideInInspector] public float Velocity => totalForce.magnitude;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        InputActionMap actionMap = actionAsset.FindActionMap("Player_Bird");
        moveAction = actionMap.FindAction("Move");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    private Vector3 GetDirection()
    {
        //Vector3 dir = moveAction.ReadValue<Vector2>();
        //dir = new Vector3(dir.x, 0, dir.y);
        //dir = CameraUtils.ConvertToCameraSpace(dir, false);
        //Debug.Log($"direction: {dir}");
        //return dir;

        Vector3 input = moveAction.ReadValue<Vector2>();
        
        Vector3 leftRight = Vector3.zero;
        if(input.x < 0) { leftRight = Vector3.left; }
        else if(input.x > 0) { leftRight = Vector3.right; }

        Vector3 upDown = Vector3.zero;
        if(input.y < 0) { upDown = Vector3.down; }
        else if(input.y > 0) { upDown = Vector3.up; }

        leftRight = CameraUtils.ConvertToCameraSpace(leftRight, false);
        upDown = CameraUtils.ConvertToCameraSpace(upDown, false);
        Debug.Log($"upDown: {upDown}");

        return (leftRight + upDown).normalized * turnSpeed;
    }

    private Vector3 GetBaseSpeed() 
    {
        return CameraUtils.ConvertToCameraSpace(Vector3.forward, false).normalized * baseSpeed;

    }

    private void FixedUpdate()
    {
        // apply drag
        if(moveAction.ReadValue<Vector2>() == Vector2.zero)
        {
            rb.AddForce(-rb.linearVelocity.normalized * drag);
        }

        // apply gravity
        rb.AddForce(Vector3.down * gravity, ForceMode.Force);

        totalForce = GetBaseSpeed() + GetDirection();

        rb.AddForce(totalForce);    
    }

    private void OnDrawGizmos()
    {
        Ray ray = new Ray(transform.position, totalForce);
        Gizmos.DrawRay(ray);
    }
}
