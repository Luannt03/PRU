using UnityEngine;
using UnityEngine.EventSystems;
using ChemLab9.Core;
using ChemLab9.Interaction;
namespace ChemLab9
{
    public class PlayerInteractor : MonoBehaviour
    {
        public Camera View;
        public float InteractDistance = 3.5f;
        DraggableObject dragging;
        DropZone highlighted;
        Vector3 dragOffset;
        public void CancelDrag()
        {
            if (dragging != null) { dragging.EndDrag(null); dragging = null; }
            SetHighlight(null);
        }
        void SetHighlight(DropZone zone)
        {
            if (zone == highlighted) return;
            if (highlighted != null) highlighted.Highlight(false);
            highlighted = zone;
            if (highlighted != null) highlighted.Highlight(true);
        }
        bool OnUI => EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        void Update()
        {
            GameManager game = GameManager.Instance;
            if (game == null || game.IsMenu || View == null || game.Paused) return;
            if (game.ActiveStation == null)
            {
                string prompt = "WASD: di chuyển • E: vào bàn • Esc: tạm dừng";
                if (Physics.Raycast(View.ViewportPointToRay(new Vector3(.5f, .5f, 0)), out RaycastHit hit, InteractDistance))
                {
                    StationController station = hit.collider.GetComponentInParent<StationController>();
                    if (station != null) { prompt = station.Prompt; if (Input.GetKeyDown(KeyCode.E)) station.Enter(); }
                }
                game.UI.SetPrompt(prompt); return;
            }
            Ray ray = View.ScreenPointToRay(Input.mousePosition);
            if (dragging != null)
            {
                StationController station = game.ActiveStation;
                Plane plane = new Plane(Vector3.up, station.transform.TransformPoint(new Vector3(0, station.WorkHeight, 0)));
                if (plane.Raycast(ray, out float distance))
                {
                    Vector3 local = station.transform.InverseTransformPoint(ray.GetPoint(distance) + dragOffset);
                    local.x = Mathf.Clamp(local.x, -1.65f, 1.65f); local.z = Mathf.Clamp(local.z, -0.7f, 0.65f); local.y = station.WorkHeight;
                    dragging.transform.localPosition = local;
                }
                DropZone zone = null;
                if (!OnUI && Physics.Raycast(ray, out RaycastHit dropHit, 8f)) zone = dropHit.collider.GetComponentInParent<DropZone>();
                if (zone != null && !zone.Accepts(dragging)) zone = null;
                SetHighlight(zone);
                if (Input.GetMouseButtonUp(0)) { dragging.EndDrag(zone); dragging = null; SetHighlight(null); }
                return;
            }
            if (OnUI) return;
            if (Physics.Raycast(ray, out RaycastHit toolHit, 8f))
            {
                Interactable item = toolHit.collider.GetComponentInParent<Interactable>();
                if (item == null || item.Station != game.ActiveStation) return;
                game.UI.SetPrompt(item.Label);
                if (!Input.GetMouseButtonDown(0)) return;
                DraggableObject tool = item as DraggableObject;
                if (tool != null && tool.BeginDrag())
                {
                    dragging = tool;
                    Plane plane = new Plane(Vector3.up, tool.transform.position);
                    dragOffset = plane.Raycast(ray, out float distance) ? tool.transform.position - ray.GetPoint(distance) : Vector3.zero;
                }
                else if (tool == null) item.Activate();
            }
        }
    }
}
