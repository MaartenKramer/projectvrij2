using UnityEngine;

public class DebugUIManager : MonoBehaviour
{
    [SerializeField] private GameObject parent;

    private void OnEnable()
    {
        EventHandler.AddListener(GlobalEvents.UI_DEBUG_SHOW, OnTriggerDebugInfo);
    }

    private void OnDisable()
    {
        EventHandler.RemoveListener(GlobalEvents.UI_DEBUG_SHOW, OnTriggerDebugInfo);
    }

    private void OnTriggerDebugInfo()
    {
        parent.SetActive(!parent.activeSelf);
    }
}
