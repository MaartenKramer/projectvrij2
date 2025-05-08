using Unity.VisualScripting;
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
    private Vector3 direction;

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
        Transform camTransform = Camera.main.transform;
        Debug.Log($"[Cam forward]: {camTransform.forward}");
        form.RigidbodyController.orientation.forward = (form.RigidbodyController.Position - new Vector3(camTransform.position.x, form.RigidbodyController.Position.y, camTransform.position.z)).normalized;

        Vector3 moveInput = moveAction.ReadValue<Vector2>();
        direction = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        Debug.Log($"[Cam forward] moveInput {moveInput}");
        
        direction = form.RigidbodyController.orientation.forward * direction.z + form.RigidbodyController.orientation.right * direction.x;
        if (direction != Vector3.zero) { form.RigidbodyController.RotateTowards(direction.normalized, data.rotationSpeed); }
    }

    public void HandlePhysics()
    {
        if(direction == Vector3.zero) { return; }

        Vector3 force = direction.normalized * currentSpeed * 10f;
        form.RigidbodyController.rigidbody.AddForce(force, ForceMode.Force);

    }

    public void UpdateState()
    {

        if(direction != Vector3.zero) 
        {
            form.RigidbodyController.Rotate(direction, data.rotationSpeed);
        }
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

    public float rotationSpeed;
}