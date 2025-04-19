using UnityEngine;
using UnityEngine.InputSystem;

public class IdleState : IState
{
    public bool IsUnique => false;

    private string stateId;
    public string StateId => stateId;
    public string StateTransitionId => null;

    private StateMachine owner;
    private IFormBehaviour form;

    private InputActionMap actionMap;
    InputAction moveAction;
    InputAction jumpAction;

    public IdleState(StateMachine owner, IFormBehaviour form, string stateId)
    {
        this.stateId = stateId;
        this.owner = owner;
        this.form = form;

        InputActionAsset input = form.FormProfile.actionAsset;
        actionMap = input.FindActionMap(form.FormProfile.actionMapId);
        if(input == null || actionMap == null) { Debug.Log("No input action or actionmap found! Did you assign it in the form profile?"); }

        moveAction = actionMap.FindAction("Move");
        jumpAction = actionMap.FindAction("Jump");
    }

    public void EnterState()
    {
        // Debug.Log($"[{owner.owner.name}] Entered Idle state");
    }

    public void UpdateState()
    {
        //Debug.Log($"[{owner.owner.name}] Updating Idle state");
    }

    public void ExitState()
    {
        //Debug.Log($"[{owner.owner.name}] Exited Idle state");
    }

    public void HandleAbilities()
    {
    }

    public void HandleInput()
    {
        Vector2 direction = moveAction.ReadValue<Vector2>();
        Debug.Log($"[{owner.owner.name}] direction: {direction}");

        if (jumpAction.triggered) { Debug.Log($"[{owner.owner.name}] jump input received"); }
    }

    public void HandlePhysics()
    {
        //Debug.Log($"[{owner.owner.name}] Fixed updating Idle physics");
    }

}
