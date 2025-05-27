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

    private PlayerController playerController;

    //input
    InputAction moveAction;
    InputAction sprintAction;
    InputAction jumpAction;

    // debug
    private float currentSpeed;
    private float desiredGravity;
    private float currentGravity;
    private float currentGravityMultiplier = 1f;
    private float currentGravityTweenSpeed;
    private Vector3 direction;
    private float jumpTimestamp;

    // ground check
    private bool isGrounded = true;
    private bool readyToJump = false;

    // TODO - slope check
    private RaycastHit slopeHit;
    

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

        playerController = form.StateMachine.owner.GetComponent<PlayerController>();
    }

    public void EnterState()
    {
        jumpAction.started += ctx => TryJump();
        jumpAction.canceled += ctx => ReleaseJump();

        sprintAction.started += ctx => SetSpeed(data.sprintSpeed, true);
        sprintAction.canceled += ctx => SetSpeed(data.walkSpeed, false);

        SetDesiredGravity(data.fallingGravity, 1f);
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

        Vector3 force;
        if (OnSlope())
        {
            force = GetSlopeDirection() * currentSpeed * 10f;

            playerController.debugVariables.onSlope = true;
        }
        else
        {
            force = direction.normalized * currentSpeed * 10f;
            playerController.debugVariables.onSlope = false;
        }
        
        if (direction == Vector3.zero) {
            return; 
        }

        form.RigidbodyController.rigidbody.AddForce(force, ForceMode.Force);

        // opposing force when turning quickly (stops sliding when turning)
        float directionDot = Vector3.Dot(direction, form.RigidbodyController.LinearVelocity.normalized);
        if(directionDot < 0) 
        {
            Vector3 brakingForce = (direction - form.RigidbodyController.LinearVelocity.normalized) * (data.brakingForce * 10f);
            form.RigidbodyController.rigidbody.AddForce(brakingForce, ForceMode.Acceleration);
        }

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
            playerController.debugVariables.isGrounded = isGrounded;
        }

        //Debug.Log($"[GroundCheck] {isGrounded}");
        if(direction == Vector3.zero && isGrounded) { form.RigidbodyController.TweenDrag(data.idleDrag, 3f); }
        else if (isGrounded) { form.RigidbodyController.TweenDrag(data.groundedDrag, 3f); }
        else { form.RigidbodyController.TweenDrag(data.airDrag, 3f); }

        playerController.GetComponent<PlayerController>().debugVariables.gravity = currentGravity;

    }

    private void DetermineGravity()
    {
        // apply gravity manually
        if (!isGrounded && readyToJump)
        {
            if (currentGravity != data.airGravity) { SetDesiredGravity(data.airGravity, 5f); }
        }
        else if (!isGrounded)
        {
            if (currentGravity != data.fallingGravity) { SetDesiredGravity(data.fallingGravity, 5f); }
        }
        else
        {
            if (currentGravity != data.groundedGravity) { SetDesiredGravity(data.groundedGravity, 5f); }
        }

        if (currentGravity > desiredGravity - .05f && currentGravity < desiredGravity + .05f)
        {
            currentGravity = desiredGravity;
        }
        else
        {
            currentGravity = Mathf.Lerp(currentGravity, desiredGravity, currentGravityTweenSpeed * Time.deltaTime);
        }

        if (form.RigidbodyController.LinearVelocity.y < 0) { currentGravityMultiplier = 2f; }
        else { currentGravityMultiplier = 1f; }
    }

    private void SetDesiredGravity(float target, float tweenSpeed)
    {
        desiredGravity = target;
        currentGravityTweenSpeed = tweenSpeed;
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

    private bool OnSlope()
    {
        if(Physics.Raycast(form.RigidbodyController.Position, Vector3.down, out slopeHit, data.checkDistance * 2f, data.groundMask))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            //Debug.Log($"[Raycast] Hit! {slopeHit.collider.gameObject.name} - angle: {angle}");
            return angle < data.maxSlopeAngle && angle != 0;
        }


        return false;
    }

    private Vector3 GetSlopeDirection()
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    private void TryJump()
    {

        if (isGrounded) { readyToJump = true;  }
        else { return; }

        if (Time.time <= jumpTimestamp + data.jumpCooldown && jumpTimestamp != 0) { return; }

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
    [Header("speed vars")]
    public float walkSpeed;
    public float sprintSpeed;

    [Header("jump vars")]
    public float jumpForce;
    public float jumpCooldown;

    [Header("slope vars")]
    public float maxSlopeAngle;

    [Header("gravity vars")]
    public float groundedGravity;
    public float airGravity;
    public float fallingGravity;

    [Header("drag vars")]
    public float idleDrag;
    public float groundedDrag;
    public float airDrag;

    [Header("rotational vars")]
    public float rotationSpeed;

    [Header("Turning vars")]
    public float brakingForce;

    [Header("ground check")]
    public LayerMask groundMask;
    public Vector3 checkSize;
    public float checkDistance;

}