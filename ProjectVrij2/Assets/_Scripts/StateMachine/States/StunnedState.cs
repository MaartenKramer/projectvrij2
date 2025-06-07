using UnityEngine;

[System.Serializable]
public struct StunData
{
    [Header("General")]
    public float duration;
    public AnimationCurve speedToDurationCurve;
    [Header("On impact")]
    public float minVelocity;
    public float speedSlashMultiplier;  // multiplier used to slash the total velocity at point of impact
    [Header("Bounce")]
    public float maxVelocity;
    public float bounceStrength;
    public AnimationCurve speedToBounceCurve;
}
public class StunnedState : IState
{
    public bool IsUnique => false;
    public string StateId => "state_shared_stunned";

    private string stateTransitionId;
    public string StateTransitionId => stateTransitionId;

    private IFormBehaviour form;
    private StunData data;

    private float timestamp = Mathf.Infinity;
    private float finalDuration;

    public StunnedState(IFormBehaviour form, StunData data, string transitionId)
    {
        this.form = form;
        this.data = data;
        stateTransitionId = transitionId;
    }

    public void EnterState()
    {
        timestamp = Time.time;
        float remapped = MyMathUtils.Remap01(form.RigidbodyController.lastRelativeVelocity.magnitude, data.minVelocity, data.maxVelocity);
        finalDuration = data.speedToDurationCurve.Evaluate(remapped) * data.duration;
        form.Toggleable.Disable();
    }
    public void ExitState()
    {
        timestamp = Mathf.Infinity;
    }
    public void HandleAbilities()
    {
    }
    public void UpdateState()
    {
        if(Time.time < timestamp + finalDuration) 
        {
            Debug.Log($"[Stunned] stunned for {(Time.time - timestamp).ToString("0.00")} / {finalDuration.ToString("0.00")}");
            return; 
        }
        form.StateMachine.SwitchState(stateTransitionId);
    }
    public void HandleInput()
    {
    }
    public void HandlePhysics()
    {
    }
    public void OnDrawGizmos()
    {
    }
}
