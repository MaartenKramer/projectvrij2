using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class HumanForm : IFormBehaviour
{
    private FormProfileSO formProfile;
    public FormProfileSO FormProfile => formProfile;

    private StateMachine stateMachine;
    public StateMachine StateMachine => stateMachine;

    private InputController inputController;
    public InputController InputController => inputController;

    private RigidbodyController rbController;
    public RigidbodyController RigidbodyController => rbController;

    private GameObject owner;

    // unique form variables
    [SerializeField] private string defaultStateId = "state_player_terrestrial";
    [SerializeField] private string actionMapId = "Player_Human";
    [SerializeField] private TerrestrialData data;

    public void Initialize(GameObject owner, RigidbodyController rbController, InputController inputController, FormProfileSO profile)
    {
        // inject data
        formProfile = profile;
        this.owner = owner;
        this.rbController = rbController;
        this.inputController = inputController;

        // setup state-machine
        stateMachine = new StateMachine(owner, defaultStateId);
        stateMachine.availableStates.Add("state_player_terrestrial", new TerrestrialState(this, "Player_Human", data));

        stateMachine.SetState(defaultStateId);
    }

    public void EnterForm()
    {
        Debug.Log("Entered human form");
        rbController.rigidbody.mass = formProfile.mass;
        rbController.rigidbody.linearDamping = formProfile.linearDrag;
        rbController.rigidbody.angularDamping = formProfile.angularDrag;

        // camera
        if (CameraManager.Instance.SwitchCMCam(formProfile.cameraId)) { Debug.Log($"[Human] Succesfully switched to {formProfile.cameraId}"); }
        else { Debug.Log($"[Human] Failed switching to {formProfile.cameraId}! Check the profile camera id"); }

        stateMachine.currentState.EnterState();
    }

    public void UpdateForm()
    {
        stateMachine.currentState.UpdateState();
    }

    public void ExitForm()
    {
        Debug.Log("Exited human form");
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

    public void OnDrawGizmos()
    {
        if(stateMachine == null || stateMachine.currentState == null) { return; }
        stateMachine.currentState.OnDrawGizmos();
    }
}
