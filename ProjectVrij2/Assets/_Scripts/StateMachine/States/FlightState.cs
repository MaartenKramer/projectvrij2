using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlightState : IState
{
    public bool IsUnique => false;

    public string StateId => "state_player_flight";

    public string StateTransitionId => "state_player_terrestrial";   // equivalent state in human form

    private IFormBehaviour form;
    private FlightData data;

    // inputs
    InputAction moveAction;
    InputAction lookAction;
    InputAction speedUpAction;
    InputAction slowDownAction;
    InputAction boostAction;
    InputAction rollLeftAction;
    InputAction rollRightAction;

    private Vector2 direction;
    private Vector2 lookDirection;

    private float currentSpeed;
    private float previousDrag;

    private float boostTimestamp = 0f;
    private float rollTimestamp = 0f;

    //states
    private bool isSlowedDown = false;

    public FlightState(IFormBehaviour form, string actionMapId, FlightData data) 
    {
        this.form = form;
        this.data = data;

        // initialisation
        moveAction = form.InputController.GetAction(actionMapId, "Move");
        lookAction = form.InputController.GetAction(actionMapId, "Look");
        speedUpAction = form.InputController.GetAction(actionMapId, "SpeedUp");
        slowDownAction = form.InputController.GetAction(actionMapId, "SlowDown");
        boostAction = form.InputController.GetAction(actionMapId, "Boost");
        rollLeftAction = form.InputController.GetAction(actionMapId, "Roll_Left");
        rollRightAction = form.InputController.GetAction(actionMapId, "Roll_Right");

        currentSpeed = data.flightSpeed;
    }

    public void EnterState()
    {
        speedUpAction.started += ctx => SpeedUp(data.quickFlightSpeed, true);
        speedUpAction.canceled += ctx => SpeedUp(data.flightSpeed, false);

        slowDownAction.started += ctx => SlowDown(true);
        slowDownAction.canceled += ctx => SlowDown(false);
    }

    public void ExitState()
    {
        speedUpAction.started -= ctx => SpeedUp(data.quickFlightSpeed, true);
        speedUpAction.canceled -= ctx => SpeedUp(data.flightSpeed, false);

        slowDownAction.started -= ctx => SlowDown(true);
        slowDownAction.canceled -= ctx => SlowDown(false);
    }

    public void HandleAbilities()
    {
    }

    public void HandleInput()
    {
        direction = moveAction.ReadValue<Vector2>();
        direction = new Vector3(direction.x, direction.y, 0);
        lookDirection = lookAction.ReadValue<Vector2>();
        direction = direction + (lookDirection * data.mouseSensitivity);

        if (rollLeftAction.triggered)
        {
            Roll(true);
        }
        else if (rollRightAction.triggered)
        {
            Roll(false);
        }
        else if (boostAction.triggered)
        {
            Boost();
        }
    }

    public void HandlePhysics()
    {
        // apply force forward
        form.RigidbodyController.rigidbody.AddRelativeForce(Vector3.forward * currentSpeed);

        // rotate towards direction
        form.RigidbodyController.Rotate(direction, data.turnSpeed);

        // Stabilize the Z-axis rotation
        if (form.RigidbodyController.rigidbody.linearVelocity.magnitude > 0.1f)
        {
            // Get current rotation of the object
            Vector3 currentRotation = form.RigidbodyController.Rotation.eulerAngles;

            // Reset only the Z-axis rotation (keep X and Y as they are)
            currentRotation.z = 0;

            // Apply the corrected rotation
            form.RigidbodyController.Rotation = Quaternion.Euler(currentRotation);
        }

        float dot = Vector3.Dot(form.RigidbodyController.Forward.normalized, Vector3.up);

        // if player is facing downwards apply gravity
        if (!isSlowedDown)
        {
            if (dot < 0)
            {
                float desiredDrag = data.minDrag + (data.maxDrag - data.diveCurve.Evaluate(Mathf.Abs(dot)) * data.maxDrag);
                if(desiredDrag > previousDrag) {  /* Debug.Log($"[Drag] diving -> drag recovering"); */ form.RigidbodyController.TweenDrag(desiredDrag, data.dragRecoveryRate); }
                else { /* Debug.Log($"[Drag] diving -> drag reducing"); */ form.RigidbodyController.TweenDrag(desiredDrag, data.dragReductionRate); }
            
                previousDrag = desiredDrag;
                //form.RigidbodyController.SetDrag(data.drag - data.drag * Mathf.Abs(Mathf.Clamp(dot, 0, -1)));
                //form.RigidbodyController.rigidbody.AddForce((-Vector3.up * data.gravity) * Mathf.Abs(dot), ForceMode.Acceleration);
            }
            else 
            {
                if(form.RigidbodyController.LinearDrag != data.maxDrag)
                {
                    /* Debug.Log($"[Drag] level -> drag recovering"); */
                    form.RigidbodyController.TweenDrag(data.maxDrag, data.dragRecoveryRate);
                }
            }
        }
        else
        {
            form.RigidbodyController.TweenDrag(data.slowDownDrag, data.dragReductionRate);
        }

        // calculate and apply lift
        bool isRising = dot > data.risingDotThreshold;
        float forwardSpeed = Vector3.Dot(form.RigidbodyController.LinearVelocity, form.RigidbodyController.Forward);
        if(isRising && forwardSpeed >= data.minLiftSpeed)
        {
            float liftStrength = data.liftCoefficient * Mathf.Pow(forwardSpeed, 2);
            liftStrength = Mathf.Min(liftStrength, data.maxLift);
            liftStrength *= data.liftToAngleCurve.Evaluate(Mathf.Abs(dot));

            float remappedDrag = MyMathUtils.Remap(form.RigidbodyController.LinearDrag, data.minDrag, data.maxDrag, 0, 1);
            liftStrength *= data.liftToDragCurve.Evaluate(1 - Mathf.Abs(remappedDrag));

            Vector3 liftForce = liftStrength * Vector3.up;

            //form.RigidbodyController.rigidbody.AddRelativeForce(liftForce, ForceMode.Force);
            form.RigidbodyController.rigidbody.AddForce(liftForce, ForceMode.Force);

            form.StateMachine.owner.GetComponent<PlayerController>().debugVariables.lift = liftStrength;
        }

        // if minLiftSpeed = 50, lift = 0.01 * 50^2 = 

        //else { form.RigidbodyController.SetDrag(data.drag); }

        /* Debug.Log($"Rigidbody velocity: {form.RigidbodyController.LinearVelocity.magnitude}"); */

        // apply force left/right
        //form.RigidbodyController.rigidbody.AddForce((form.RigidbodyController.Right * direction.x) * data.turnSpeed);

        // apply force up/down
        //form.RigidbodyController.rigidbody.AddForce((Vector3.up * direction.y) * data.turnSpeed);

        //// apply gravity
        //float gravity = data.gravity;
        //if(direction == Vector2.zero) { gravity = 0; }
        //else { gravity = data.gravity; }
        //form.RigidbodyController.rigidbody.AddForce(-Vector3.up * gravity, ForceMode.Acceleration);
    }

    public void UpdateState()
    {
    }

    private void SpeedUp(float value, bool state)
    {
        SetSpeed(value);
        form.StateMachine.owner.GetComponent<PlayerController>().debugVariables.speedingUp = state;
    }

    private void SetSpeed(float value)
    {
        if(value == currentSpeed) { return; }
        currentSpeed = value;
    }

    private void SlowDown(bool state)
    {
        isSlowedDown = state;
        form.StateMachine.owner.GetComponent<PlayerController>().debugVariables.slowingDown = state;
        Debug.Log($"Slowed down: {state}");
    }

    private void Boost()
    {
        Debug.Log($"[Boost] timestamp: {boostTimestamp}");
        if (Time.time < boostTimestamp + data.boostCooldown && boostTimestamp != 0f) { return; }

        Debug.Log($"[Boost] boosted");
        form.RigidbodyController.rigidbody.AddRelativeForce(Vector3.forward * data.boostForce, ForceMode.Impulse);

        boostTimestamp = Time.time;
    }

    private void Roll(bool leftOrRight)
    {
        Debug.Log($"[Roll] timestamp: {rollTimestamp}");
        if (Time.time < rollTimestamp + data.rollCooldown && rollTimestamp != 0f) { return; }

        switch(leftOrRight)
        {
            case true:
                Debug.Log($"[Roll] left");
                form.RigidbodyController.rigidbody.AddRelativeForce(-Vector3.right * data.rollForce, ForceMode.Impulse);
                break;
            case false:
                Debug.Log($"[Roll] right");
                form.RigidbodyController.rigidbody.AddRelativeForce(Vector3.right * data.rollForce, ForceMode.Impulse);
                break;
        }

        rollTimestamp = Time.time;
    }

    public void OnDrawGizmos()
    {

    }
}

[System.Serializable]
public struct FlightData
{
    [Header("Speed vars")]
    public float flightSpeed;
    public float quickFlightSpeed;
    public float turnSpeed;

    [Header("Drag vars")]
    public float maxDrag;
    public float minDrag;
    public float dragRecoveryRate;
    public float dragReductionRate;
    public AnimationCurve diveCurve;
    public float slowDownDrag;

    [Header("Lift vars")]
    public float liftCoefficient;
    public AnimationCurve liftToAngleCurve;
    public AnimationCurve liftToDragCurve;
    public float maxLift;
    public float minLiftSpeed;
    [Space]
    public float risingDotThreshold;

    [Header("Ability vars")]
    public float boostForce;
    public float boostCooldown;
    public float rollForce;
    public float rollCooldown;

    [Header("Control vars")]
    [Range(0.01f,1f)] public float mouseSensitivity;
}
