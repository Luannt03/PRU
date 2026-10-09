using UnityEngine;
namespace ChemLab9.Equipment
{
    public class GasTubeVisual : MonoBehaviour
    {
        public Transform Source, Tip;
        LineRenderer line;
        public void Configure(Transform source, Transform tip)
        {
            Source = source; Tip = tip; line = gameObject.AddComponent<LineRenderer>(); line.positionCount = 16;
            line.startWidth = line.endWidth = .025f; line.sharedMaterial = Core.ChemLabBootstrap.Factory.Material(new Color(.35f,.55f,.65f));
        }
        void LateUpdate()
        {
            for (int i=0;i<16;i++) { float t=i/15f; Vector3 p=Vector3.Lerp(Source.position+Vector3.up*.3f,Tip.position,t); p.y += Mathf.Sin(t*Mathf.PI)*.22f; line.SetPosition(i,p); }
        }
    }
}
