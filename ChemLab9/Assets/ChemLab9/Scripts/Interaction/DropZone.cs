using UnityEngine;
using ChemLab9.Core;
namespace ChemLab9.Interaction
{
    public enum ToolKind { GasTube, Litmus, CaOSpoon, IndicatorBottle, CopperSample }
    public class DropZone : MonoBehaviour
    {
        public StationController Station;
        public string ZoneId;
        public ToolKind[] Accepted;
        public Transform SnapPoint;
        public Renderer HighlightRenderer;
        Color original;
        public void Configure(StationController station, string id, ToolKind[] accepted, Renderer renderer)
        {
            Station = station; ZoneId = id; Accepted = accepted; HighlightRenderer = renderer;
            original = renderer.sharedMaterial.color;
        }
        public bool Accepts(DraggableObject item) => Station == item.Station && System.Array.IndexOf(Accepted, item.Kind) >= 0;
        public void Highlight(bool active)
        {
            if (HighlightRenderer != null) HighlightRenderer.sharedMaterial.color = active ? new Color(0.2f, 1f, 0.65f) : original;
        }
        public bool Receive(DraggableObject item) => Accepts(item) && Station.Lesson.ApplyDrop(item, this);
    }
}
