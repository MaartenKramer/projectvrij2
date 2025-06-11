using Sequencing;
using System.Linq;
using UnityEngine;

public class Player : Toggleable
{
    [Header("Form data & references")]
    [SerializeField] private FormProfileSO activeForm;
    public FormProfileSO ActiveForm => activeForm;
    [SerializeField] private FormProfileSO[] availableForms;
    public FormProfileSO[] AvailableForms => availableForms;

    [Space]
    [SerializeField] private int startingFormIndex;
    public int StartingFormIndex => startingFormIndex;

    [SerializeField] private int currentFormIndex;
    public int CurrentFormIndex => currentFormIndex;
    [Space]
    [SerializeField] private Animator activeAnimator;
    public Animator ActiveAnimator => activeAnimator;

    [SerializeField] private Animator[] availableAnimators; // this should be in the same order as available forms (this way currentFormIndex can be used for both)
    public Animator[] AvailableAnimators => availableAnimators;

    [Header("Input & Physics")]
    [SerializeField] private InputController inputController;
    public InputController InputController => inputController;

    [SerializeField] private RigidbodyController rigidbodyController;
    public RigidbodyController RigidbodyController => rigidbodyController;


    public PlayerDebugVariables debugVariables;

    //[Space]
    //[SerializeField] private int sequencerChannel = 0;
    //private Sequencer initialisationSequence;

    //private void Awake()
    //{
    //    var sequencers = GetComponents<Sequencer>();
    //    initialisationSequence = sequencers.Single<Sequencer>(x => x.CompareChannel(sequencerChannel));
    //}

    public void SetForm(FormProfileSO form) 
    {
        activeForm = form;
    }
    public void SetIndex(int index) 
    {
        currentFormIndex = index;
        SetAnimator();
    }
    private void SetAnimator()
    {
        activeAnimator = availableAnimators[currentFormIndex];
    }
}
