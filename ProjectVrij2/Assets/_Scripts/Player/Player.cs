using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private FormProfileSO activeForm;
    public FormProfileSO ActiveForm => activeForm;

    [SerializeField] private InputController inputController;
    public InputController InputController => inputController;

    public void SetForm(FormProfileSO form) { activeForm = form; }
}
