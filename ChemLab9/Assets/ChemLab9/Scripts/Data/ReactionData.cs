using UnityEngine;
using ChemLab9.Chemistry;
namespace ChemLab9.Data
{
    [CreateAssetMenu(menuName = "ChemLab9/Reaction")]
    public class ReactionData : ScriptableObject
    {
        public ReactionKind kind;
        public Substance[] reactants, products;
        public int[] reactantCoefficients, productCoefficients;
        public string equation, condition;
        [TextArea] public string explanation;
    }
}
