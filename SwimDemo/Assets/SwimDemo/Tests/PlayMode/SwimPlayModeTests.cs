using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
namespace SwimDemo.Tests
{
    public class SwimPlayModeTests
    {
        [UnitySetUp]
        public IEnumerator OpenDemo()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene("Swimming");
            yield return null; yield return null;
        }
        [UnityTearDown]
        public IEnumerator RestoreTime() { Time.timeScale = 1; yield return null; }
        [UnityTest]
        public IEnumerator UploadedCharacterAnimatesAndStaysAnchoredAcrossALoop()
        {
            var swimmer = Object.FindFirstObjectByType<SwimmerController>();
            Assert.That(swimmer, Is.Not.Null, "Run SwimDemo/Setup / Repair Mixamo Import first.");
            var animator = swimmer.GetComponentInChildren<Animator>();
            Assert.That(animator.avatar.isValid && animator.avatar.isHuman, Is.True);
            Transform hips = animator.GetBoneTransform(HumanBodyBones.Hips);
            Transform arm = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
            Quaternion before = arm.localRotation;
            yield return new WaitForSeconds(.25f);
            Assert.That(Quaternion.Angle(before, arm.localRotation), Is.GreaterThan(.1f), "The uploaded clip must move joints.");
            float duration = swimmer.Settings.SwimmingClip.length / .7f + .2f;
            float elapsed = 0;
            while (elapsed < duration)
            {
                yield return null; elapsed += Time.deltaTime;
                Assert.That(Vector3.Distance(hips.position, swimmer.VisualPivot.position), Is.LessThan(.005f), "Root translation escaped keyboard control.");
            }
        }
        [UnityTest]
        public IEnumerator PauseFreezesWavesJointsAndPositionThenResumeRestartsThem()
        {
            var swimmer = Object.FindFirstObjectByType<SwimmerController>();
            var water = Object.FindFirstObjectByType<WaterSurface>();
            var ui = Object.FindFirstObjectByType<SwimDemoUI>();
            Assert.That(swimmer, Is.Not.Null);
            yield return null;
            var arm = swimmer.GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.LeftUpperArm);
            ui.SetPause(true); var rotation = arm.localRotation;
            var position = swimmer.transform.position; double waveTime = water.SimulationTime;
            yield return new WaitForSecondsRealtime(.25f);
            Assert.That(Quaternion.Angle(rotation, arm.localRotation), Is.LessThan(.01f));
            Assert.That(Vector3.Distance(position, swimmer.transform.position), Is.LessThan(.001f));
            Assert.That(water.SimulationTime, Is.EqualTo(waveTime));
            ui.SetPause(false); yield return new WaitForSeconds(.25f);
            Assert.That(water.SimulationTime, Is.GreaterThan(waveTime));
            Assert.That(Quaternion.Angle(rotation, arm.localRotation), Is.GreaterThan(.1f));
        }
    }
}
