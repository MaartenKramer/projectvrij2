using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlayerController : Toggleable
{
    [SerializeField] private FormProfileSO currentFormProfile;

    [SerializeField] private List<FormProfileSO> availableForms = new List<FormProfileSO>();
    [SerializeField] private int currentFormIndex = 0;

    [Header("Input")]
    private InputAction transformAction;
    private InputAction interactAction;

    private InputAction showDebugAction;
    //[SerializeField] private InputActionAsset globalInput;

    [Header("References")]
    [SerializeField] private RigidbodyController rbController;
    [SerializeField] private InputController inputController;

    [Header("Events")]

    [Header("Debugging")]
    public PlayerDebugVariables debugVariables;
    [SerializeField] public int startingForm;

    private void Awake()
    {
        // give form scripts acces to important variables from their respective profile
        foreach (var form in availableForms) { form.behaviour.Initialize(gameObject, this, rbController, inputController, form); }

        transformAction = inputController.GetActionGlobal("Transform");
        interactAction = inputController.GetActionGlobal("Interact");
        showDebugAction = inputController.GetActionDebug("ShowDebug");

        debugVariables = new PlayerDebugVariables();
    }

    private void OnEnable()
    {
        inputController.OnEnable();
    }

    private void OnDisable()
    {
        inputController.OnDisable();
    }

    void Start()
    {
        currentFormIndex = 0;
        currentFormProfile = availableForms[currentFormIndex]; // first form in the list is considered the default

        SwitchForm(startingForm);
        CameraManager.Instance.SwitchCMCam(currentFormProfile.cameraId);
    }

    void Update()
    {
        // input
        HandleInput();

        currentFormProfile?.behaviour.HandleInput();
        currentFormProfile?.behaviour.UpdateForm();
        currentFormProfile?.behaviour.HandleAbilities();

        // update debug variable text
        debugVariables.velocity = rbController.LinearVelocity.magnitude;
        debugVariables.drag = rbController.LinearDrag;

        EventHandler<PlayerDebugVariables>.InvokeEvent(GlobalEvents.UI_DEBUG_UPDATEVARIABLES, debugVariables);
    }

    private void FixedUpdate()
    {
        currentFormProfile?.behaviour.HandlePhysics();
    }

    private void HandleInput()
    {
        if (IsDisabled) { return; }

        if (transformAction.triggered)
        {
            Debug.Log($"transform action; got key down");
            if (transformAction.ReadValue<float>() < 0) { CycleForms(false); Debug.Log($"transform action triggered! value: {transformAction.ReadValue<float>()}"); }
            else if (transformAction.ReadValue<float>() > 0) { CycleForms(true); Debug.Log($"transform action triggered! value: {transformAction.ReadValue<float>()}"); }
        }

        if (showDebugAction.triggered)
        {
            EventHandler.InvokeEvent(GlobalEvents.UI_DEBUG_SHOW);
        }
    }

    public void CycleForms(bool forward)
    {
        int newIndex = currentFormIndex;
        if (forward) { newIndex++; }
        else { newIndex--; }

        if(newIndex < 0) { newIndex = availableForms.Count - 1; }
        else if(newIndex >= availableForms.Count) { newIndex = 0; }

        currentFormIndex = newIndex;
        SwitchForm(currentFormIndex);
    }

    public void SwitchForm(FormProfileSO profile)
    {
        Debug.Log("--------------------------------------");

        // determine if this profile is available. If it is, determine it's index
        int desiredIndex = -1;
        for(int i = 0; i < availableForms.Count; i++)
        {
            if (availableForms[i] != profile) { continue; }
            desiredIndex = i;
            Debug.Log($"profile {profile.formName}/{profile.id} found at index {desiredIndex} of available forms!");
        }
        if(desiredIndex == -1) 
        { 
            Debug.Log($"No corresponding index for {profile.formName}/{profile.id} found! <br> Is it missing from the available forms?");
            return;
        }

        IState previousState = currentFormProfile.behaviour.StateMachine.currentState;
        currentFormProfile?.behaviour.ExitForm();
        currentFormProfile = profile;
        if (previousState.IsUnique)
        {
            if (currentFormProfile.behaviour.StateMachine.availableStates.ContainsValue(currentFormProfile.behaviour.StateMachine.currentState))
            {
                // if new form also has the same unique state, set it to that
                currentFormProfile.behaviour.StateMachine.SetState(previousState);
                Debug.Log($"Setting movement state to unique state: {previousState.StateId}");
            }
            else
            {
                // if new form does not have the same unique state, change it to it's specified transition state
                // (e.g. if you are flying as a bird and transform into something flightless, you transition to a falling state instead of the last known state)
                string newStateId = currentFormProfile.behaviour.StateMachine.currentState.StateTransitionId;
                currentFormProfile.behaviour.StateMachine.SetState(newStateId);
                Debug.Log($"Setting movement state to transition state: {newStateId}");
            }
        }
        else
        {
            // if previous state is not unique this form has it, so change it to that
            currentFormProfile.behaviour.StateMachine.SetState(previousState);
            Debug.Log($"Setting movement state to general state: {previousState.StateId}");
        }
        currentFormProfile?.behaviour.EnterForm();

        currentFormIndex = desiredIndex;
        CursorUtils.SetCursor(currentFormProfile.cursorSettings);

        debugVariables.form = currentFormProfile.formName;
        EventHandler<string>.InvokeEvent(GlobalEvents.PLAYER_FORM_CHANGED, currentFormProfile.id);

        Debug.Log("--------------------------------------");
    }
    public void SwitchForm(int profileIndex)
    {
        Debug.Log("--------------------------------------");

        IState previousState = currentFormProfile.behaviour.StateMachine.currentState;
        currentFormProfile?.behaviour.ExitForm();
        currentFormProfile = availableForms[profileIndex];
        if (previousState.IsUnique)
        {
            if (currentFormProfile.behaviour.StateMachine.availableStates.ContainsValue(currentFormProfile.behaviour.StateMachine.currentState))
            {
                // if new form also has the same unique state, set it to that
                currentFormProfile.behaviour.StateMachine.SetState(previousState.StateId);
                Debug.Log($"Setting movement state to unique state: {previousState.StateId}");
            }
            else
            {
                // if new form does not have the same unique state, change it to it's specified transition state
                // (e.g. if you are flying as a bird and transform into something flightless, you transition to a falling state instead of the last known state)
                string newStateId = currentFormProfile.behaviour.StateMachine.currentState.StateTransitionId;
                currentFormProfile.behaviour.StateMachine.SetState(newStateId);
                Debug.Log($"Setting movement state to transition state: {newStateId}");
            }
        }
        else
        {
            // if previous state is not unique this form has it, so change it to that
            currentFormProfile.behaviour.StateMachine.SetState(previousState.StateId);
            Debug.Log($"Setting movement state to general state: {previousState.StateId}");
        }
        currentFormProfile?.behaviour.EnterForm();

        currentFormIndex = profileIndex;
        CursorUtils.SetCursor(currentFormProfile.cursorSettings);

        debugVariables.form = currentFormProfile.formName;
        EventHandler<string>.InvokeEvent(GlobalEvents.PLAYER_FORM_CHANGED, currentFormProfile.id);

        Debug.Log("--------------------------------------");
    }

    private void OnDrawGizmos()
    {
        if (currentFormProfile == null) { return; }
        currentFormProfile.behaviour.OnDrawGizmos();

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down * 1.3f);
    }

    public void HandleFormCollision(CollisionData data)
    {
        if (currentFormProfile == null) { return; }
        rbController.lastRelativeVelocity = data.coll.relativeVelocity;
        currentFormProfile.behaviour.OnCollision(data);
    }
}
