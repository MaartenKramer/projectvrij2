using UnityEngine;
using UnityEngine.InputSystem;

public class FlightState : IState
{
    public bool IsUnique => false;

    public string StateId => "state_player_flight";

    public string StateTransitionId => "state_player_moving";   // equivalent state in human form

    private IFormBehaviour form;

    InputAction moveAction;
    InputAction speedUpAction;
    InputAction slowDownAction;
    InputAction boostAction;

    public FlightState(IFormBehaviour form, string actionMapId) 
    {
        this.form = form;

        moveAction = form.InputController.GetAction(actionMapId, "Move");
    }

    public void EnterState()
    {
    }

    public void ExitState()
    {
    }

    public void HandleAbilities()
    {
    }

    public void HandleInput()
    {
    }

    public void HandlePhysics()
    {
    }

    public void UpdateState()
    {
    }
}
