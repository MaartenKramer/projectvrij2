using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    public GameObject owner;

    private string defaultStateId;
    public IState currentState;
    public Dictionary<string, IState> availableStates = new Dictionary<string, IState>();

    public StateMachine(GameObject owner, string defaultStateId) 
    {
        this.owner = owner;
        this.defaultStateId = defaultStateId;
    }

    public void SwitchState(string stateId)
    {
        if(!availableStates.ContainsKey(stateId)) 
        {
            Debug.Log($"No available state with id: {stateId} found! You might need to add it to the available states dictionary");
            return;
        }
        currentState.ExitState();
        currentState = availableStates[stateId];
        currentState.EnterState();
    }

    // sets state without notifying enter and exit
    // usefull when setting state when not in the right form
    public void SetState(string stateId)
    {
        Debug.Log($"Attempting to set state on {owner.name} state-machine");
        if (!availableStates.ContainsKey(stateId))
        {
            Debug.Log($"No available state with id: {stateId} found! defaulting to default state");
            if(!availableStates.ContainsKey(stateId) ) 
            { 
                Debug.LogError($"default state: {defaultStateId} can not be found in available states!"); 
            }
            currentState = availableStates[defaultStateId];
        }
        else
        {
            currentState = availableStates[stateId];
        }
    }
    public void SetState(IState state)
    {
        Debug.Log($"Attempting to set state on {owner.name} state-machine");
        if (!availableStates.ContainsValue(state))
        {
            Debug.Log($"No available state found! defaulting to default state");
            if (!availableStates.ContainsKey(defaultStateId))
            {
                Debug.LogError($"default state: {defaultStateId} can not be found in available states!");
            }
            currentState = availableStates[defaultStateId];
        }
        else
        {
            currentState = state;
        }
    }
}
