using UnityEngine;

public class IdleState : IState
{
    public bool IsUnique => false;

    private string stateId;
    public string StateId => stateId;

    public string StateTransitionId => null;

    public IdleState(string stateId)
    {
        this.stateId = stateId;
    }

    public void EnterState()
    {
        Debug.Log($"Entered Idle state");
    }

    public void UpdateState()
    {
    }

    public void ExitState()
    {
        Debug.Log($"Exited Idle state");
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

}
