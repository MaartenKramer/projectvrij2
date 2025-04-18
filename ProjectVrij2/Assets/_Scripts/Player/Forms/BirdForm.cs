using UnityEngine;

public class BirdForm : IFormBehaviour
{
    private FormProfileSO formProfile;
    public FormProfileSO FormProfile => formProfile;

    public void Initialize(FormProfileSO profile)
    {
        // inject data
        formProfile = profile;

        // setup state-machine
    }

    public void EnterForm()
    {
        Debug.Log("Entered bird form");
    }

    public void UpdateForm()
    {

    }

    public void ExitForm()
    {
        Debug.Log("Exited bird form");
    }

    public void HandleAbilities()
    {
    }

    public void HandleInput()
    {
    }

    public void HandlePhysics()
    {
    }
}
