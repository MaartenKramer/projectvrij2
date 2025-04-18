using UnityEngine;

public interface IFormBehaviour
{
    public FormProfileSO FormProfile { get; }
    public StateMachine StateMachine { get; }

    public void Initialize(GameObject owner, FormProfileSO profile);  // inject data and setup state-machine 

    public void EnterForm();
    public void UpdateForm();
    public void ExitForm();

    public void HandleInput();
    public void HandlePhysics();
    public void HandleAbilities();
}
