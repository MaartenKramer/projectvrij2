using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    public bool IsUnique { get; }
    public string StateId { get; }
    public string StateTransitionId { get; }

    public void EnterState();
    public void UpdateState();
    public void ExitState();

    public void HandleInput();
    public void HandleAbilities();
    public void HandlePhysics();

    public void OnDrawGizmos();
}
