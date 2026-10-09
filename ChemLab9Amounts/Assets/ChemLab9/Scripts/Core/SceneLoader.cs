using UnityEngine;
using UnityEngine.SceneManagement;
namespace ChemLab9.Core
{
    public static class SceneLoader
    {
        public static void StartLab(int lessonId = 0) { GameManager.RequestedLesson = lessonId; Time.timeScale = 1; SceneManager.LoadScene("ChemistryLab"); }
        public static void MainMenu() { Time.timeScale = 1; SceneManager.LoadScene("MainMenu"); }
        public static void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
