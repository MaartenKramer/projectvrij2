using UnityEngine;

[System.Serializable]
public class HumanForm : IFormBehaviour
{
    private FormProfileSO formProfile;
    public FormProfileSO FormProfile => formProfile;

    public void Initialize(FormProfileSO profile)
    {
        formProfile = profile;
    }

    // form state machine

    public void EnterForm()
    {
        Debug.Log("Entered human form");
    }

    public void UpdateForm()
    {
        
    }

    public void ExitForm()
    {
        Debug.Log("Exited human form");
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
