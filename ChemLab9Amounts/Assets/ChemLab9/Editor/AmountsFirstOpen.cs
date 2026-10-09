using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
namespace ChemLab9.Editor
{
    [InitializeOnLoad]
    public static class AmountsFirstOpen
    {
        static AmountsFirstOpen() { EditorApplication.delayCall += OpenWhenReady; }
        static void OpenWhenReady()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            { EditorApplication.delayCall += OpenWhenReady; return; }
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            Scene scene = SceneManager.GetActiveScene();
            if (string.IsNullOrEmpty(scene.path) && !scene.isDirty)
                EditorSceneManager.OpenScene("Assets/ChemLab9/Scenes/MainMenu.unity");
        }
    }
}
