using UnityEngine;
using UnityEngine.InputSystem;

public class IdleState : IState
{
    public bool IsUnique => true;

    public string StateId => "state_player_idle";

    public string StateTransitionId => "state_player_flight";

    private IFormBehaviour form;

    InputAction moveAction;
    InputAction jumpAction;

    private RigidbodyController rbController;

    public IdleState(IFormBehaviour form, string actionMapId)
    {
        this.form = form;

        //InputActionAsset input = form.Input;
        //actionMap = input.FindActionMap(form.in);
        //if(input == null || actionMap == null) { Debug.Log("No input action or actionmap found! Did you assign it in the form profile?"); }

        moveAction = form.InputController.GetAction(actionMapId, "Move");
        jumpAction = form.InputController.GetAction(actionMapId, "Jump");
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
        //Debug.Log($"[{owner.owner.name}] direction: {direction}");

        if (jumpAction.triggered) { Debug.Log($"[{form.StateMachine.owner.name}] jump input received"); }
    }

    public void HandlePhysics()
    {
        //Debug.Log($"[{owner.owner.name}] Fixed updating Idle physics");
    }

    public void OnDrawGizmos()
    {

    }
}
