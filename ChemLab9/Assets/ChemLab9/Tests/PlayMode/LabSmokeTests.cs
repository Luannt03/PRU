using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using ChemLab9.Core;
using ChemLab9.Interaction;
using ChemLab9.Lessons;
namespace ChemLab9.Tests
{
    public class LabSmokeTests
    {
        [UnityTearDown]
        public IEnumerator Cleanup()
        {
            Time.timeScale=1; AudioListener.pause=false;
            yield return SceneManager.LoadSceneAsync("MainMenu");
        }
        [UnityTest]
        public IEnumerator BootstrapBuildsFiveStationsAndToolsSurviveColliderCleanup()
        {
            yield return SceneManager.LoadSceneAsync("ChemistryLab"); yield return null;
            var game=GameManager.Instance;
            Assert.NotNull(game); Assert.NotNull(game.UI);
            Assert.AreEqual(1,Object.FindObjectsByType<PlayerController>(FindObjectsSortMode.None).Length);
            Assert.AreEqual(1,Object.FindObjectsByType<Camera>(FindObjectsSortMode.None).Length);
            StationController[] stations=Object.FindObjectsByType<StationController>(FindObjectsSortMode.None);
            Assert.AreEqual(5,stations.Length);
            foreach(StationController station in stations)
            {
                game.EnterStation(station); station.Lesson.Begin();
                foreach(DraggableObject item in station.GetComponentsInChildren<DraggableObject>())
                {
                    Vector3 before=item.transform.localPosition;
                    Assert.IsTrue(item.BeginDrag()); item.transform.localPosition+=Vector3.right*.2f;
                    item.EndDrag(null); Assert.AreEqual(before,item.transform.localPosition);
                }
                station.ResetLesson(); game.ExitStation();
            }
            Assert.IsNull(game.ActiveStation);
        }
        [UnityTest]
        public IEnumerator PauseFreezesHeatingAndExitStopsEquipment()
        {
            yield return SceneManager.LoadSceneAsync("ChemistryLab"); yield return null;
            var game=GameManager.Instance;
            Lesson08Manager lesson=Object.FindFirstObjectByType<Lesson08Manager>();
            game.EnterStation(lesson.Station); lesson.Begin();
            var sample=lesson.Sample.GetComponent<DraggableObject>();
            var zone=lesson.GetComponentInChildren<DropZone>();
            Assert.IsTrue(lesson.ApplyDrop(sample,zone)); lesson.Heater.Toggle();
            yield return new WaitForSeconds(.2f);
            Assert.Greater(lesson.Sample.State.HeatSeconds,0);
            game.SetPause(true); float seconds=lesson.Sample.State.HeatSeconds;
            yield return new WaitForSecondsRealtime(.2f);
            Assert.AreEqual(seconds,lesson.Sample.State.HeatSeconds); Assert.AreEqual(0,Time.timeScale);
            game.SetPause(false); game.ExitStation();
            Assert.IsFalse(lesson.Heater.IsOn); Assert.IsNull(game.ActiveStation); Assert.AreEqual(1,Time.timeScale);
        }
    }
}
