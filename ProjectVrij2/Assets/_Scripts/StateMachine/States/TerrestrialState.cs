using UnityEngine;
using UnityEngine.InputSystem;

public class TerrestrialState : IState
{
    public bool IsUnique => false;

    public string StateId => "state_player_terrestrial";

    public string StateTransitionId => "state_player_terrestrial";

    private IFormBehaviour form;
    private TerrestrialData data;

    //input
    InputAction moveAction;
    InputAction sprintAction;
    InputAction jumpAction;

    // debug
    private float currentSpeed;

    public TerrestrialState(IFormBehaviour form, string actionMapId, TerrestrialData data)
    {
        this.form = form;
        this.data = data;

        // initialisation
        moveAction = form.InputController.GetAction(actionMapId, "Move");
        sprintAction = form.InputController.GetAction(actionMapId, "Sprint");
        jumpAction = form.InputController.GetAction(actionMapId, "Jump");

        currentSpeed = data.walkSpeed;
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

[System.Serializable]
public struct TerrestrialData
{
    public float walkSpeed;
    public float sprintSpeed;

    public float jumpForce;

    public float groundedDrag;
    public float airDrag;
}