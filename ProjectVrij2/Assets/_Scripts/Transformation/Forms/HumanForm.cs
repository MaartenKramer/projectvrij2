using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HumanForm : IFormBehaviour
{
    private FormProfileSO formProfile;
    public FormProfileSO FormProfile => formProfile;

    private StateMachine stateMachine;
    public StateMachine StateMachine => stateMachine;

    private GameObject owner;
    private string defaultStateId = "state_player_idle";

    public void Initialize(GameObject owner, FormProfileSO profile)
    {
        // inject data
        formProfile = profile;
        this.owner = owner;

        // setup state-machine
        stateMachine = new StateMachine(owner, defaultStateId);
        stateMachine.availableStates.Add("state_player_idle", new IdleState("state_player_idle"));

        stateMachine.SetState(defaultStateId);
    }

    public void EnterForm()
    {
        Debug.Log("Entered human form");
    }

    public void UpdateForm()
    {
        
    }

    public void ExitForm()
    {
        Debug.Log("Exited human form");
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
