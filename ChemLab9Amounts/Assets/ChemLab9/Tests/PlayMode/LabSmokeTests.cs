using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using ChemLab9.Core;
using ChemLab9.Interaction;
using ChemLab9.Lessons;
using ChemLab9.PeriodicTable;
using ChemLab9.UI;
using TMPro;
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
        [UnityTest]
        public IEnumerator LitmusDipsThenPresentsAndResetCancelsPendingCheck()
        {
            yield return SceneManager.LoadSceneAsync("ChemistryLab"); yield return null;
            var game = GameManager.Instance;
            var lesson = Object.FindFirstObjectByType<Lesson01Manager>();
            game.EnterStation(lesson.Station); lesson.Begin();
            var paper = lesson.LitmusTool;
            var zone = lesson.Water.GetComponentInChildren<DropZone>();
            DraggableObject tube = null;
            foreach (var tool in lesson.GetComponentsInChildren<DraggableObject>())
                if (tool.Kind == ToolKind.GasTube) tube = tool;
            Assert.NotNull(tube);
            Assert.IsTrue(lesson.ApplyDrop(tube, zone));
            Physics.SyncTransforms();
            Vector3 tubeCenter = tube.transform.TransformPoint(Vector3.up * .1f);
            Assert.AreSame(tube, game.Interactor.FindTool(new Ray(tubeCenter - lesson.transform.forward, lesson.transform.forward)),
                "The receiving volume must not prevent picking up a connected gas tip.");
            lesson.Water.State.DeliverGas(1, true);
            Assert.IsTrue(paper.BeginDrag()); paper.EndDrag(zone);
            Assert.IsTrue(paper.Busy);
            Assert.AreNotEqual(Color.red, lesson.Litmus.sharedMaterial.color, "Color must not change before dipping.");
            yield return new WaitForSeconds(.2f);
            game.SetPause(true); Vector3 paused = paper.transform.position;
            yield return new WaitForSecondsRealtime(.15f);
            Assert.AreEqual(paused, paper.transform.position);
            game.SetPause(false);
            yield return new WaitForSeconds(2.5f);
            Assert.IsFalse(paper.Busy);
            Assert.Less(Vector3.Distance(paper.transform.localPosition, new Vector3(0, 1.23f, -.62f)), .001f);
            Assert.AreEqual(Color.red, lesson.Litmus.sharedMaterial.color);
            Assert.AreEqual(Vector3.one * 1.5f, paper.transform.localScale);
            lesson.Station.ResetLesson();
            Assert.AreEqual(paper.HomePosition, paper.transform.localPosition);
            Assert.AreEqual(Vector3.one, paper.transform.localScale);
            lesson.Water.State.DeliverGas(1, true);
            Assert.IsTrue(paper.BeginDrag()); paper.EndDrag(zone);
            yield return new WaitForSeconds(.2f);
            lesson.Station.ResetLesson();
            yield return new WaitForSeconds(2.5f);
            Assert.IsFalse(paper.Busy);
            Assert.AreEqual(paper.HomePosition, paper.transform.localPosition);
            Assert.AreEqual(new Color(.6f, .2f, .8f), lesson.Litmus.sharedMaterial.color, "A cancelled dip must not recolor the fresh sample.");
        }
        [UnityTest]
        public IEnumerator HeatingSnapKeepsTubeBottomAboveAlcoholLamp()
        {
            yield return SceneManager.LoadSceneAsync("ChemistryLab"); yield return null;
            var game = GameManager.Instance;
            var lesson = Object.FindFirstObjectByType<Lesson08Manager>();
            game.EnterStation(lesson.Station); lesson.Begin();
            var tool = lesson.Sample.GetComponent<DraggableObject>();
            var zone = lesson.GetComponentInChildren<DropZone>();
            Assert.IsTrue(lesson.ApplyDrop(tool, zone));
            Physics.SyncTransforms();
            Vector3 center = tool.transform.TransformPoint(Vector3.up * .1f);
            Assert.AreSame(tool, game.Interactor.FindTool(new Ray(center - lesson.transform.forward, lesson.transform.forward)));
            Assert.Less(Quaternion.Angle(tool.transform.rotation, zone.SnapPoint.rotation), .001f);
            Vector3 bottom = lesson.transform.InverseTransformPoint(tool.transform.position);
            Vector3 flame = lesson.Heater.HeatVisual.transform.localPosition;
            Assert.AreEqual(flame.x, bottom.x, .001f);
            Assert.AreEqual(flame.z, bottom.z, .001f);
            Assert.Greater(bottom.y, flame.y);
            Assert.Less(bottom.y - flame.y, .05f, "The tube must meet the flame, not float far above it.");
        }
        [UnityTest]
        public IEnumerator ElementInfoStaysVisibleAndTrendNotesStayBelowSpheres()
        {
            yield return SceneManager.LoadSceneAsync("ChemistryLab"); yield return null;
            var game = GameManager.Instance;
            var table = Object.FindFirstObjectByType<Lesson30Manager>();
            game.EnterStation(table.Station); table.Begin();
            table.Select(game.Database.Element("Na")); yield return null;
            TMP_Text details = FindText(game.UI, "ElementDetails");
            Assert.IsTrue(details.gameObject.activeInHierarchy);
            StringAssert.Contains("Z = 11", details.text);
            StringAssert.Contains("Proton = 11 | Electron = 11", details.text);
            StringAssert.Contains("2, 8, 1", details.text);
            table.Select(game.Database.Element("Ca")); yield return null;
            StringAssert.Contains("Z = 20", details.text);
            StringAssert.Contains("Số lớp electron: 4", details.text);
            table.Station.ResetLesson(); yield return null;
            StringAssert.DoesNotContain("Z = 20", details.text);
            game.ExitStation();
            var trends = Object.FindFirstObjectByType<Lesson31Manager>();
            game.EnterStation(trends.Station); trends.Begin();
            yield return new WaitForSeconds(2f);
            Transform radius = trends.Visualizer.transform.Find("RadiusMode");
            for (int i = 0; i < 6; i++)
            {
                string[] symbols = { "Li", "Na", "K", "Na", "Mg", "Al" };
                Transform sphere = radius.Find("AtomModel_" + i + "_" + symbols[i]);
                Transform note = radius.Find("Name_" + i + "Background");
                float noteTop = note.localPosition.y + note.localScale.y * .5f;
                float sphereBottom = sphere.localPosition.y - sphere.localScale.y * .5f;
                Assert.Less(noteTop, sphereBottom, "Notes must be below the sphere silhouettes.");
                Assert.Greater(note.localPosition.y - note.localScale.y * .5f, 1.1f, "Notes must stay above the table surface.");
                var label = radius.Find("Name_" + i).GetComponent<TMP_Text>();
                Assert.Less(label.rectTransform.rect.width, note.localScale.x);
                Assert.Less(label.rectTransform.rect.height, note.localScale.y);
            }
            trends.SetMode(1); yield return null;
            Assert.IsFalse(radius.gameObject.activeSelf);
            Assert.IsTrue(trends.Visualizer.transform.Find("MetallicMode").gameObject.activeSelf);
        }
        static TMP_Text FindText(LabUI ui, string name)
        {
            foreach (TMP_Text text in ui.GetComponentsInChildren<TMP_Text>(true))
                if (text.name == name) return text;
            Assert.Fail("Missing UI text: " + name); return null;
        }
    }
}
