using UnityEngine;
using System.Collections.Generic;
using System.Text;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "New ControlsText", menuName = "ScriptableObjects/Debug/ControlsText")]
public class ControlsText : ScriptableObject
{
    public List<KeyActionPair> actions;
    public string deviceName;

    public string GetString()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine(deviceName + ":");
        foreach (KeyActionPair pair in actions)
        {
            sb.AppendLine(pair.action + " - " + pair.key);
        }

        string result = sb.ToString();
        sb = null;

        return result;
    }
}
