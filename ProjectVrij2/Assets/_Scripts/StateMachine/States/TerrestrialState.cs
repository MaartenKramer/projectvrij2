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
    private float currentGravity;
    private float currentGravityMultiplier = 1f;
    private Vector3 direction;
    private float jumpTimestamp;

    // ground check
    private bool isGrounded = true;
    private bool readyToJump = false;

    // TODO - slope check

    public TerrestrialState(IFormBehaviour form, string actionMapId, TerrestrialData data)
    {
        this.form = form;
        this.data = data;

        // initialisation
        moveAction = form.InputController.GetAction(actionMapId, "Move");
        sprintAction = form.InputController.GetAction(actionMapId, "Sprint");
        jumpAction = form.InputController.GetAction(actionMapId, "Jump");

        currentSpeed = data.walkSpeed;
        currentGravity = data.airGravity;
    }

    public void EnterState()
    {
        jumpAction.started += ctx => TryJump();
        jumpAction.canceled += ctx => ReleaseJump();

        sprintAction.started += ctx => SetSpeed(data.sprintSpeed, true);
        sprintAction.canceled += ctx => SetSpeed(data.walkSpeed, false);

        DetermineGravity();
    }

    public void ExitState()
    {
        jumpAction.started -= ctx => TryJump();
        jumpAction.canceled -= ctx => ReleaseJump();

        sprintAction.started -= ctx => SetSpeed(data.sprintSpeed, true);
        sprintAction.started -= ctx => SetSpeed(data.walkSpeed, false);
    }

    public void HandleAbilities()
    {
    }

    public void HandleInput()
    {
        Transform camTransform = Camera.main.transform;
        form.RigidbodyController.orientation.forward = (form.RigidbodyController.Position - new Vector3(camTransform.position.x, form.RigidbodyController.Position.y, camTransform.position.z)).normalized;

        Vector3 moveInput = moveAction.ReadValue<Vector2>();
        direction = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        
        direction = form.RigidbodyController.orientation.forward * direction.z + form.RigidbodyController.orientation.right * direction.x;

        if(readyToJump && isGrounded)
        {
            TryJump();
        }
    }

    public void HandlePhysics()
    {
        DetermineGravity();

        Vector3 gravity = Vector3.down * (currentGravity * currentGravityMultiplier);
        form.RigidbodyController.rigidbody.AddForce(gravity, ForceMode.Acceleration);
        //form.RigidbodyController.ApplyGravity(currentGravityMultiplier);

        if (direction == Vector3.zero) { return; }

        Vector3 force = direction.normalized * currentSpeed * 10f;
        form.RigidbodyController.rigidbody.AddForce(force, ForceMode.Force);

        if (direction != Vector3.zero) { form.RigidbodyController.RotateTowards(direction.normalized, data.rotationSpeed); }
    }

    public void UpdateState()
    {
        if(direction != Vector3.zero) 
        {
            form.RigidbodyController.Rotate(direction, data.rotationSpeed);
        }

        bool grounded = CheckIsGrounded();
        if(grounded != isGrounded) 
        { 
            isGrounded = grounded;
            form.StateMachine.owner.GetComponent<PlayerController>().debugVariables.isGrounded = isGrounded;
        }

        //Debug.Log($"[GroundCheck] {isGrounded}");

        if (isGrounded) { form.RigidbodyController.TweenDrag(data.groundedDrag, .3f);}
        else { form.RigidbodyController.TweenDrag(data.airDrag, .1f); }

        form.StateMachine.owner.GetComponent<PlayerController>().debugVariables.gravity = currentGravity;

    }

    private void DetermineGravity()
    {
        // apply gravity manually
        if (!isGrounded && readyToJump)
        {
            if (currentGravity != data.airGravity) { currentGravity = data.airGravity; }
        }
        else if (!isGrounded)
        {
            if (currentGravity != data.fallingGravity) { currentGravity = data.fallingGravity; }
        }
        else
        {
            if (currentGravity != data.groundedGravity) { currentGravity = data.groundedGravity; }
        }

        if (form.RigidbodyController.LinearVelocity.y < 0) { currentGravityMultiplier = 2f; }
        else { currentGravityMultiplier = 1f; }
    }

    private bool CheckIsGrounded()
    {
        //Vector3 boxOrigin = form.RigidbodyController.Position;
        //boxOrigin.y = -data.checkDistance;
        //bool check = Physics.BoxCast(boxOrigin, data.checkSize * .5f, Vector3.down, Quaternion.identity, .2f, data.groundMask);

        //bool check = Physics.Raycast(form.RigidbodyController.Position, Vector3.down, data.checkDistance, data.groundMask);
        bool check = Physics.BoxCast(form.RigidbodyController.Position, data.checkSize, Vector3.down, Quaternion.identity, data.checkDistance, data.groundMask);
        return check;
    }

    private void Jump()
    {
        Vector3 force = Vector3.up * data.jumpForce;
        form.RigidbodyController.rigidbody.AddForce(force, ForceMode.Impulse);

        jumpTimestamp = Time.time;
    }

    private void TryJump()
    {
        readyToJump = true;

        if (Time.time <= jumpTimestamp + data.jumpCooldown && jumpTimestamp != 0) { return; }
        if (!isGrounded) { return; }

        Jump();
    }

    private void ReleaseJump()
    {
        readyToJump = false;
    }

    private void SetSpeed(float value, bool state)
    {
        currentSpeed = value;
        form.StateMachine.owner.GetComponent<PlayerController>().debugVariables.speedingUp = state;
    }

    public void OnDrawGizmos()
    {
    }

}

[System.Serializable]
public struct TerrestrialData
{
    public float walkSpeed;
    public float sprintSpeed;

    public float jumpForce;

    public float groundedGravity;
    public float airGravity;
    public float fallingGravity;

    public float groundedDrag;
    public float airDrag;

    public float rotationSpeed;

    public LayerMask groundMask;
    public Vector3 checkSize;
    public float checkDistance;

    public float jumpCooldown;

}