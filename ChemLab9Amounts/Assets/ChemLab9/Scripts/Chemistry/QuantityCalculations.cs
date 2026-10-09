using System;
using System.Globalization;

namespace ChemLab9.Chemistry
{
    // Grade-9 rounded molar masses: CO2=44, Ca(OH)2=74, CaCO3=100, CaO=56, H2O=18 g/mol.
    // Results are theoretical masses of pure substances, not measured experimental yields.
    public static class QuantityCalculations
    {
        public const double LimewaterCO2Grams = .044;
        public const double LimewaterCaOH2Grams = .074;
        public const double WaterGrams = 50;
        public const float MinCaOGrams = .1f, MaxCaOGrams = 5f;
        static void RequireMass(double mass)
        {
            if (double.IsNaN(mass) || double.IsInfinity(mass) || mass < 0)
                throw new ArgumentOutOfRangeException(nameof(mass));
        }
        public static double PrecipitateGrams(double co2Grams, double caoh2Grams)
        {
            RequireMass(co2Grams); RequireMass(caoh2Grams);
            if (co2Grams / 44 > caoh2Grams / 74 + 1e-10)
                throw new ArgumentException("This lesson does not simulate excess CO2 dissolving the precipitate.");
            return Math.Min(co2Grams / 44, caoh2Grams / 74) * 100;
        }
        public static HydrationYield Hydrate(double caoGrams, double waterGrams)
        {
            RequireMass(caoGrams); RequireMass(waterGrams);
            double moles = Math.Min(caoGrams / 56, waterGrams / 18);
            return new HydrationYield
            {
                CaOReacted = moles * 56, WaterConsumed = moles * 18,
                CaOH2Produced = moles * 74, CaORemaining = Math.Max(0, caoGrams - moles * 56),
                WaterRemaining = Math.Max(0, waterGrams - moles * 18)
            };
        }
        public static bool TryReadCaOMass(string text, out float mass)
        {
            mass = 0;
            string normalized = (text ?? "").Trim().Replace(',', '.');
            return float.TryParse(normalized, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                CultureInfo.InvariantCulture, out mass) && !float.IsNaN(mass) && !float.IsInfinity(mass)
                && mass >= MinCaOGrams && mass <= MaxCaOGrams;
        }
    }
    public struct HydrationYield
    {
        public double CaOReacted, WaterConsumed, CaOH2Produced, CaORemaining, WaterRemaining;
    }
}
