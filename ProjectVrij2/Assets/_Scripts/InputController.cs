using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

[System.Serializable]
public class InputController
{
    public InputDevice InputDevice { get; private set; }

    public void OnEnable()
    {
    }

    public void OnDisable()
    {
    }

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

        action = actionAsset.FindActionMap("Actions_Global").FindAction(actionId);
        if(action == null) { return null; }

        return action;
    }

    public InputAction GetActionDebug(string actionId)
    {
        InputAction action = null;

        action = actionAsset.FindActionMap("Actions_Debug").FindAction(actionId);
        if (action == null) { return null; }

        return action;
    }
}
