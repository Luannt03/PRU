using UnityEngine;
using ChemLab9.Data;
using ChemLab9.Core;
namespace ChemLab9.PeriodicTable
{
    public class AtomVisualizer : MonoBehaviour
    {
        Transform content;
        public void Clear() { if (content != null) { content.gameObject.SetActive(false); ChemLabBootstrap.Factory.ReleaseVisuals(content.gameObject); LabFactory.Remove(content.gameObject); content = null; } }
        public void Show(ElementRecord element)
        {
            Clear(); LabFactory f = ChemLabBootstrap.Factory; content = f.Empty("Atom_" + element.symbol, transform, Vector3.zero).transform;
            f.Shape("Nucleus_Z" + element.atomicNumber, content, Vector3.zero, Vector3.one * .16f, new Color(1f,.45f,.3f), PrimitiveType.Sphere);
            for (int shell = 0; shell < element.shells.Length; shell++)
            {
                float radius = .18f + shell * .105f;
                Transform layer = f.Empty("Shell_" + (shell+1), content, Vector3.zero).transform;
                LineRenderer ring = layer.gameObject.AddComponent<LineRenderer>(); ring.useWorldSpace = false; ring.loop = true; ring.positionCount = 64;
                ring.startWidth = ring.endWidth = .006f; ring.sharedMaterial = f.Material(new Color(.4f,.8f,1f));
                for (int i=0;i<64;i++) { float angle = i * Mathf.PI * 2f / 64; ring.SetPosition(i, new Vector3(Mathf.Cos(angle),Mathf.Sin(angle),0) * radius); }
                for (int i=0;i<element.shells[shell];i++)
                {
                    float angle = i * Mathf.PI * 2f / element.shells[shell];
                    f.Shape("Electron_" + i, layer, new Vector3(Mathf.Cos(angle),Mathf.Sin(angle),0) * radius, Vector3.one * .035f, new Color(.2f,1f,.9f), PrimitiveType.Sphere);
                }
            }
        }
        void Update() { if (content != null && Time.timeScale > 0) foreach (Transform shell in content) if (shell.name.StartsWith("Shell_")) shell.Rotate(0,0,Time.deltaTime * 18f); }
    }
}
