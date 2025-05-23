using System.Runtime.InteropServices;
using System.Text;
using TMPro;
using UnityEngine;

public class DebugVariableUIHandler : MonoBehaviour
{
    private TextMeshProUGUI textMesh;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();    
    }

    private void OnEnable()
    {
        EventHandler<PlayerDebugVariables>.AddListener(GlobalEvents.UI_DEBUG_UPDATEVARIABLES, OnUpdateVariables);
    }

    private void OnDisable()
    {
        EventHandler<PlayerDebugVariables>.RemoveListener(GlobalEvents.UI_DEBUG_UPDATEVARIABLES, OnUpdateVariables);
    }

    private void OnUpdateVariables(PlayerDebugVariables variables)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("Form: " + variables.form);
        sb.AppendLine();
        sb.AppendLine("Velocity: " + variables.velocity.ToString("00.00"));
        sb.AppendLine("Drag: " + variables.drag.ToString("00.00"));
        sb.AppendLine("Lift: " + variables.lift.ToString("00.00"));
        sb.AppendLine();
        sb.AppendLine("Speeding up: " + variables.speedingUp.ToString());
        sb.AppendLine("Slowing down: " + variables.slowingDown.ToString());
        sb.AppendLine();
        sb.AppendLine("IsGrounded: " + variables.isGrounded.ToString());
        sb.AppendLine("OnSlope: " + variables.onSlope.ToString());
        sb.AppendLine("Gravity: " + variables.gravity.ToString("00.00"));

        string result = sb.ToString();
        sb = null;

        textMesh.text = result;
    }
}
