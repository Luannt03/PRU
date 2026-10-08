using System.Collections.Generic;
using NUnit.Framework;
namespace ChemLab9.Tests
{
    public class ChemistryTests
    {
        public static IEnumerable<string> Names => RegressionScenarios.Cases.Keys;
        [TestCaseSource(nameof(Names))]
        public void GameplayRule(string name) => RegressionScenarios.Cases[name]();
    }
}
