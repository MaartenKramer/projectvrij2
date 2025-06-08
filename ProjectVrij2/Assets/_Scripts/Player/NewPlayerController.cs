using UnityEngine;
using UnityEngine.InputSystem;

public class NewPlayerController : MonoBehaviour
{
    [SerializeField] private Player player;

    private InputAction interactAction;
    private InputAction debugAction;

    public void Awake()
    {
        player = GetComponent<Player>();

        interactAction = player.InputController.GetActionGlobal("Interact");
        debugAction = player.InputController.GetActionDebug("ShowDebug");

        player.debugVariables = new PlayerDebugVariables();
    }

    private void OnEnable()
    {
        player.InputController.OnEnable();  // does nothing right now...
    }

    private void OnDisable()
    {
        player.InputController.OnDisable(); // does nothing right now...
    }

    public void Start()
    {
    }

    void Update()
    {
        // input
        HandleInput();

        player.ActiveForm?.behaviour.HandleInput();
        player.ActiveForm?.behaviour.UpdateForm();
        player.ActiveForm?.behaviour.HandleAbilities();

        // update debug variable text
        player.debugVariables.velocity = player.RigidbodyController.LinearVelocity.magnitude;
        player.debugVariables.drag = player.RigidbodyController.LinearDrag;

        EventHandler<PlayerDebugVariables>.InvokeEvent(GlobalEvents.UI_DEBUG_UPDATEVARIABLES, player.debugVariables);
    }

    private void FixedUpdate()
    {
        player.ActiveForm?.behaviour.HandlePhysics();
    }

    private void HandleInput()
    {
        if (player.IsDisabled) { return; }

        if (debugAction.triggered)
        {
            EventHandler.InvokeEvent(GlobalEvents.UI_DEBUG_SHOW);
        }
    }

    private void OnDrawGizmos()
    {
        if(player == null) { return; }
        if (player.ActiveForm == null) { return; }
        player.ActiveForm.behaviour.OnDrawGizmos();

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down * (2 * 1.1f));
    }

    public void HandleFormCollision(CollisionData data)
    {
        if (player.ActiveForm == null) { return; }
        player.RigidbodyController.lastRelativeVelocity = data.coll.relativeVelocity;
        player.ActiveForm.behaviour.OnCollision(data);
    }
}

public struct PlayerDebugVariables
{
    public string form;

    public float velocity;
    public float drag;
    public float lift;

    public bool speedingUp;
    public bool slowingDown;

    public bool isGrounded;
    public bool onSlope;
    public float gravity;
}
