using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace ChemLab9.Core
{
    public sealed class LabFactory
    {
        public TMP_FontAsset Font { get; }
        readonly Material template, transparentTemplate;
        readonly List<Material> materials = new List<Material>();
        public LabFactory(TMP_FontAsset font, Material opaque, Material transparent)
        { Font = font; template = opaque; transparentTemplate = transparent; }
        public Material Material(Color color, bool transparent = false)
        {
            Material material = new Material(transparent ? transparentTemplate : template);
            material.color = color; materials.Add(material); return material;
        }
        public static void Remove(Object obj)
        { if (Application.isPlaying) Object.Destroy(obj); else Object.DestroyImmediate(obj); }
        public static string ReadableText(string value)
        {
            return (value ?? "").Replace("⇌", "<=>").Replace("→", "->").Replace("↓", "(kết tủa)")
                .Replace("✓", "[X]").Replace("≈", "~");
        }
        public GameObject Empty(string name, Transform parent, Vector3 position)
        {
            GameObject go = new GameObject(name); go.transform.SetParent(parent, false); go.transform.localPosition = position; return go;
        }
        public GameObject Shape(string name, Transform parent, Vector3 position, Vector3 scale, Color color, PrimitiveType type = PrimitiveType.Cube, bool collider = false, bool transparent = false)
        {
            GameObject go = GameObject.CreatePrimitive(type); go.name = name;
            go.transform.SetParent(parent, false); go.transform.localPosition = position; go.transform.localScale = scale;
            go.GetComponent<Renderer>().sharedMaterial = Material(color, transparent);
            if (!collider) { go.layer = 2; var c = go.GetComponent<Collider>(); c.enabled = false; Remove(c); }
            return go;
        }
        public TMP_Text Label(string name, Transform parent, Vector3 position, string text, float size = .11f, Color? color = null)
        {
            GameObject go = new GameObject(name, typeof(RectTransform)); go.transform.SetParent(parent, false); go.transform.localPosition = position; go.layer = 2;
            TextMeshPro label = go.AddComponent<TextMeshPro>(); label.font = Font; label.text = ReadableText(text);
            label.fontSize = size * 10f; label.color = color ?? Color.white;
            label.alignment = TextAlignmentOptions.Center; label.rectTransform.sizeDelta = new Vector2(4f, 1.4f);
            label.textWrappingMode = TextWrappingModes.Normal; return label;
        }
        public TMP_Text EquipmentLabel(string name, Transform parent, Vector3 position, string text, float size = .048f)
        {
            TMP_Text label = Label(name, parent, position, text, size);
            label.rectTransform.sizeDelta = new Vector2(.78f, .20f);
            label.enableAutoSizing = true; label.fontSizeMin = size * 7f; label.fontSizeMax = size * 10f;
            label.overflowMode = TextOverflowModes.Ellipsis;
            return label;
        }
        public TMP_Text PanelLabel(string name, Transform parent, Vector3 position, string text, Vector2 size, float fontSize, Color background)
        {
            Shape(name + "Background", parent, position, new Vector3(size.x, size.y, .025f), background);
            TMP_Text label = Label(name, parent, position + Vector3.back * .02f, text, fontSize);
            label.rectTransform.sizeDelta = size - new Vector2(.06f, .025f);
            label.enableAutoSizing = true; label.fontSizeMin = fontSize * 7.5f; label.fontSizeMax = fontSize * 10f;
            label.overflowMode = TextOverflowModes.Ellipsis;
            return label;
        }
        public GameObject Model(string name, GameObject prefab, Transform parent, Vector3 position, Vector3 size, bool glass = false)
        {
            GameObject wrapper = Empty(name, parent, position);
            if (prefab == null)
            {
                // Unity's Cylinder has a native height of 2. Match the requested height so
                // the bottle neck/outlet at y=.29 really lies above its y=.25 body.
                Vector3 cylinderScale = new Vector3(size.x, size.y * .5f, size.z);
                Shape("PrimitiveModel", wrapper.transform, Vector3.up * size.y * .5f, cylinderScale,
                    glass ? new Color(.7f,.9f,1f,.25f) : new Color(.3f,.5f,.55f), PrimitiveType.Cylinder, false, glass);
                return wrapper;
            }
            GameObject model = Object.Instantiate(prefab); model.name = "Model";
            model.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity); model.transform.localScale = Vector3.one;
            foreach (Collider c in model.GetComponentsInChildren<Collider>()) { c.enabled = false; Remove(c); }
            Renderer[] renderers = model.GetComponentsInChildren<Renderer>();
            if (renderers.Length > 0)
            {
                Bounds bounds = renderers[0].bounds; foreach (Renderer r in renderers) bounds.Encapsulate(r.bounds);
                Vector3 extent = bounds.size;
                Vector3 fit = new Vector3(size.x / Mathf.Max(extent.x,.001f), size.y / Mathf.Max(extent.y,.001f), size.z / Mathf.Max(extent.z,.001f));
                model.transform.SetParent(wrapper.transform, false); model.transform.localScale = fit;
                model.transform.localPosition = Vector3.Scale(new Vector3(-bounds.center.x, -bounds.min.y, -bounds.center.z), fit);
                foreach (Renderer r in renderers)
                {
                    r.gameObject.layer = 2;
                    if (glass) { Material[] mats = r.sharedMaterials; for (int i=0;i<mats.Length;i++) mats[i] = Material(new Color(.7f,.9f,1f,.23f), true); r.sharedMaterials = mats; }
                }
            }
            else model.transform.SetParent(wrapper.transform, false);
            return wrapper;
        }
        public void ReleaseVisuals(GameObject root)
        {
            foreach (Renderer renderer in root.GetComponentsInChildren<Renderer>(true))
                foreach (Material material in renderer.sharedMaterials)
                    if (material != null && materials.Remove(material)) Remove(material);
        }
        public void Dispose() { foreach (Material m in materials) if (m != null) Remove(m); if (Font != null) { Remove(Font.material); foreach (var texture in Font.atlasTextures) if (texture != null) Remove(texture); Remove(Font); } }
    }
}
