using UnityEngine;
using UnityEngine.InputSystem;

public interface IFormBehaviour
{
    [Header("State")]
    public StateMachine StateMachine { get; }

    [Header("Data")]
    public FormProfileSO FormProfile { get; }

    [Header("References")]
    public InputController InputController { get; }
    public RigidbodyController RigidbodyController { get; }

    public void Initialize(GameObject owner, RigidbodyController rbController, InputController inputController, FormProfileSO profile);  // inject data and setup state-machine 

    public void EnterForm();
    public void UpdateForm();
    public void ExitForm();

    public void HandleInput();
    public void HandlePhysics();
    public void HandleAbilities();

    public void OnDrawGizmos();
}
