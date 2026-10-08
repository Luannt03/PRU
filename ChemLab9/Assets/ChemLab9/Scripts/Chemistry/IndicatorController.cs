using UnityEngine;
namespace ChemLab9.Chemistry
{
    public static class IndicatorController
    {
        public static Color LitmusColor(SimulationState state, bool adequateDose) => state.IsAcidic && adequateDose ? Color.red : new Color(.6f,.2f,.8f);
        public static bool PhenolphthaleinPink(SimulationState state) => state.IndicatorPink;
    }
}
