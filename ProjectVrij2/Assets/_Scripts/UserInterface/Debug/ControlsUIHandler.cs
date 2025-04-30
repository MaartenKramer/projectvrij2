using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ControlsUIHandler : MonoBehaviour
{
    [SerializeField] private FormControls[] Controls;
    [SerializeField] private FormControls currentControls;
    [Space]
    [SerializeField] private FormControls globalControls;

    private TextMeshProUGUI textMesh;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        EventHandler<string>.AddListener(GlobalEvents.PLAYER_FORM_CHANGED, OnFormChange);
    }

    private void OnDisable()
    {
        EventHandler<string>.RemoveListener(GlobalEvents.PLAYER_FORM_CHANGED, OnFormChange);
    }

    private void OnFormChange(string formId)
    {
        Debug.Log($"[Debug UI] Form changed! {formId}");

        foreach (var control in Controls)
        {
            if (control.formId == formId) { currentControls = control; break; }
        }

        textMesh.text = GetString();
    }

    private string GetString()
    {
        StringBuilder sb = new StringBuilder();

        if(currentControls != null)
        {
            sb.AppendLine(currentControls.GetString());
        }
        sb.AppendLine(globalControls.GetString());

        string results = sb.ToString();
        sb = null;

        return results;
    }
}
