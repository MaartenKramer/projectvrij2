using UnityEngine;
using System.Collections.Generic;
using System.Text;

[CreateAssetMenu(fileName = "New FormControls", menuName = "ScriptableObjects/Debug/FormControls")]
public class FormControls : ScriptableObject
{
    public string formId;
    public List<ControlsText> deviceControls;

    public string GetString()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("<b>" + formId + ": <b>");
        foreach (ControlsText controls in deviceControls)
        {
            sb.AppendLine(controls.GetString());
        }

        string results = sb.ToString();
        sb = null;

        return results;
    }
}
