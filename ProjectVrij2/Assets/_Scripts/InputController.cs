using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class InputController
{
    public InputActionAsset actionAsset;

    public InputAction GetAction(string actionMapId, string actionId)
    {
        InputAction action = null;
        
        InputActionMap actionMap = actionAsset.FindActionMap(actionMapId);
        if (actionMap == null) { return null; }

        action = actionMap.FindAction(actionId);
        if(action == null) { return null; }

        return action;
    }

    public InputAction GetActionGlobal(string actionId)
    {
        InputAction action = null;

        action = actionAsset.FindAction(actionId);
        if(action == null) { return null; }

        return action;
    }
}
