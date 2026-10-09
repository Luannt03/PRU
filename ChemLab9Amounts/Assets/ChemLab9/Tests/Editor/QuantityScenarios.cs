using System;
using System.Collections.Generic;
using ChemLab9.Chemistry;
namespace ChemLab9.Tests
{
    public static class QuantityScenarios
    {
        static void Check(bool ok, string reason) { if (!ok) throw new Exception(reason); }
        static void Near(double actual, double expected, double tolerance = 1e-6)
        { Check(Math.Abs(actual - expected) < tolerance, actual + " != " + expected); }
        static void Throws(Action action)
        { try { action(); } catch (ArgumentException) { return; } throw new Exception("Expected invalid input rejection"); }
        public static readonly Dictionary<string, Action> Cases = new Dictionary<string, Action>
        {
            {"Lesson1FullDoseMakesPointOneGram", () => Near(QuantityCalculations.PrecipitateGrams(.044,.074),.1)},
            {"Lesson1HalfDoseMakesHalfPrecipitate", () => Near(QuantityCalculations.PrecipitateGrams(.022,.074),.05)},
            {"NoCO2MakesNoPrecipitate", () => Near(QuantityCalculations.PrecipitateGrams(0,.074),0)},
            {"ExcessCO2MustNotPretendPrecipitateKeepsIncreasing", () => Throws(() => QuantityCalculations.PrecipitateGrams(.088,.074))},
            {"OneGramCaOStoichiometry", () => { var y=QuantityCalculations.Hydrate(1,50); Near(y.CaOReacted,1); Near(y.CaOH2Produced,74d/56); Near(y.WaterConsumed,18d/56); Near(y.CaORemaining,0); }},
            {"TwoGramsCaODoublesYield", () => { var a=QuantityCalculations.Hydrate(1,50); var b=QuantityCalculations.Hydrate(2,50); Near(b.CaOH2Produced,a.CaOH2Produced*2); }},
            {"WaterLimitingReagentIsHandled", () => { var y=QuantityCalculations.Hydrate(56,9); Near(y.CaOReacted,28); Near(y.CaOH2Produced,37); Near(y.CaORemaining,28); Near(y.WaterRemaining,0); }},
            {"MassIsConservedAcrossAllowedDoseRange", () => { for(int i=1;i<=50;i++) { double m=i*.1; var y=QuantityCalculations.Hydrate(m,50); Near(m+50,y.CaOH2Produced+y.CaORemaining+y.WaterRemaining); Check(y.WaterRemaining>=0,"Negative water"); } }},
            {"NoCaOOrNoWaterDoesNotReact", () => { Near(QuantityCalculations.Hydrate(0,50).CaOH2Produced,0); Near(QuantityCalculations.Hydrate(1,0).CaOH2Produced,0); }},
            {"InvalidMassesAreRejected", () => { Throws(()=>QuantityCalculations.Hydrate(-1,50)); Throws(()=>QuantityCalculations.Hydrate(double.NaN,50)); Throws(()=>QuantityCalculations.PrecipitateGrams(double.PositiveInfinity,.074)); }},
            {"CommaAndDotInputBothWork", () => { Check(QuantityCalculations.TryReadCaOMass("1,25",out float a),"Comma rejected"); Check(QuantityCalculations.TryReadCaOMass("1.25",out float b),"Dot rejected"); Near(a,b); }},
            {"EmptyTextAndOutOfRangeInputAreRejected", () => { foreach(string v in new[]{"", "abc", "NaN", "Infinity", "0", "-1", "5.01", "1,2,3"}) Check(!QuantityCalculations.TryReadCaOMass(v,out _),"Accepted "+v); }},
            {"BothMassEndpointsAreAccepted", () => { Check(QuantityCalculations.TryReadCaOMass("0.10",out _),"Minimum rejected"); Check(QuantityCalculations.TryReadCaOMass("5.00",out _),"Maximum rejected"); }},
            {"LimewaterDoseIsHalfAtFiveSecondsAndFullAtTen", () => { var s=new SimulationState(); s.Reset(Substance.CaOH2); for(int i=0;i<50;i++) s.DeliverGas(.01f,true); Near(s.GasDose,.5,1e-5); Near(QuantityCalculations.PrecipitateGrams(.044*s.GasDose,.074),.05,1e-5); for(int i=0;i<50;i++) s.DeliverGas(.01f,true); Near(s.GasDose,1,1e-5); }},
            {"AmountMappingCreatesBasicMixtureAtEverySelectableMass", () => { for(int i=1;i<=50;i++) { var s=new SimulationState(); s.Reset(Substance.H2O); Check(s.Hydrate(i*.1f/5f),"No reaction at "+i); s.AddIndicator(); Check(s.IndicatorPink,"Indicator failed at "+i); } }}
        };
    }
}
