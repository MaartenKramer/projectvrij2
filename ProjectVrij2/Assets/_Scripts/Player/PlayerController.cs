using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private FormProfileSO currentFormProfile;

    [SerializeField] private List<FormProfileSO> availableForms = new List<FormProfileSO>();

    private void Awake()
    {
        // give form scripts acces to important variables from their respective profile
        foreach (var form in availableForms) { form.behaviour.Initialize(form); }
    }

    // handle form state machine

    void Start()
    {
        currentFormProfile = availableForms[0]; // first form in the list is considered the default
    }

    void Update()
    {
        
    }
}
