using UnityEngine;
using UnityEngine.InputSystem;
using Sequencing;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;

public class TransformHandler : MonoBehaviour
{
    [Header("Forms")]
    [SerializeField] private FormProfileSO[] availableForms;
    [SerializeField] private int startingFormIndex;
    [SerializeField] private int currentFormIndex = 0;

    [Header("Transformation")]
    [SerializeField] private float cooldown = 4f;

    private Player player;
    private FormController formController;
    private Sequencer transformSequence;

    private InputAction transformAction;
    private float timestamp;

    private void Awake()
    {
        // get necessary references
        player = GetComponent<Player>();
        formController = GetComponent<FormController>();
        transformSequence = GetComponent<Sequencer>();
    }

    private void Start()
    {
        // bind input
        transformAction = player.InputController.GetActionGlobal("Transform");

        SetForm(startingFormIndex);
    }

    private void Update()
    {
        // check for transform input
        if (transformAction.triggered)
        {
            if(Time.time < timestamp + cooldown && timestamp != 0) 
            {
                Debug.Log($"[TransformHandler] transform is on cooldown! {(Time.time - timestamp).ToString("0.00")} / {cooldown.ToString("0.00")}");
                return; 
            }

            transformSequence.StartSequence();
            timestamp = Time.time;
        }
    }

    // this should be called from an transform sequence event action
    public void CycleForms(bool forward)
    {
        int newIndex = currentFormIndex;
        if (forward) { newIndex++; }
        else { newIndex--; }

        if (newIndex < 0) { newIndex = availableForms.Length - 1; }
        else if (newIndex >= availableForms.Length) { newIndex = 0; }

        SetForm(newIndex);
    }

    private void SetForm(int index)
    {
        currentFormIndex = index;
        FormProfileSO newForm = availableForms[currentFormIndex];

        player.SetForm(newForm);
        if (!formController.SwitchForms(newForm.id))
        {
            Debug.Log("[TransformHandler] Switching forms failed!");
        }
    }
}
