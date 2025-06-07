using UnityEngine;
using UnityEngine.InputSystem;
using Sequencing;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using System.Linq;

public class TransformHandler : MonoBehaviour
{
    [Header("Transformation")]
    [SerializeField] private float cooldown = 4f;
    [Space]
    [SerializeField] private int sequencerChannel = 1;
    [Space]
    [SerializeField] private Player player;
    [SerializeField] private FormController formController;
    [SerializeField] private Sequencer transformSequence;

    private InputAction transformAction;
    private float timestamp;

    public void Awake()
    {
        // get necessary references
        player = GetComponent<Player>();
        formController = GetComponent<FormController>();

        // give form scripts acces to important variables from their respective profile
        foreach (var form in player.AvailableForms) { form.behaviour.Initialize(gameObject, player, player.RigidbodyController, player.InputController, form); }

        var sequencers = GetComponents<Sequencer>();
        transformSequence = sequencers.Single<Sequencer>(x => x.CompareChannel(sequencerChannel));

    }

    public void Start()
    {
        SetForm(player.StartingFormIndex);

        // bind input
        transformAction = player.InputController.GetActionGlobal("Transform");
    }

    private void Update()
    {
        if (player.IsDisabled) { return; }

        // check for transform input
        if (transformAction.triggered)
        {
            if(Time.time < timestamp + cooldown && timestamp != 0) 
            {
                Debug.Log($"[TransformHandler] transform is on cooldown! {(Time.time - timestamp).ToString("0.00")} / {cooldown.ToString("0.00")}");
                return; 
            }

            transformSequence.StartSequence();
            timestamp = Time.time;
        }
    }

    // this should be called from an transform sequence event action
    public void CycleForms(bool forward)
    {
        int newIndex = player.CurrentFormIndex;
        if (forward) { newIndex++; }
        else { newIndex--; }

        if (newIndex < 0) { newIndex = player.AvailableForms.Length - 1; }
        else if (newIndex >= player.AvailableForms.Length) { newIndex = 0; }

        SetForm(newIndex);
    }

    public void SwitchForm(FormProfileSO profile)
    {
        Debug.Log("--------------------------------------");

        // determine if this profile is available. If it is, determine it's index
        int desiredIndex = -1;
        for (int i = 0; i < player.AvailableForms.Length; i++)
        {
            if (player.AvailableForms[i] != profile) { continue; }
            desiredIndex = i;
            Debug.Log($"profile {profile.formName}/{profile.id} found at index {desiredIndex} of available forms!");
        }
        if (desiredIndex == -1)
        {
            Debug.Log($"No corresponding index for {profile.formName}/{profile.id} found! <br> Is it missing from the available forms?");
            return;
        }

        IState previousState = player.ActiveForm.behaviour.StateMachine.currentState;
        player.ActiveForm?.behaviour.ExitForm();
        player.SetForm(profile);
        if (previousState.IsUnique)
        {
            if (player.ActiveForm.behaviour.StateMachine.availableStates.ContainsValue(player.ActiveForm.behaviour.StateMachine.currentState))
            {
                // if new form also has the same unique state, set it to that
                player.ActiveForm.behaviour.StateMachine.SetState(previousState);
                Debug.Log($"Setting movement state to unique state: {previousState.StateId}");
            }
            else
            {
                // if new form does not have the same unique state, change it to it's specified transition state
                // (e.g. if you are flying as a bird and transform into something flightless, you transition to a falling state instead of the last known state)
                string newStateId = player.ActiveForm.behaviour.StateMachine.currentState.StateTransitionId;
                player.ActiveForm.behaviour.StateMachine.SetState(newStateId);
                Debug.Log($"Setting movement state to transition state: {newStateId}");
            }
        }
        else
        {
            // if previous state is not unique this form has it, so change it to that
            player.ActiveForm.behaviour.StateMachine.SetState(previousState);
            Debug.Log($"Setting movement state to general state: {previousState.StateId}");
        }
        player.ActiveForm?.behaviour.EnterForm();

        player.SetIndex(desiredIndex);
        CursorUtils.SetCursor(player.ActiveForm.cursorSettings);

        player.debugVariables.form = player.ActiveForm.formName;
        EventHandler<string>.InvokeEvent(GlobalEvents.PLAYER_FORM_CHANGED, player.ActiveForm.id);

        Debug.Log("--------------------------------------");
    }
    public void SwitchForm(int profileIndex)
    {
        Debug.Log("--------------------------------------");

        IState previousState = player.ActiveForm.behaviour.StateMachine.currentState;
        player.ActiveForm?.behaviour.ExitForm();
        player.SetForm(player.AvailableForms[profileIndex]);
        if (previousState.IsUnique)
        {
            if (player.ActiveForm.behaviour.StateMachine.availableStates.ContainsValue(player.ActiveForm.behaviour.StateMachine.currentState))
            {
                // if new form also has the same unique state, set it to that
                player.ActiveForm.behaviour.StateMachine.SetState(previousState.StateId);
                Debug.Log($"Setting movement state to unique state: {previousState.StateId}");
            }
            else
            {
                // if new form does not have the same unique state, change it to it's specified transition state
                // (e.g. if you are flying as a bird and transform into something flightless, you transition to a falling state instead of the last known state)
                string newStateId = player.ActiveForm.behaviour.StateMachine.currentState.StateTransitionId;
                player.ActiveForm.behaviour.StateMachine.SetState(newStateId);
                Debug.Log($"Setting movement state to transition state: {newStateId}");
            }
        }
        else
        {
            // if previous state is not unique this form has it, so change it to that
            player.ActiveForm.behaviour.StateMachine.SetState(previousState.StateId);
            Debug.Log($"Setting movement state to general state: {previousState.StateId}");
        }
        player.ActiveForm?.behaviour.EnterForm();

        player.SetIndex(profileIndex);
        CursorUtils.SetCursor(player.ActiveForm.cursorSettings);

        player.debugVariables.form = player.ActiveForm.formName;
        EventHandler<string>.InvokeEvent(GlobalEvents.PLAYER_FORM_CHANGED, player.ActiveForm.id);

        Debug.Log("--------------------------------------");
    }

    private void SetForm(int index)
    {
        FormProfileSO newForm = player.AvailableForms[index];
        player.SetForm(newForm);
        player.SetIndex(index);
        SwitchForm(index);
        if (!formController.SwitchForms(newForm.id))
        {
            Debug.Log("[TransformHandler] Switching forms failed!");
        }
    }
}
