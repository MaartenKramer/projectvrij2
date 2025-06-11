using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class BirdForm : IFormBehaviour
{
    private FormProfileSO formProfile;
    public FormProfileSO FormProfile => formProfile;

    private StateMachine stateMachine;
    public StateMachine StateMachine => stateMachine;

    private Toggleable toggleable;
    public Toggleable Toggleable => toggleable;

    private InputController inputController;
    public InputController InputController => inputController;

    private RigidbodyController rbController;
    public RigidbodyController RigidbodyController => rbController;

    private GameObject owner;
    public GameObject Owner => owner;

    // unique form variables
    [Header("Data")]
    [SerializeField] private string defaultStateId = "state_player_flight";
    [SerializeField] private string actionMapId = "Player_Bird";
    [SerializeField] private FlightData flightData;
    [SerializeField] private StunData stunData;

    private Vector3 bounceDir;

    public void Initialize(GameObject owner, Toggleable toggleable, RigidbodyController rbController, InputController inputController, FormProfileSO profile)
    {
        // inject data
        formProfile = profile;
        this.owner = owner;
        this.toggleable = toggleable;
        this.rbController = rbController;
        this.inputController = inputController;

        // setup state-machine
        stateMachine = new StateMachine(owner, defaultStateId);
        stateMachine.availableStates.Add("state_player_flight", new FlightState(this, actionMapId, flightData));
        stateMachine.availableStates.Add("state_shared_stunned", new StunnedState(this, stunData, "state_player_flight"));
        //stateMachine.availableStates.Add("state_player_idle", new IdleState(this, rigidBody, "state_player_idle"));

        stateMachine.SetState(defaultStateId);
    }

    public void EnterForm()
    {
        Debug.Log("Entered bird form");
        //rbController.DisableGravity();
        rbController.rigidbody.mass = formProfile.mass;
        rbController.rigidbody.linearDamping = formProfile.linearDrag;
        rbController.rigidbody.angularDamping = formProfile.angularDrag;
        //rbController.UnfreezeRotation();

        // camera
        if (CameraManager.Instance.SwitchCMCam(formProfile.cameraId)) { Debug.Log($"[Human] Succesfully switched to {formProfile.cameraId}"); }
        else { Debug.Log($"[Human] Failed switching to {formProfile.cameraId}! Check the profile camera id"); }

        stateMachine.currentState.EnterState();
        //Debug.Log($"baseSpeed: {baseSpeed}");
    }

    public void UpdateForm()
    {
        stateMachine.currentState.UpdateState();
    }

    public void ExitForm()
    {

        Debug.Log("Exited bird form");

        //rbController.EnableGravity();
        //rbController.FreezeRotation();

        stateMachine.currentState.ExitState();
    }

    public void HandleAbilities()
    {
        stateMachine.currentState.HandleAbilities();
    }

    public void HandleInput()
    {
        stateMachine.currentState.HandleInput();
    }

    public void HandlePhysics()
    {
        // TODO - manual collision detection

        stateMachine.currentState.HandlePhysics();
    }

    public void OnDrawGizmos()
    {
        if (stateMachine == null || stateMachine.currentState == null) { return; }
        stateMachine.currentState.OnDrawGizmos();

        Gizmos.DrawLine(RigidbodyController.Position, RigidbodyController.Position + (bounceDir * 10f));
    }

    public void OnCollision(CollisionData data)
    {
        Collision coll = data.coll;
        if(coll.relativeVelocity.magnitude < stunData.minVelocity) { return; };

        // determine impact strength between 0-1
        float impact = MyMathUtils.Remap01(coll.relativeVelocity.magnitude, stunData.minVelocity, stunData.maxVelocity);

        Debug.Log($"[BirdForm] collided! {coll.gameObject.name}, velocity: {coll.relativeVelocity.magnitude}");
        data.collEvent?.Invoke();

        // determine bounce
        bounceDir = coll.GetContact(0).normal;
        float curved = stunData.speedToBounceCurve.Evaluate(impact);
        float bounceForce = stunData.speedToBounceCurve.Evaluate(impact) * stunData.bounceStrength;
        
        // apply bounce
        RigidbodyController.SlashVelocity(stunData.speedSlashMultiplier);
        RigidbodyController.rigidbody.AddForce(bounceForce * bounceDir, ForceMode.Impulse);
        Debug.Log($"[BirdForm] applied bounce! dir: {bounceDir}, remapped: {impact}, curved: {curved}, force: {bounceForce}, total: {bounceForce * bounceDir}");

        // switch to stunned state
        stateMachine.SwitchState("state_shared_stunned");
    }
}
