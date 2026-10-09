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
        public Vector3 PourOutlet { get; private set; }
        GameObject pourCap, pourSample;
        Vector3 pourSampleScale;
        public void Configure(Core.StationController station, ToolKind kind, string label)
        {
            Station = station; Kind = kind; Label = label;
            HomePosition = transform.localPosition; homeRotation = transform.localRotation; homeScale = transform.localScale;
            // Visual model colliders are removed at end of frame; keep the stable wrapper collider.
            colliders = GetComponents<Collider>();
            flow = gameObject.AddComponent<Equipment.FlowEffect>();
            PourOutlet = kind == ToolKind.CaOSpoon ? Vector3.up*.07f : Vector3.up*.24f;
            flow.Configure(kind == ToolKind.CaOSpoon ? Color.white : new Color(.75f,.9f,1f), PourOutlet);
        }
        public void ConfigurePour(Vector3 outlet, GameObject cap = null, GameObject sample = null)
        { PourOutlet = outlet; pourCap = cap; pourSample = sample; flow.SetOutlet(outlet); }
        // Solve for the object's origin from its outlet, not its pivot. This keeps the stream
        // vertically above the beaker even as the bottle/spoon rotates.
        public static Vector3 PositionForOutlet(Vector3 outletWorld, Quaternion rotation, Vector3 outletLocal, Vector3 scale)
            => outletWorld - rotation * Vector3.Scale(outletLocal, scale);
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
            foreach (Collider c in colliders) if (c != null) c.enabled = false;
            if (pourCap != null) pourCap.SetActive(false);
            if (pourSample != null) pourSampleScale = pourSample.transform.localScale;
            Vector3 liquidPoint = destination.position - Vector3.up * .025f;
            Vector3 outletAbove = destination.position + Vector3.up * .24f;
            Quaternion upright = Station.transform.rotation * homeRotation;
            Vector3 abovePosition = PositionForOutlet(outletAbove, upright, PourOutlet, transform.lossyScale);
            yield return MoveTo(abovePosition, upright, .45f, homeScale);
            Quaternion tilted = Station.transform.rotation * Quaternion.Euler(0, 0, Kind == ToolKind.IndicatorBottle ? -125 : -75);
            float elapsed = 0;
            while (elapsed < .4f)
            {
                elapsed += Time.deltaTime; float t = Mathf.SmoothStep(0, 1, Mathf.Clamp01(elapsed / .4f));
                Quaternion rotation = Quaternion.Slerp(upright, tilted, t);
                transform.SetPositionAndRotation(PositionForOutlet(outletAbove, rotation, PourOutlet, transform.lossyScale), rotation);
                yield return null;
            }
            flow.SetFlow(true, liquidPoint);
            elapsed = 0;
            while (elapsed < .75f)
            {
                elapsed += Time.deltaTime;
                if (pourSample != null) pourSample.transform.localScale = pourSampleScale * (1f - Mathf.Clamp01(elapsed / .75f));
                yield return null;
            }
            flow.SetFlow(false, liquidPoint); finish?.Invoke();
            elapsed = 0;
            while (elapsed < .4f)
            {
                elapsed += Time.deltaTime; float t = Mathf.SmoothStep(0, 1, Mathf.Clamp01(elapsed / .4f));
                Quaternion rotation = Quaternion.Slerp(tilted, upright, t);
                transform.SetPositionAndRotation(PositionForOutlet(outletAbove, rotation, PourOutlet, transform.lossyScale), rotation);
                yield return null;
            }
            yield return MoveTo(Station.transform.TransformPoint(HomePosition), upright, .5f, homeScale);
            transform.localPosition = HomePosition; transform.localRotation = homeRotation;
            if (pourCap != null) pourCap.SetActive(true);
            foreach (Collider c in colliders) if (c != null) c.enabled = true;
            Busy = false;
        }
        public void ResetObject()
        {
            StopAllCoroutines(); Busy = false;
            if (flow != null) flow.SetFlow(false, transform.position);
            if (pourCap != null) pourCap.SetActive(true);
            if (pourSample != null && pourSampleScale != Vector3.zero) pourSample.transform.localScale = pourSampleScale;
            if (colliders != null) foreach (Collider c in colliders) if (c != null) c.enabled = true;
            transform.localPosition = HomePosition; transform.localRotation = homeRotation; transform.localScale = homeScale;
        }
    }
}
