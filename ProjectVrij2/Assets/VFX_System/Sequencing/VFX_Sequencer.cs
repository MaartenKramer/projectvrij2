using UnityEngine;
using VFX.Controllers;
using System.Collections.Generic;
using System.Linq;

namespace Sequencing
{
    public class VFX_Sequencer : Sequencer
    {
        [System.Serializable]
        private struct VFX_ControllerItem
        {
            public string id;
            public VFX_Controller controller;
        }

        [SerializeField] private List<VFX_ControllerItem> controllers = new List<VFX_ControllerItem>();

        public bool GetController(string id, out VFX_Controller controller)
        {
            controller = controllers.Single(x => x.id == id).controller;
            if (controller == null) { return false; }
            return true;
        }
        public bool GetControllers(out VFX_Controller[] c)
        {
            var items = this.controllers;
            List<VFX_Controller> contr = new List<VFX_Controller>();
            foreach (var item in items)
            {
                contr.Add(item.controller);
            }
            c = contr.ToArray();

            if(c.Length == 0 ) { return false; }
            return true;
        }

        public override void Kill()
        {
            base.Kill();

            foreach (var item in controllers)
            {
                item.controller.Stop();
            }
        }
    }
}
