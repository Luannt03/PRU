using System.Collections;
using UnityEngine;
namespace ChemLab9.Interaction
{
    public class DraggableObject : Interactable
    {
        public ToolKind Kind;
        public bool Busy { get; private set; }
        public Vector3 HomePosition { get; private set; }
        Quaternion homeRotation;
        Vector3 dragStart;
        Vector3 homeScale;
        Quaternion dragRotation;
        Collider[] colliders;
        Equipment.FlowEffect flow;
        public void Configure(Core.StationController station, ToolKind kind, string label)
        {
            Station = station; Kind = kind; Label = label;
            HomePosition = transform.localPosition; homeRotation = transform.localRotation; homeScale = transform.localScale;
            // Visual model colliders are removed at end of frame; keep the stable wrapper collider.
            colliders = GetComponents<Collider>();
            flow = gameObject.AddComponent<Equipment.FlowEffect>();
            flow.Configure(kind == ToolKind.CaOSpoon ? Color.white : new Color(.75f,.9f,1f), kind == ToolKind.CaOSpoon ? Vector3.up*.07f : Vector3.up*.24f);
        }
        public bool BeginDrag()
        {
            if (Busy || !Station.Lesson.Started) return false;
            if (Station.Lesson is Lessons.Lesson01Manager acid && acid.Gas.IsOn) return false;
            if (Station.Lesson is Lessons.Lesson08Manager heat && heat.Heater.IsOn) return false;
            dragStart = transform.localPosition; dragRotation = transform.localRotation;
            foreach (Collider c in colliders) if (c != null) c.enabled = false;
            return true;
        }
        public void EndDrag(DropZone zone)
        {
            foreach (Collider c in colliders) if (c != null) c.enabled = true;
            if (zone == null || !zone.Receive(this)) ReturnBeforeDrag();
        }
        public void ReturnBeforeDrag() { transform.localPosition = dragStart; transform.localRotation = dragRotation; }
        public void Snap(DropZone zone) { transform.SetPositionAndRotation(zone.SnapPoint.position, zone.SnapPoint.rotation); }
        public void AnimateDipAndPresent(Transform destination, Vector3 presentationPosition, System.Action checkIndicator)
        {
            StartCoroutine(DipRoutine(destination, presentationPosition, checkIndicator));
        }
        IEnumerator MoveTo(Vector3 position, Quaternion rotation, float seconds, Vector3 scale)
        {
            Vector3 from = transform.position, fromScale = transform.localScale;
            Quaternion fromRotation = transform.rotation;
            float elapsed = 0;
            while (elapsed < seconds)
            {
                elapsed += Time.deltaTime; float t = Mathf.SmoothStep(0, 1, Mathf.Clamp01(elapsed / seconds));
                transform.SetPositionAndRotation(Vector3.Lerp(from, position, t), Quaternion.Slerp(fromRotation, rotation, t));
                transform.localScale = Vector3.Lerp(fromScale, scale, t);
                yield return null;
            }
        }
        IEnumerator DipRoutine(Transform destination, Vector3 presentationPosition, System.Action checkIndicator)
        {
            Busy = true;
            foreach (Collider c in colliders) if (c != null) c.enabled = false;
            Quaternion upright = Station.transform.rotation;
            Vector3 aboveCup = destination.position + Vector3.up * .28f;
            yield return MoveTo(aboveCup, upright, .35f, homeScale);
            yield return MoveTo(destination.position - Vector3.up * .08f, upright, .45f, homeScale);
            yield return new WaitForSeconds(.4f);
            checkIndicator?.Invoke();
            yield return MoveTo(aboveCup, upright, .4f, homeScale);
            yield return MoveTo(Station.transform.TransformPoint(presentationPosition), upright, .55f, homeScale * 1.5f);
            foreach (Collider c in colliders) if (c != null) c.enabled = true;
            Busy = false;
        }
        public void AnimatePour(Transform destination, System.Action finish)
        {
            StartCoroutine(PourRoutine(destination, finish));
        }
        IEnumerator PourRoutine(Transform destination, System.Action finish)
        {
            Busy = true;
            Vector3 start = transform.position; Quaternion rotation = transform.rotation;
            Vector3 target = destination.position + Vector3.up * 0.35f;
            Quaternion tilted = Station.transform.rotation * Quaternion.Euler(0, 0, -65);
            float elapsed = 0;
            while (elapsed < 0.45f)
            {
                elapsed += Time.deltaTime; float t = Mathf.Clamp01(elapsed / 0.45f);
                transform.position = Vector3.Lerp(start, target, t); transform.rotation = Quaternion.Slerp(rotation, tilted, t); yield return null;
            }
            flow.SetFlow(true, destination.position);
            yield return new WaitForSeconds(0.65f);
            flow.SetFlow(false, destination.position); finish?.Invoke();
            elapsed = 0;
            while (elapsed < 0.45f)
            {
                elapsed += Time.deltaTime; float t = Mathf.Clamp01(elapsed / 0.45f);
                transform.position = Vector3.Lerp(target, Station.transform.TransformPoint(HomePosition), t);
                transform.rotation = Quaternion.Slerp(tilted, Station.transform.rotation * homeRotation, t); yield return null;
            }
            transform.localPosition = HomePosition; transform.localRotation = homeRotation; Busy = false;
        }
        public void ResetObject()
        {
            StopAllCoroutines(); Busy = false;
            if (flow != null) flow.SetFlow(false, transform.position);
            if (colliders != null) foreach (Collider c in colliders) if (c != null) c.enabled = true;
            transform.localPosition = HomePosition; transform.localRotation = homeRotation; transform.localScale = homeScale;
        }
    }
}
