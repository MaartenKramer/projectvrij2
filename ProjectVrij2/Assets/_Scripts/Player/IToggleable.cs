using UnityEngine;

// it was this or Enableable, take your pick >:(
public class Toggleable : MonoBehaviour 
{
    [Header("Toggleable")]
    [SerializeField] private bool isDisabled;
    public bool IsDisabled => isDisabled;
    public void Enable() { isDisabled = true; }
    public void Disable() { isDisabled = false; }
}
