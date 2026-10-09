using UnityEngine;
namespace ChemLab9.Equipment
{
    public class FlowEffect : MonoBehaviour
    {
        LineRenderer line;
        Vector3 target;
        Vector3 outlet;
        public void Configure(Color color, Vector3 localOutlet)
        {
            outlet = localOutlet;
            line = gameObject.AddComponent<LineRenderer>(); line.positionCount = 2;
            line.startWidth = .009f; line.endWidth = .004f;
            line.sharedMaterial = Core.ChemLabBootstrap.Factory.Material(color); line.enabled = false;
        }
        public void SetFlow(bool active, Vector3 destination) { if (line == null) return; target = destination; line.enabled = active; }
        public void SetOutlet(Vector3 localOutlet) { outlet = localOutlet; }
        void LateUpdate() { if (line != null && line.enabled) { line.SetPosition(0, transform.TransformPoint(outlet)); line.SetPosition(1, target); } }
    }
}
