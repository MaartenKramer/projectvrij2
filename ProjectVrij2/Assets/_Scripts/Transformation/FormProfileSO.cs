using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "New Form Profile", menuName ="ScriptableObjects/Transformations/FormProfile")]
public class FormProfileSO : ScriptableObject
{

    // identifiers
    [Header("Identification")]
    [SerializeField] public string id;                  // id with which the profile can be looked-up | structure: type_owner_name (e.g. form_player_bird or form_enemy-grunt_human)
    [SerializeField] public string formName;            // for clarity purposes mostly

    [Header("Variables")]
    [SerializeField] public float mass = 1f;
    [SerializeField] public float linearDrag = 1f;
    [SerializeField] public float angularDrag = 1f;
    //[SerializeField] public float defaultSpeed;         // speed for the default way of movement (i.e. walking/locomotion)
    //[SerializeField] public float sprintMultiplier;
    //[Space]
    //[SerializeField] public float altSpeed;             // speed for any alternate way of movement the form has (e.g. flight speed for bird-form)

    [Header("Form specific")]
    //[SerializeField] public InputActionAsset actionAsset;
    //[SerializeField] public string actionMapId;
    //[SerializeField] public System.Type[] states;     // could be like this, but "hard-code" it for now in the IFormBehaviour
    // form behaviour
    [SerializeReference]
    [SerializeField] public IFormBehaviour behaviour;   // associated behaviour and implementation of the form's movement
    [SerializeField] public string cameraId;
}
