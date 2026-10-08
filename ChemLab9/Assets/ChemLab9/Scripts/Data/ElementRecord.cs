using System;
namespace ChemLab9.Data
{
    public enum ElementCategory { AlkaliMetal, AlkalineEarth, Metal, Metalloid, Nonmetal, Halogen, NobleGas }
    [Serializable]
    public class ElementRecord
    {
        public string symbol, name, oldGroup;
        public int atomicNumber, group, period;
        public float atomicMass;
        public float relativeRadius;
        public string radiusNote;
        public int[] shells;
        public ElementCategory category;
        public bool Valid()
        {
            if (atomicNumber < 1 || atomicNumber > 20 || group < 1 || group > 18 || shells == null || shells.Length != period) return false;
            int sum = 0; foreach (int n in shells) { if (n <= 0) return false; sum += n; }
            return sum == atomicNumber;
        }
    }
}
