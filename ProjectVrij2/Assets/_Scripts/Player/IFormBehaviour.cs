using UnityEngine;

public interface IFormBehaviour
{
    public FormProfileSO FormProfile { get; }

    public void Initialize(FormProfileSO profile);  // call this when before doing anything else with the interface

    public void EnterForm();
    public void UpdateForm();
    public void ExitForm();

    public void HandleInput();
    public void HandlePhysics();
    public void HandleAbilities();
}
