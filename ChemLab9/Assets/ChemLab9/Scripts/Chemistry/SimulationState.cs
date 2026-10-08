using System;
using System.Collections.Generic;

namespace ChemLab9.Chemistry
{
    // One unit is a gameplay dose, not moles or millilitres. Aqueous states are simplified.
    public enum Substance { CO2, H2O, H2CO3, CaO, CaOH2, CaCO3, CuOH2, CuO }
    public enum ReactionKind { None, CarbonicAcid, Limewater, Hydration, Decomposition }

    public sealed class SimulationState
    {
        readonly Dictionary<Substance, float> amounts = new Dictionary<Substance, float>();
        public float Capacity { get; }
        public float HeatSeconds { get; private set; }
        public float GasDose { get; private set; }
        public ReactionKind LastReaction { get; private set; }
        public bool IndicatorAdded { get; private set; }
        public bool IsAcidic => Amount(Substance.H2CO3) > 0;
        public bool IsBasic => Amount(Substance.CaOH2) > 0;
        public bool IndicatorPink => IndicatorAdded && IsBasic;
        public float Total { get { float total = 0; foreach (float value in amounts.Values) total += value; return total; } }
        public SimulationState(float capacity = 12f)
        {
            if (!Finite(capacity) || capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity));
            Capacity = capacity;
        }
        static bool Finite(float value) => !float.IsNaN(value) && !float.IsInfinity(value);
        public float Amount(Substance substance) => amounts.TryGetValue(substance, out float n) ? n : 0f;
        public bool Add(Substance substance, float dose)
        {
            if (!Finite(dose) || dose <= 0 || Total + dose > Capacity + 0.0001f) return false;
            amounts[substance] = Amount(substance) + dose;
            return true;
        }
        public void Reset(params Substance[] initial)
        {
            amounts.Clear(); HeatSeconds = GasDose = 0; LastReaction = ReactionKind.None; IndicatorAdded = false;
            foreach (Substance substance in initial) Add(substance, substance == Substance.H2O ? 8f : 1f);
        }
        void Consume(Substance substance, float dose) => amounts[substance] = Math.Max(0, Amount(substance) - dose);
        public ReactionKind DeliverGas(float dose, bool tubeConnected, string sourceId = "CO2")
        {
            if (!tubeConnected || sourceId != "CO2" || !Finite(dose) || dose <= 0 || GasDose >= 1f) return ReactionKind.None;
            float accepted = Math.Min(dose, 1f - GasDose);
            // Restrict MVP to a single dose for each receiving beaker. No excess-CO2 mode.
            if (Amount(Substance.CaOH2) > 0 && Amount(Substance.CaOH2) + .0001f >= accepted)
            {
                if (Total + accepted > Capacity + 0.0001f) return ReactionKind.None;
                Consume(Substance.CaOH2, accepted); Add(Substance.CaCO3, accepted); Add(Substance.H2O, accepted);
                GasDose = Math.Min(1f, GasDose + accepted); return LastReaction = ReactionKind.Limewater;
            }
            if (Amount(Substance.H2O) > 0 && Amount(Substance.H2O) + .0001f >= accepted && Amount(Substance.CaCO3) == 0 && Amount(Substance.CaO) == 0)
            {
                Consume(Substance.H2O, accepted); Add(Substance.H2CO3, accepted);
                GasDose = Math.Min(1f, GasDose + accepted); return LastReaction = ReactionKind.CarbonicAcid;
            }
            return ReactionKind.None;
        }
        public bool Hydrate(float dose, string sourceId = "CaO")
        {
            if (sourceId != "CaO" || !Finite(dose) || dose <= 0 || LastReaction == ReactionKind.Hydration || Amount(Substance.H2O) < dose) return false;
            if (Total + dose > Capacity + 0.0001f) return false;
            Add(Substance.CaO, dose); Consume(Substance.CaO, dose); Consume(Substance.H2O, dose);
            Add(Substance.CaOH2, dose); LastReaction = ReactionKind.Hydration; return true;
        }
        public void AddIndicator() => IndicatorAdded = true;
        public bool Heat(float seconds, bool heaterOn, bool inHeatingZone, float requiredSeconds)
        {
            if (!Finite(seconds) || seconds <= 0 || !Finite(requiredSeconds) || requiredSeconds <= 0 || !heaterOn || !inHeatingZone || Amount(Substance.CuOH2) <= 0) return false;
            if (Total + Amount(Substance.CuOH2) > Capacity) return false;
            HeatSeconds = Math.Min(requiredSeconds, HeatSeconds + seconds);
            if (HeatSeconds < requiredSeconds) return false;
            float dose = Amount(Substance.CuOH2); Consume(Substance.CuOH2, dose);
            Add(Substance.CuO, dose); Add(Substance.H2O, dose); LastReaction = ReactionKind.Decomposition; return true;
        }
    }
}
