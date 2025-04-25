using UnityEngine;

public class BirdForm : IFormBehaviour
{
    private FormProfileSO formProfile;
    public FormProfileSO FormProfile => formProfile;

    private StateMachine stateMachine;
    public StateMachine StateMachine => stateMachine;

    private GameObject owner;
    [SerializeField] private string defaultStateId = "state_player_idle";
    [SerializeField] private float baseSpeed;
    [SerializeField] private float baseSprintSpeed;
    [SerializeField] private float baseTurnSpeed;
    [SerializeField] private Rigidbody rb;

    public void Initialize(GameObject owner, FormProfileSO profile)
    {
        // inject data
        formProfile = profile;
        this.owner = owner;

        // setup state-machine
        stateMachine = new StateMachine(owner, defaultStateId);
        stateMachine.availableStates.Add("state_player_idle", new IdleState(stateMachine, this, "state_player_idle"));

        stateMachine.SetState(defaultStateId);
    }

    public void EnterForm()
    {
        Debug.Log("Entered bird form");
        stateMachine.currentState.EnterState();
    }

    public void UpdateForm()
    {
        stateMachine.currentState.UpdateState();
    }

    public void ExitForm()
    {
        Debug.Log("Exited bird form");
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
        stateMachine.currentState.HandlePhysics();
    }
}
