using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private FormProfileSO currentFormProfile;

    [SerializeField] private List<FormProfileSO> availableForms = new List<FormProfileSO>();
    [SerializeField] private int currentFormIndex = 0;

    [Header("Debugging")]
    [SerializeField] private FormProfileSO debugProfile;

    private void Awake()
    {
        // give form scripts acces to important variables from their respective profile
        foreach (var form in availableForms) { form.behaviour.Initialize(form); }
    }

    void Start()
    {
        currentFormIndex = 0;
        currentFormProfile = availableForms[currentFormIndex]; // first form in the list is considered the default
    }

    void Update()
    {
        // debug input | TODO - Change input to new input system and decouple it
        if (Input.GetKeyDown(KeyCode.Q)) { CycleForms(false); }
        if (Input.GetKeyDown(KeyCode.E)) { CycleForms(true); }

        if (Input.GetKeyDown(KeyCode.F) && debugProfile != null) { SwitchForm(debugProfile); }

        currentFormProfile?.behaviour.HandleInput();
        currentFormProfile?.behaviour.HandleAbilities();
        currentFormProfile?.behaviour.UpdateForm();
    }

    private void FixedUpdate()
    {
        currentFormProfile?.behaviour.HandlePhysics();
    }

    public void CycleForms(bool forward)
    {
        int newIndex = currentFormIndex;
        if (forward) { newIndex++; }
        else { newIndex--; }

        if(newIndex < 0) { newIndex = availableForms.Count - 1; }
        else if(newIndex >= availableForms.Count) { newIndex = 0; }

        currentFormIndex = newIndex;
        SwitchForm(currentFormIndex);
    }

    public void SwitchForm(FormProfileSO profile)
    {
        // determine if this profile is available. If it is, determine it's index
        int desiredIndex = -1;
        for(int i = 0; i < availableForms.Count; i++)
        {
            if (availableForms[i] != profile) { continue; }
            desiredIndex = i;
            Debug.Log($"profile {profile.formName}/{profile.id} found at index {desiredIndex} of available forms!");
        }
        if(desiredIndex == -1) 
        { 
            Debug.Log($"No corresponding index for {profile.formName}/{profile.id} found! <br> Is it missing from the available forms?");
            return;
        }

        currentFormProfile?.behaviour.ExitForm();
        currentFormProfile = profile;
        currentFormProfile?.behaviour.EnterForm();

        currentFormIndex = desiredIndex;
    }
    public void SwitchForm(int profileIndex)
    {
        currentFormProfile?.behaviour.ExitForm();
        currentFormProfile = availableForms[profileIndex];
        currentFormProfile?.behaviour.EnterForm();
    }
}
