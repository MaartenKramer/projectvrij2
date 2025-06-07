using UnityEngine;
using System.Collections.Generic;

namespace VFX.Controllers.Shaders
{
    public enum VFX_MaterialProcessing { SINGLE, MULTIPLE }
    public class VFX_ShaderController : VFX_Controller
    {
        [Header("references")]
        [SerializeField] protected List<Material> materials;
        [SerializeField] VFX_MaterialProcessing materialProcessing;

        protected override void Start()
        {
            base.Start();

            switch (materialProcessing)
            {
                case VFX_MaterialProcessing.SINGLE:
                    materials.Add(GetComponentInChildren<MeshRenderer>().material);
                    break;
                case VFX_MaterialProcessing.MULTIPLE:
                    var meshes = GetComponentsInChildren<MeshRenderer>(true);
                    foreach (var mesh in meshes) 
                    { 
                        materials.Add(mesh.material); 
                    }
                    break;
            }
        }

        protected bool ReassignMaterial(bool inChildren = true) 
        {
            if(materialProcessing == VFX_MaterialProcessing.MULTIPLE) { return false; }

            Material newMaterial;

            if (inChildren) { newMaterial = GetComponentInChildren<Material>(); }
            else { newMaterial = GetComponent<Material>();}

            if(newMaterial == null) { return false; }

            materials[0] = newMaterial;
            return true;
        }
        protected void SetTexture(string name, Texture2D tex) 
        {
            foreach (Material m in materials) 
            {
                m.SetTexture(name, tex); 
            }  
        }
        protected void SetColor(string name, Color color) 
        {
            foreach (Material m in materials)
            {
                m.SetColor(name, color);
            }
        }
        protected void SetFloat(string name, float value) 
        {
            foreach (Material m in materials)
            {
                m.SetFloat(name, value);
            }
        }
    }
}
