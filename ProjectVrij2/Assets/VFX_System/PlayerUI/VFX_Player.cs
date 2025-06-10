using UnityEngine;
using UnityEngine.InputSystem;
using VFX.Controllers;

namespace VFX.Controls
{
    public class VFX_Player : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private InputActionAsset actionAsset;
        private string mapName = "VFX";
        private string actionName = "Trigger";
        private string clickName = "Click";

        private bool isPlaying = false;
        private InputAction triggerAction;

        [Header("UI")]
        [SerializeField] private VFX_PlayerControls controls;
        [SerializeField] private LayerMask clickMask;
        private InputAction clickAction;


        private void Start()
        {
            triggerAction = actionAsset.FindActionMap(mapName).FindAction(actionName);
            clickAction = actionAsset.FindActionMap(mapName).FindAction(clickName);
        }

        private void Update()
        {
            //if (triggerAction.triggered) 
            //{
            //    Debug.Log("[VFX player] input received!");

            //    if (isPlaying) { EventHandler.InvokeEvent(GlobalEvents.VFX_STOP); }
            //    else { EventHandler.InvokeEvent(GlobalEvents.VFX_PLAY); }

            //    isPlaying = !isPlaying;
            //}

            if (clickAction.triggered && !controls.OverControls)
            {
                Debug.Log($"clicked!");

                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    Transform objectHit = hit.transform;

                    // Do something with the object that was hit by the raycast.
                    Debug.Log($"Hit! {hit.transform.gameObject.name}");

                    VFX_Controller[] allControllers = objectHit.GetComponentsInChildren<VFX_Controller>();

                    VFX_Controller vfx = allControllers[0];
                    for(int i = 1; i < allControllers.Length; i++)
                    {
                        if (allControllers[i].priority <= vfx.priority) { continue; }
                        vfx = allControllers[i];
                    }

                    if (vfx != null)
                    {
                        controls.Open(vfx);
                    }
                    else
                    {
                        Debug.Log($"No VFX found!");
                    }
                }
                else
                {
                    if (controls.IsOpen && !controls.OverControls)
                    {
                        controls.Close();
                    }
                }
            }
        }
    }
}
