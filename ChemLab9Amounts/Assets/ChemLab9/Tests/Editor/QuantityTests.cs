using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using ChemLab9.Interaction;
namespace ChemLab9.Tests
{
    public class QuantityTests
    {
        public static IEnumerable<TestCaseData> Cases()
        { foreach(var item in QuantityScenarios.Cases) yield return new TestCaseData(item.Key).SetName(item.Key); }
        [TestCaseSource(nameof(Cases))]
        public void QuantityLogic(string name) => QuantityScenarios.Cases[name]();
        [TestCase(-75f,0f)] [TestCase(-125f,0f)] [TestCase(-75f,180f)] [TestCase(-125f,180f)]
        public void TiltKeepsRealOutletDirectlyAboveCup(float tilt, float yaw)
        {
            Vector3 destination=new Vector3(4.2f,1.52f,3.6f), outlet=new Vector3(0,.29f,.09f), scale=new Vector3(1,.9f,1.2f);
            Quaternion rotation=Quaternion.Euler(0,yaw,0)*Quaternion.Euler(0,0,tilt);
            Vector3 position=DraggableObject.PositionForOutlet(destination,rotation,outlet,scale);
            Vector3 actualOutlet=position+rotation*Vector3.Scale(outlet,scale);
            Assert.Less(Vector3.Distance(destination,actualOutlet),.0001f);
        }
    }
}
