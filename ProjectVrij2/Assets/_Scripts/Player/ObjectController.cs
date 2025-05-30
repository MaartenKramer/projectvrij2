using UnityEngine;

public class ObjectController : MonoBehaviour
{
    private bool isDisabled;
    public bool IsDisabled => isDisabled;

    public void Enable() { isDisabled = true; }
    public void Disable() { isDisabled = false;}
}
