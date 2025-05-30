using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance == null) { Instance = this; }
        else { Destroy(this); }

        brain = Camera.main.GetComponent<CinemachineBrain>();
    }

    [Header("Camera's")]
    [SerializeField] private CM_CamItem[] cams;

    [Header("Debug")]
    [SerializeField] private CM_CamItem activeCam;

    private CinemachineBrain brain;

    public CM_CamItem GetCMCamItem(string id)
    {
        foreach (CM_CamItem camItem in cams)
        {
            if (camItem.id == id)
            {
                return camItem;
            }
        }

        Debug.Log($"[Camera] No camera found with id: {id}! returning null");
        return new CM_CamItem();
    }

    public bool SwitchCMCam(string id) 
    {
        // disable active camera
        if(!activeCam.CompareId(null)) // if active camera isn't null, disable active came
        { 
            activeCam.obj.SetActive(false); 
        }    
        
        // enable target camera
        CM_CamItem target = GetCMCamItem(id);
        if (target.CompareId(null)) 
        {
            return false; 
        }
        target.obj.SetActive(true);
        activeCam = target;

        return true;
    }
}

[System.Serializable]
public struct CM_CamItem
{
    public string id;
    public GameObject obj;
    public CinemachineCamera cam;

    public bool CompareId(string idToCompare)
    {
        if(id == idToCompare || (idToCompare == null) && id == "") { return true; }
        else { return false; }
    }
}
