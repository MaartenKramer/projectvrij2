using System.Linq;
using UnityEngine;

public class FormController : MonoBehaviour
{
    [System.Serializable]
    private struct FormItem
    {
        public string id;
        public GameObject form;
    }

    [SerializeField] FormItem[] availableForms;
    [Space]
    [SerializeField] private FormItem currentForm;
    private int currentFormIndex;

    public bool SwitchForms(int index)
    {
        // set new index id
        currentFormIndex = index;

        // get the specified form
        GameObject form = availableForms[index].form;
        if(form == null) { return false; }

        SwapForms(form);
        return true;
    }

    public bool SwitchForms(string id)
    {
        // set new index id
        for (int i = 0; i < availableForms.Length; i++)
        {
            if (availableForms[i].id == id) { currentFormIndex = i; break; }
        }

        // get the specified form
        GameObject form = availableForms.Single(x => x.id == id).form;
        if(form == null) { return false; }

        SwapForms(form);
        return true;
    }

    private void SwapForms(GameObject form)
    {
        for(int i = 0; i < availableForms.Length; i++)
        {
            if (availableForms[i].form == form) { form.SetActive(true); }
            else { availableForms[i].form.SetActive(false); }
        }

        currentForm = availableForms[currentFormIndex];
    }
}
