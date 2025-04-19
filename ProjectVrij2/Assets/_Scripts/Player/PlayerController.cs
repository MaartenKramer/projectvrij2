using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private FormProfileSO currentFormProfile;

    [SerializeField] private List<FormProfileSO> availableForms = new List<FormProfileSO>();
    [SerializeField] private int currentFormIndex = 0;

    [Header("Input")]
    [SerializeField] private InputActionAsset globalInput;
    private InputAction transformAction;

    [Header("Debugging")]
    [SerializeField] private FormProfileSO debugProfile;

    private void Awake()
    {
        // give form scripts acces to important variables from their respective profile
        foreach (var form in availableForms) { form.behaviour.Initialize(gameObject, form); }

        InputActionMap globalActionMap = globalInput.FindActionMap("Actions_Global");
        transformAction = globalActionMap.FindAction("Transform");
    }

    private void OnDisable()
    {
        
    }

    void Start()
    {
        currentFormIndex = 0;
        currentFormProfile = availableForms[currentFormIndex]; // first form in the list is considered the default
    }

    void Update()
    {
        // debug input | TODO - Change input to new input system and decouple it
        if (transformAction.triggered) 
        {
            Debug.Log($"transform action; got key down");
            if (transformAction.ReadValue<float>() < 0) { CycleForms(false); Debug.Log($"transform action triggered! value: {transformAction.ReadValue<float>()}"); }
            else if (transformAction.ReadValue<float>() > 0) { CycleForms(true); Debug.Log($"transform action triggered! value: {transformAction.ReadValue<float>()}"); }     
        }

        //if (Input.GetKeyDown(KeyCode.Q)) { CycleForms(false); }
        //if (Input.GetKeyDown(KeyCode.E)) { CycleForms(true); }

        //if (Input.GetKeyDown(KeyCode.F) && debugProfile != null) { SwitchForm(debugProfile); }

        currentFormProfile?.behaviour.HandleInput();
        currentFormProfile?.behaviour.HandleAbilities();
        currentFormProfile?.behaviour.UpdateForm();
    }

    private void FixedUpdate()
    {
        currentFormProfile?.behaviour.HandlePhysics();
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

        Debug.Log("--------------------------------------");
    }
}
