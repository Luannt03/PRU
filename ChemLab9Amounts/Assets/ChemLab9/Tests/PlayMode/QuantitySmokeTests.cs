using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using TMPro;
using ChemLab9.Core;
using ChemLab9.Interaction;
using ChemLab9.Lessons;
namespace ChemLab9.Tests
{
    public class QuantitySmokeTests
    {
        [UnityTearDown] public IEnumerator Cleanup() { Time.timeScale=1; yield return SceneManager.LoadSceneAsync("MainMenu"); }
        static T Named<T>(Component root, string name) where T:Component
        { foreach(var item in root.GetComponentsInChildren<T>(true)) if(item.name==name) return item; Assert.Fail("Missing "+name); return null; }
        [UnityTest]
        public IEnumerator QuantityResultAppearsOnlyAfterLimewaterCompletes()
        {
            yield return SceneManager.LoadSceneAsync("ChemistryLab"); yield return null;
            var game=GameManager.Instance; var lesson=Object.FindFirstObjectByType<Lesson01Manager>();
            game.EnterStation(lesson.Station); lesson.Begin(); yield return null;
            Slider result=Named<Slider>(game.UI,"ReactionResultSlider");
            Assert.IsFalse(result.gameObject.activeSelf);
            lesson.Limewater.State.DeliverGas(.5f,true); yield return null;
            Assert.IsFalse(result.gameObject.activeSelf);
            lesson.Limewater.State.DeliverGas(.5f,true); yield return null;
            Assert.IsTrue(result.gameObject.activeInHierarchy); Assert.IsFalse(result.interactable);
            Assert.AreEqual(.1f,result.value,.0001f);
            StringAssert.Contains("CaCO3",Named<TMP_Text>(game.UI,"QuantityResult").text);
            lesson.Station.ResetLesson(); yield return null; Assert.IsFalse(result.gameObject.activeSelf);
        }
        [UnityTest]
        public IEnumerator SelectedMassLocksDuringPourAndResultsMatchTheCapturedDose()
        {
            yield return SceneManager.LoadSceneAsync("ChemistryLab"); yield return null;
            var game=GameManager.Instance; var lesson=Object.FindFirstObjectByType<Lesson02Manager>();
            game.EnterStation(lesson.Station); lesson.Begin(); yield return null;
            TMP_InputField input=Named<TMP_InputField>(game.UI,"CaOMassInput"); input.text="2,00";
            Assert.AreEqual(2,lesson.SelectedMassGrams,.001f);
            var zone=lesson.Beaker.GetComponentInChildren<DropZone>();
            Assert.IsTrue(lesson.ApplyDrop(lesson.CaOSpoon,zone)); Assert.IsFalse(lesson.SetMassFromText("5"));
            yield return new WaitForSeconds(1f);
            Vector3 expected=zone.SnapPoint.position+Vector3.up*.24f;
            Assert.Less(Vector3.Distance(lesson.CaOSpoon.transform.TransformPoint(lesson.CaOSpoon.PourOutlet),expected),.002f);
            yield return new WaitForSeconds(4.2f);
            Assert.IsTrue(lesson.ReactionComplete); Assert.AreEqual(2,lesson.Yield.CaOReacted,.001);
            Assert.IsTrue(Named<Slider>(game.UI,"ReactionResultSlider").gameObject.activeInHierarchy);
            Assert.IsTrue(lesson.ApplyDrop(lesson.IndicatorTool,zone));
            yield return new WaitForSeconds(1f);
            Assert.IsFalse(lesson.IndicatorTool.transform.Find("BottleCap").gameObject.activeSelf);
            Assert.Less(Vector3.Distance(lesson.IndicatorTool.transform.TransformPoint(lesson.IndicatorTool.PourOutlet),expected),.002f);
            yield return new WaitForSeconds(2f); Assert.IsTrue(lesson.Ready);
            lesson.Station.ResetLesson(); yield return null;
            Assert.IsTrue(lesson.CanChangeMass); Assert.IsFalse(lesson.ReactionComplete);
        }
        [UnityTest]
        public IEnumerator LeavingDuringPourCancelsAddingTheChemical()
        {
            yield return SceneManager.LoadSceneAsync("ChemistryLab"); yield return null;
            var game=GameManager.Instance; var lesson=Object.FindFirstObjectByType<Lesson02Manager>();
            game.EnterStation(lesson.Station); lesson.Begin();
            Assert.IsTrue(lesson.ApplyDrop(lesson.CaOSpoon,lesson.Beaker.GetComponentInChildren<DropZone>()));
            yield return new WaitForSeconds(.2f); game.ExitStation();
            yield return new WaitForSeconds(2f);
            Assert.IsFalse(lesson.ReactionComplete); Assert.IsTrue(lesson.CanChangeMass);
            Assert.AreEqual(0,lesson.Yield.CaOReacted);
        }
    }
}
