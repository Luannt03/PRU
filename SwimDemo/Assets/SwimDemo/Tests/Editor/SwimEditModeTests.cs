using System.Collections.Generic;
using NUnit.Framework;
using SwimDemo.Editor;
namespace SwimDemo.Tests
{
    public class SwimEditModeTests
    {
        public static IEnumerable<TestCaseData> Scenarios()
        {
            foreach (var entry in MovementScenarios.Cases)
                yield return new TestCaseData(entry.Key).SetName(entry.Key);
        }
        [TestCaseSource(nameof(Scenarios))]
        public void Movement(string name)
        { foreach (var entry in MovementScenarios.Cases) if (entry.Key == name) { entry.Value(); return; } Assert.Fail("Unknown scenario"); }
        [Test]
        public void ImportedModelAndBuildConfigurationAreValid()
        { Assert.That(SwimProjectSetup.Configure(false), Is.True); SwimProjectSetup.Validate(); }
    }
}
