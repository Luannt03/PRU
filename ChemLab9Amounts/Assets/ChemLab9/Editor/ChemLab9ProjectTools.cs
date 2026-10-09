using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using ChemLab9.Core;
using ChemLab9.Data;
namespace ChemLab9.Editor
{
    public static class ChemLab9ProjectTools
    {
        const string Root = "Assets/ChemLab9/";
        [MenuItem("ChemLab9Amounts/Open Main Menu")]
        public static void OpenMenu() { if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) EditorSceneManager.OpenScene(Root+"Scenes/MainMenu.unity"); }
        [MenuItem("ChemLab9Amounts/Open Chemistry Lab")]
        public static void OpenLab() { if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) EditorSceneManager.OpenScene(Root+"Scenes/ChemistryLab.unity"); }
        [MenuItem("ChemLab9Amounts/Validate Project")]
        public static void Validate()
        {
            ChemicalDatabase db=AssetDatabase.LoadAssetAtPath<ChemicalDatabase>(Root+"Data/ChemicalDatabase.asset");
            if (db==null || db.elements==null || db.elements.Length!=20 || db.lessons.Length!=5 || db.reactions.Length!=4) throw new BuildFailedException("Database không đủ 20 nguyên tố/5 bài/4 phản ứng.");
            foreach(ElementRecord e in db.elements) if(!e.Valid()) throw new BuildFailedException("Sai dữ liệu electron: "+e.symbol);
            foreach(int id in ProgressLedger.LessonIds) if(db.Lesson(id)==null || db.Lesson(id).questions.Length==0) throw new BuildFailedException("Thiếu bài/câu hỏi: "+id);
            foreach(var lesson in db.lessons) foreach(var q in lesson.questions) if(q.answers==null || q.correctIndex<0 || q.correctIndex>=q.answers.Length) throw new BuildFailedException("Sai đáp án trong bài "+lesson.lessonId);
            foreach(string scene in new[]{"MainMenu","ChemistryLab"})
            {
                if(AssetDatabase.LoadAssetAtPath<SceneAsset>(Root+"Scenes/"+scene+".unity")==null) throw new BuildFailedException("Thiếu scene "+scene);
                if(!Array.Exists(EditorBuildSettings.scenes,s=>s.enabled && s.path==Root+"Scenes/"+scene+".unity")) throw new BuildFailedException("Thiếu scene trong Build Profiles: "+scene);
            }
            if(UnityEngine.Rendering.GraphicsSettings.defaultRenderPipeline==null) throw new BuildFailedException("Project cần URP hiện có. Gán PC_RPAsset trong Graphics/Quality.");
            Debug.Log("ChemLab9: dữ liệu, electron, câu hỏi, scene và render pipeline hợp lệ. Chạy Play Mode checklist để kiểm tra tương tác.");
        }
        [MenuItem("ChemLab9Amounts/Build Windows x64")]
        public static void BuildWindows()
        {
            Validate();
            if(!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Standalone,BuildTarget.StandaloneWindows64)) throw new BuildFailedException("Cài Windows Build Support trong Unity Hub trước khi build.");
            Directory.CreateDirectory("Builds/Windows");
            BuildReport report=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{Root+"Scenes/MainMenu.unity",Root+"Scenes/ChemistryLab.unity"},locationPathName="Builds/Windows/ChemLab9Amounts.exe",target=BuildTarget.StandaloneWindows64,options=BuildOptions.None});
            if(report.summary.result!=BuildResult.Succeeded) throw new BuildFailedException("Windows build thất bại: "+report.summary.result);
            Debug.Log("Build hoàn tất: Builds/Windows/ChemLab9Amounts.exe. Giữ nguyên các thư mục đi kèm khi sao chép demo.");
        }
        [MenuItem("ChemLab9Amounts/Create Project Prefab Wrappers")]
        public static void CreateWrappers()
        {
            string publisher="Assets/3D Laboratory Environment with Appratus/Prefabs/";
            string[] names={"Beaker","TestTube","ChemicalBottle","GasSource","GasTube","Heater","ExperimentTable","ElementTile","AtomModel","Liquid","Precipitate","ResultPanel","Player"};
            string[] sources={"Beaker","Glass_Lab_test_tube","Erlenmeyer_flask","florence_flask","","Spirit_Lamp with water","table with drawers","","","","","",""};
            Directory.CreateDirectory(Root+"Prefabs");
            for(int i=0;i<names.Length;i++)
            {
                string path=Root+"Prefabs/"+names[i]+".prefab"; if(AssetDatabase.LoadAssetAtPath<GameObject>(path)!=null) continue;
                GameObject root=new GameObject(names[i]);
                GameObject model=string.IsNullOrEmpty(sources[i])?null:AssetDatabase.LoadAssetAtPath<GameObject>(publisher+sources[i]+".prefab");
                if(model!=null) { var child=(GameObject)PrefabUtility.InstantiatePrefab(model); child.name="Model"; child.transform.SetParent(root.transform,false); }
                else if(names[i]!="Player" && names[i]!="ResultPanel") { var child=GameObject.CreatePrimitive(names[i]=="AtomModel"?PrimitiveType.Sphere:PrimitiveType.Cube); child.name="Model"; child.transform.SetParent(root.transform,false); }
                if(names[i]=="Player") { root.AddComponent<CharacterController>(); var view=new GameObject("FirstPersonCamera",typeof(Camera)); view.transform.SetParent(root.transform,false); view.transform.localPosition=Vector3.up*1.6f; root.AddComponent<PlayerController>(); root.AddComponent<PlayerInteractor>().View=view.GetComponent<Camera>(); }
                else if(names[i]=="ResultPanel") { root.AddComponent<RectTransform>(); root.AddComponent<UnityEngine.UI.Image>(); }
                else if(names[i]!="Liquid" && names[i]!="Precipitate") root.AddComponent<BoxCollider>();
                PrefabUtility.SaveAsPrefabAsset(root,path); UnityEngine.Object.DestroyImmediate(root);
            }
            AssetDatabase.SaveAssets(); Debug.Log("Đã tạo wrapper prefab. Gameplay hiện được bootstrap gán logic/reference trong Play; xem docs/PREFABS.md trước khi dùng wrapper độc lập.");
        }
    }
    public class ValidateBeforeBuild : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;
        public void OnPreprocessBuild(BuildReport report) => ChemLab9ProjectTools.Validate();
    }
}
