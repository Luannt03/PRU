using UnityEngine;
namespace ChemLab9.Data
{
    [CreateAssetMenu(menuName = "ChemLab9/Substance")]
    public class SubstanceData : ScriptableObject
    {
        public string id, formula, substanceName, physicalState;
        public Color displayColor = Color.white;
        public bool isIndicator;
    }
}
