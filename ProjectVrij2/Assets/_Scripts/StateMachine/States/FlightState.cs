using UnityEngine;
using UnityEngine.InputSystem;

public class FlightState : IState
{
    public bool IsUnique => false;

    public string StateId => "state_player_flight";

    public string StateTransitionId => "state_player_moving";   // equivalent state in human form

    private IFormBehaviour form;
    private BirdData data;

    InputAction moveAction;
    InputAction lookAction;
    InputAction speedUpAction;
    InputAction slowDownAction;
    InputAction boostAction;

    private Vector2 direction;
    private Vector2 lookDirection;

    public FlightState(IFormBehaviour form, string actionMapId, BirdData data) 
    {
        this.form = form;
        this.data = data;

        moveAction = form.InputController.GetAction(actionMapId, "Move");
        lookAction = form.InputController.GetAction(actionMapId, "Look");
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
        direction = moveAction.ReadValue<Vector2>();
        direction = new Vector3(direction.y, direction.x, 1);
        lookDirection = lookAction.ReadValue<Vector2>();
    }

    public void HandlePhysics()
    {
        // apply force forward
        form.RigidbodyController.rigidbody.AddForce(form.RigidbodyController.Forward * data.flightSpeed);

        // apply force left/right
        form.RigidbodyController.rigidbody.AddForce((form.RigidbodyController.Right * direction.x) * data.turnSpeed);

        // apply force up/down
        form.RigidbodyController.rigidbody.AddForce((Vector3.up * direction.y) * data.turnSpeed);

        form.RigidbodyController.Rotate(form.RigidbodyController.rigidbody.GetAccumulatedForce().normalized, data.turnSpeed * 10);

        // apply gravity
        float gravity = data.gravity;
        if(direction == Vector2.zero) { gravity = 0; }
        else { gravity = data.gravity; }
        form.RigidbodyController.rigidbody.AddForce(-Vector3.up * gravity, ForceMode.Acceleration);
    }

    public void UpdateState()
    {
    }
}

//// constant forwards force/thrust
//form.RigidbodyController.rigidbody.AddForce(form.RigidbodyController.Forward * data.flightSpeed, ForceMode.Force);

//// rotation follow lookdelta
//form.RigidbodyController.Rotate(direction, data.turnSpeed);

//Debug.Log($"Velocity: {form.RigidbodyController.rigidbody.linearVelocity.magnitude}");

//// Rotate the bird based on input
//form.RigidbodyController.Rotate(direction, data.turnSpeed);

//// Apply forward thrust automatically (bird always moves forward)
//Vector3 forwardForce = form.RigidbodyController.Forward * data.flightSpeed;
//form.RigidbodyController.rigidbody.AddForce(forwardForce, ForceMode.Force);

//// Apply lift based on how "nose-up" or "nose-down" the bird is
//float lift = Mathf.Clamp(Vector3.Dot(form.RigidbodyController.Up, Vector3.up), 0f, 1f) * data.flightSpeed;
//form.RigidbodyController.rigidbody.AddForce(Vector3.up * lift, ForceMode.Force);

//// Apply gravity manually (you can tweak gravityScale for a more floaty feeling)
//form.RigidbodyController.rigidbody.AddForce(Vector3.down * data.gravity, ForceMode.Acceleration);

// Constant forward movement (by setting velocity to a constant speed along the forward axis)
//form.RigidbodyController.rigidbody.AddForce(form.RigidbodyController.Forward * data.flightSpeed, ForceMode.Force);

//// Vertical movement (up and down using pitch rotation)
//if (Input.GetKey(KeyCode.W)) // Fly down
//    form.RigidbodyController.Rotate(-Vector3.up, data.turnSpeed);  // Rotate downwards (pitch down)
//if (Input.GetKey(KeyCode.S)) // Fly up
//    form.RigidbodyController.Rotate(Vector3.up, data.turnSpeed); // Rotate upwards (pitch up)

//// Horizontal movement (left/right using bank rotation)
//if (Input.GetKey(KeyCode.A)) // Bank left
//    form.RigidbodyController.Rotate(-form.RigidbodyController.Right, data.turnSpeed); // Rotate left (yaw left)
//if (Input.GetKey(KeyCode.D)) // Bank right
//    form.RigidbodyController.Rotate(form.RigidbodyController.Right, data.turnSpeed); // Rotate right (yaw right)
