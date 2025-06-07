using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VFX.Controllers;

namespace VFX.Controls
{
    public class VFX_PlayerControls : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private Image loopIcon;

        private GameObject obj;

        private bool isOpen = false;
        public bool IsOpen => isOpen;

        private bool overControls;
        public bool OverControls => overControls;

        [Header("| Do NOT touch! |")]
        [SerializeField] private VFX_Controller selected;

        private void Awake()
        {
            obj = transform.gameObject;
            Debug.Log($"obj: {obj.name}");
        }

        private void Start()
        {
            isOpen = false;
            obj.SetActive(false);
        }

        public void Open(VFX_Controller selected)
        {
            obj.SetActive(true);
            isOpen = true;

            this.selected = selected;

            label.text = $"obj: {selected.ObjName} - vfx: {selected.vfxName}";
            loopIcon.enabled = selected.isLooping;
        }

        public void Close()
        {
            if (!isOpen) { return; }

            // reset ui element
            label.text = "undefined";
            
            selected = null;

            isOpen = false; 
            obj.SetActive(false);

            overControls = false;
        }

        public void Trigger() { selected.Trigger(); }
        public void SetLoop() { selected.SetLoop(); }
        public void ToggleIcon() 
        {
            loopIcon.enabled = !loopIcon.enabled; 
        }
        public void Stop() { selected.Stop(); }
        public void Reset() { selected.Reset(); }

        public void OnPointerEnter(PointerEventData eventData)
        {
            overControls = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            overControls = false;
        }
    }
}
