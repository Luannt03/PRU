using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
namespace SwimDemo
{
    public class SwimDemoBootstrap : MonoBehaviour
    {
        public SwimSettings Settings;
        public Font UIFont;
        public Shader SolidShader, WaterShader, BubbleShader;
        readonly List<Material> materials = new List<Material>();
        Texture2D tiles;
        void Awake()
        {
            Time.timeScale = 1; Cursor.lockState = CursorLockMode.None; Cursor.visible = true;
            QualitySettings.vSyncCount = 1;
            var ui = gameObject.AddComponent<SwimDemoUI>(); ui.Font = UIFont;
            if (Settings == null || !Settings.IsReady || SolidShader == null || WaterShader == null || BubbleShader == null)
            {
                ui.SetupError = "Chưa cấu hình FBX. Dừng Play, chọn SwimDemo > Setup / Repair Mixamo Import, đợi import xong rồi Play lại.";
                Debug.LogError("SwimDemo: cần Avatar/clip Humanoid và đủ ba shader. Chạy SwimDemo/Setup / Repair Mixamo Import.", this);
                GameObject errorCamera = new GameObject("SetupCamera", typeof(Camera), typeof(AudioListener));
                errorCamera.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
                errorCamera.GetComponent<Camera>().backgroundColor = new Color(.03f, .11f, .16f); return;
            }
            RenderSettings.ambientMode = AmbientMode.Flat; RenderSettings.ambientLight = new Color(.46f, .57f, .64f);
            RenderSettings.skybox = null;
            GameObject sun = new GameObject("Sun", typeof(Light)); sun.transform.SetParent(transform, false);
            sun.transform.rotation = Quaternion.Euler(48, -35, 0);
            Light light = sun.GetComponent<Light>(); light.type = LightType.Directional; light.intensity = 1.2f; light.color = new Color(1, .96f, .88f);
            BuildPool();
            GameObject waterObject = new GameObject("WaterSurface"); waterObject.transform.SetParent(transform, false);
            WaterSurface water = waterObject.AddComponent<WaterSurface>(); water.Build(NewMaterial(WaterShader, new Color(.05f, .48f, .60f, .27f)));
            GameObject swimmerRoot = new GameObject("Swimmer"); swimmerRoot.transform.SetParent(transform, false);
            var swimmer = swimmerRoot.AddComponent<SwimmerController>(); swimmer.Settings = Settings; swimmer.Water = water; swimmer.Speed = Settings.MoveSpeed;
            GameObject visual = new GameObject("VisualPivot"); visual.transform.SetParent(swimmerRoot.transform, false); swimmer.VisualPivot = visual.transform;
            GameObject character = Instantiate(Settings.CharacterModel, visual.transform, false); character.name = "YBot";
            character.transform.localRotation = Quaternion.Euler(Settings.ModelEulerCorrection) * character.transform.localRotation;
            character.transform.localScale *= Settings.ModelScale;
            foreach (Collider collider in character.GetComponentsInChildren<Collider>()) { collider.enabled = false; Destroy(collider); }
            foreach (Renderer renderer in character.GetComponentsInChildren<Renderer>())
            {
                Color color = renderer.name == "Beta_Joints" ? new Color(.07f, .13f, .17f) : new Color(.24f, .56f, .65f);
                Material replacement = NewMaterial(SolidShader, color);
                replacement.SetFloat("_Gloss", renderer.name == "Beta_Joints" ? 24 : 70);
                Material[] slots = renderer.sharedMaterials;
                for (int i = 0; i < slots.Length; i++) slots[i] = replacement;
                renderer.sharedMaterials = slots;
            }
            Animator animator = character.GetComponent<Animator>() ?? character.AddComponent<Animator>();
            animator.avatar = Settings.CharacterAvatar;
            var animation = character.AddComponent<ImportedSwimAnimator>(); animation.Configure(animator, Settings.SwimmingClip, visual.transform);
            swimmer.Animation = animation; swimmer.ResetSwimmer(); BuildBubbles(swimmerRoot.transform);
            GameObject cameraObject = new GameObject("MainCamera", typeof(Camera), typeof(AudioListener)); cameraObject.tag = "MainCamera";
            cameraObject.transform.SetParent(transform, false);
            Camera camera = cameraObject.GetComponent<Camera>(); camera.fieldOfView = 55; camera.nearClipPlane = .05f; camera.farClipPlane = 80;
            camera.clearFlags = CameraClearFlags.SolidColor;
            SwimCamera cameraControl = cameraObject.AddComponent<SwimCamera>(); cameraControl.Target = swimmerRoot.transform; cameraControl.Water = water;
            ui.Swimmer = swimmer; ui.CameraControl = cameraControl;
        }
        Material NewMaterial(Shader shader, Color color)
        {
            Material material = new Material(shader); material.SetColor("_Color", color); materials.Add(material); return material;
        }
        GameObject Shape(string name, Vector3 position, Vector3 size, Material material, PrimitiveType type = PrimitiveType.Cube)
        {
            GameObject shape = GameObject.CreatePrimitive(type); shape.name = name; shape.transform.SetParent(transform, false);
            shape.transform.localPosition = position; shape.transform.localScale = size;
            Collider collider = shape.GetComponent<Collider>(); collider.enabled = false; Destroy(collider);
            shape.GetComponent<Renderer>().sharedMaterial = material; return shape;
        }
        void BuildPool()
        {
            tiles = new Texture2D(64, 64) { name = "PoolTiles", wrapMode = TextureWrapMode.Repeat, filterMode = FilterMode.Bilinear };
            var pixels = new Color[4096];
            for (int y = 0; y < 64; y++) for (int x = 0; x < 64; x++)
                pixels[y * 64 + x] = x < 2 || y < 2 ? new Color(.18f, .42f, .49f) : ((x / 32 + y / 32) % 2 == 0 ? new Color(.67f, .85f, .88f) : new Color(.50f, .74f, .80f));
            tiles.SetPixels(pixels); tiles.Apply();
            Material floor = NewMaterial(SolidShader, Color.white); floor.mainTexture = tiles; floor.mainTextureScale = new Vector2(18, 12);
            Material wall = NewMaterial(SolidShader, new Color(.38f, .70f, .78f));
            Material rim = NewMaterial(SolidShader, new Color(.88f, .91f, .87f));
            Material deck = NewMaterial(SolidShader, new Color(.34f, .40f, .44f));
            Shape("PoolFloor", new Vector3(0, -3.6f, 0), new Vector3(18.2f, .2f, 12.2f), floor);
            Shape("LeftWall", new Vector3(-9.1f, -1.75f, 0), new Vector3(.2f, 3.5f, 12.4f), wall);
            Shape("RightWall", new Vector3(9.1f, -1.75f, 0), new Vector3(.2f, 3.5f, 12.4f), wall);
            Shape("FrontWall", new Vector3(0, -1.75f, -6.1f), new Vector3(18, 3.5f, .2f), wall);
            Shape("BackWall", new Vector3(0, -1.75f, 6.1f), new Vector3(18, 3.5f, .2f), wall);
            for (int sign = -1; sign <= 1; sign += 2)
            {
                Shape("SideRim_" + sign, new Vector3(sign * 9.1f, .08f, 0), new Vector3(.4f, .16f, 12.6f), rim);
                Shape("EndRim_" + sign, new Vector3(0, .08f, sign * 6.1f), new Vector3(18, .16f, .4f), rim);
                Shape("SideDeck_" + sign, new Vector3(sign * 10.7f, -.12f, 0), new Vector3(3, .2f, 18), deck);
                Shape("EndDeck_" + sign, new Vector3(0, -.12f, sign * 7.7f), new Vector3(18.4f, .2f, 3), deck);
            }
            Material marker = NewMaterial(SolidShader, new Color(1f, .65f, .15f));
            for (int i = 0; i < 4; i++) Shape("FloatingMarker_" + i, new Vector3(i < 2 ? -7.9f : 7.9f, .02f, i % 2 == 0 ? -4.8f : 4.8f), Vector3.one * .25f, marker, PrimitiveType.Sphere);
        }
        void BuildBubbles(Transform swimmer)
        {
            GameObject bubbles = new GameObject("BubbleTrail"); bubbles.transform.SetParent(swimmer, false);
            bubbles.transform.localPosition = new Vector3(0, .06f, -.55f); bubbles.transform.localRotation = Quaternion.Euler(-90, 0, 0);
            ParticleSystem particles = bubbles.AddComponent<ParticleSystem>(); particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            var main = particles.main; main.playOnAwake = false; main.startLifetime = 1.2f; main.startSpeed = .45f; main.startSize = .035f;
            main.startColor = new Color(.8f, .97f, 1f, .6f); main.simulationSpace = ParticleSystemSimulationSpace.World; main.maxParticles = 80;
            var emission = particles.emission; emission.rateOverTime = 10;
            var shape = particles.shape; shape.shapeType = ParticleSystemShapeType.Cone; shape.angle = 10; shape.radius = .04f;
            var color = particles.colorOverLifetime; color.enabled = true;
            Gradient gradient = new Gradient(); gradient.SetKeys(new[] { new GradientColorKey(Color.white, 0), new GradientColorKey(Color.white, 1) }, new[] { new GradientAlphaKey(.6f, 0), new GradientAlphaKey(0, 1) }); color.color = gradient;
            particles.GetComponent<ParticleSystemRenderer>().sharedMaterial = NewMaterial(BubbleShader, Color.white); particles.Play();
        }
        void OnDestroy()
        {
            foreach (Material material in materials) if (material != null) Destroy(material);
            if (tiles != null) Destroy(tiles);
            Time.timeScale = 1; Cursor.lockState = CursorLockMode.None; Cursor.visible = true;
        }
    }
}
