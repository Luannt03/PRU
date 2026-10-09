using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEditor.Build.Reporting;

namespace SwimDemo.Editor
{
    // Sub-asset IDs belong to Unity's FBX importer. Resolve the real model, Avatar and clip here.
    [InitializeOnLoad]
    public static class SwimProjectSetup
    {
        public const string ModelPath = "Assets/SwimDemo/Characters/YBot_Swim.fbx";
        public const string SettingsPath = "Assets/SwimDemo/Resources/SwimSettings.asset";
        public const string ScenePath = "Assets/SwimDemo/Scenes/Swimming.unity";
        internal const string ImportVersion = "SwimDemo-Humanoid-v1";
        static bool queued, configuring;

        static SwimProjectSetup() { QueueSetup(); }
        internal static void QueueSetup()
        {
            if (queued) return;
            queued = true;
            EditorApplication.delayCall += RunDelayed;
        }
        static void RunDelayed()
        {
            queued = false;
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            { QueueSetup(); return; }
            if (EditorApplication.isPlayingOrWillChangePlaymode || configuring) return;
            try
            {
                if (!Configure(false)) return;
                Scene active = SceneManager.GetActiveScene();
                if (string.IsNullOrEmpty(active.path) && !active.isDirty)
                    EditorSceneManager.OpenScene(ScenePath);
            }
            catch (Exception exception) { Debug.LogError("SwimDemo setup: " + exception.Message); }
        }

        [MenuItem("SwimDemo/Setup / Repair Mixamo Import")]
        public static void Repair() { Configure(true); }

        public static bool Configure(bool forceImport)
        {
            if (configuring) return false;
            configuring = true;
            try
            {
                ModelImporter importer = AssetImporter.GetAtPath(ModelPath) as ModelImporter;
                if (importer == null) throw new InvalidOperationException("Không tìm thấy " + ModelPath);
                if (forceImport || importer.userData != ImportVersion || importer.animationType != ModelImporterAnimationType.Human)
                {
                    SwimModelImporter.ConfigureImporter(importer);
                    importer.SaveAndReimport();
                }
                GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(ModelPath);
                UnityEngine.Object[] parts = AssetDatabase.LoadAllAssetsAtPath(ModelPath);
                Avatar avatar = parts.OfType<Avatar>().FirstOrDefault(a => a.isValid && a.isHuman);
                AnimationClip clip = parts.OfType<AnimationClip>().FirstOrDefault(c => !c.name.StartsWith("__preview__") && c.length > 0 && c.isHumanMotion);
                if (model == null || avatar == null || clip == null)
                    throw new InvalidOperationException("FBX chưa có Avatar/clip Humanoid hợp lệ. Chọn YBot_Swim.fbx > Rig > Configure để xem mapping xương; không đổi sang Generic.");
                SwimSettings settings = AssetDatabase.LoadAssetAtPath<SwimSettings>(SettingsPath);
                if (settings == null)
                {
                    settings = ScriptableObject.CreateInstance<SwimSettings>();
                    AssetDatabase.CreateAsset(settings, SettingsPath);
                }
                if (settings.CharacterModel != model || settings.CharacterAvatar != avatar || settings.SwimmingClip != clip)
                {
                    settings.CharacterModel = model; settings.CharacterAvatar = avatar; settings.SwimmingClip = clip;
                    EditorUtility.SetDirty(settings); AssetDatabase.SaveAssets();
                    Debug.Log("SwimDemo: đã gắn Y Bot, Avatar Humanoid và clip '" + clip.name + "' (" + clip.length.ToString("F2") + " giây). Mở Swimming và nhấn Play.");
                }
                return settings.IsReady;
            }
            finally { configuring = false; }
        }

        [MenuItem("SwimDemo/Open Swimming Scene")]
        public static void OpenScene()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            EditorSceneManager.OpenScene(ScenePath);
        }

        [MenuItem("SwimDemo/Validate Project")]
        public static void ValidateMenu()
        {
            try { Configure(false); Validate(); Debug.Log("SwimDemo: cấu hình model, Avatar, clip, shader, input và scene hợp lệ. Tiếp tục kiểm tra Play Mode theo README."); }
            catch (Exception exception) { Debug.LogError("SwimDemo: " + exception.Message); }
        }
        public static void Validate()
        {
            SwimSettings settings = AssetDatabase.LoadAssetAtPath<SwimSettings>(SettingsPath);
            if (settings == null || !settings.IsReady) throw new InvalidOperationException("Thiếu model/Avatar/clip Humanoid.");
            if (GraphicsSettings.defaultRenderPipeline != null || QualitySettings.renderPipeline != null)
                throw new InvalidOperationException("SwimDemo được cấu hình Built-in. Graphics/Quality đang có Render Pipeline Asset; kiểm tra lại project đã mở.");
            foreach (string shader in new[] { "SwimDemo/Solid", "SwimDemo/PoolWater", "SwimDemo/Bubbles" })
                if (Shader.Find(shader) == null || !Shader.Find(shader).isSupported)
                    throw new InvalidOperationException("Shader thiếu hoặc không được hỗ trợ: " + shader);
            var playerAssets = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset");
            if (playerAssets.Length == 0) throw new InvalidOperationException("Không đọc được Player Settings; kiểm tra project đã import xong.");
            var player = new SerializedObject(playerAssets[0]);
            var input = player.FindProperty("activeInputHandler");
            if (input != null && input.intValue != 0)
                throw new InvalidOperationException("Đặt Player > Other Settings > Active Input Handling = Input Manager (Old), rồi khởi động lại Editor.");
            if (!EditorBuildSettings.scenes.Any(s => s.enabled && s.path == ScenePath))
                throw new InvalidOperationException("Swimming.unity chưa được bật trong Build Profiles > Scene List.");
        }

        [MenuItem("SwimDemo/Build Windows x64")]
        public static void BuildWindows()
        {
            Configure(false); Validate();
            if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64))
                throw new InvalidOperationException("Cài Windows Build Support cho Unity 6000.6.0f1 trong Unity Hub > Installs > Manage > Add modules.");
            BuildReport report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] { ScenePath }, locationPathName = "Builds/Windows/SwimDemo.exe",
                target = BuildTarget.StandaloneWindows64, options = BuildOptions.None
            });
            if (report.summary.result != BuildResult.Succeeded)
                throw new InvalidOperationException("Build chưa thành công; xem lỗi đầu tiên trong Console.");
            Debug.Log("Build Windows x64 hoàn tất: Builds/Windows. Gửi toàn bộ thư mục này, không chỉ file .exe.");
        }
    }

    public class SwimModelImporter : AssetPostprocessor
    {
        internal static void ConfigureImporter(ModelImporter importer)
        {
            importer.animationType = ModelImporterAnimationType.Human;
            importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
            importer.importAnimation = true; importer.optimizeGameObjects = false;
            importer.globalScale = 1; importer.useFileScale = true;
            importer.resampleCurves = true; importer.userData = SwimProjectSetup.ImportVersion;
        }
        void OnPreprocessModel()
        {
            if (assetPath == SwimProjectSetup.ModelPath) ConfigureImporter((ModelImporter)assetImporter);
        }
        void OnPreprocessAnimation()
        {
            if (assetPath != SwimProjectSetup.ModelPath) return;
            var importer = (ModelImporter)assetImporter;
            var clips = importer.defaultClipAnimations;
            foreach (var clip in clips)
            {
                clip.loopTime = true; clip.loopPose = true;
                clip.keepOriginalOrientation = true;
                clip.lockRootRotation = true;
                // Let Humanoid extract the FBX translation. Runtime also anchors the hips,
                // so an imported root/pose translation can never override keyboard movement.
                clip.lockRootPositionXZ = false; clip.lockRootHeightY = false;
            }
            importer.clipAnimations = clips;
        }
        static void OnPostprocessAllAssets(string[] imported, string[] deleted, string[] moved, string[] movedFrom)
        {
            if (imported.Contains(SwimProjectSetup.ModelPath)) SwimProjectSetup.QueueSetup();
        }
    }
}
