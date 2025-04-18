using UnityEngine;

[CreateAssetMenu(fileName = "New Form Profile", menuName ="ScriptableObjects/Transformations/FormProfile")]
public class FormProfileSO : ScriptableObject
{
    [Header("General")]

    // identifiers
    [SerializeField] public string id;                  // id with which the profile can be looked-up | structure: type_owner_name (e.g. form_player_bird or form_enemy-grunt_human)
    [SerializeField] public string formName;            // for clarity purposes mostly

    // movement variables
    [SerializeField] public float defaultSpeed;         // speed for the default way of movement (i.e. walking/locomotion)
    [SerializeField] public float sprintMultiplier;
    [Space]
    [SerializeField] public float altSpeed;             // speed for any alternate way of movement the form has (e.g. flight speed for bird-form)

    [Header("Form specific")]

    // form behaviour
    [SerializeReference]
    [SerializeField] public IFormBehaviour behaviour;   // associated behaviour and implementation of the form's movement
    //[SerializeField] public System.Type[] states;     // could be like this, but "hard-code" it for now in the IFormBehaviour
}
