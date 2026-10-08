using System;
using System.Collections.Generic;
using ChemLab9.Chemistry;
using ChemLab9.Core;
using ChemLab9.Data;
namespace ChemLab9.Tests
{
    // Shared by Unity EditMode tests and the standalone .NET runner, using real production code.
    public static class RegressionScenarios
    {
        static void Check(bool value, string message) { if (!value) throw new Exception(message); }
        static void Near(float actual, float expected) => Check(Math.Abs(actual-expected)<.001f,$"Expected {expected}, got {actual}");
        static SimulationState Water() { var s=new SimulationState(); s.Reset(Substance.H2O); return s; }
        public static readonly Dictionary<string,Action> Cases = new Dictionary<string,Action> {
            {"WaterIsNeutralBeforeGas",()=>{var s=Water();Check(!s.IsAcidic && !s.IsBasic,"Water incorrectly acidic/basic");}},
            {"GasRequiresConnectedTube",()=>{var s=Water();Check(s.DeliverGas(1,false)==ReactionKind.None,"Unconnected gas reacted");Near(s.GasDose,0);Near(s.Amount(Substance.H2O),8);}},
            {"GasRequiresCorrectSource",()=>{var s=Water();Check(s.DeliverGas(1,true,"O2")==ReactionKind.None,"Wrong gas reacted");}},
            {"CarbonicAcidRequiresWater",()=>{var s=new SimulationState();Check(s.DeliverGas(1,true)==ReactionKind.None,"Empty beaker reacted");}},
            {"CO2CreatesWeakAcid",()=>{var s=Water();Check(s.DeliverGas(1,true)==ReactionKind.CarbonicAcid,"Missing carbonic acid");Check(s.IsAcidic,"Acid state missing");Near(s.Amount(Substance.H2O),7);Near(s.Amount(Substance.H2CO3),1);}},
            {"GasStopsAtSingleDose",()=>{var s=Water();s.DeliverGas(3,true);Near(s.GasDose,1);Check(s.DeliverGas(1,true)==ReactionKind.None,"Excess gas accepted");Near(s.Amount(Substance.H2CO3),1);}},
            {"LimewaterPrecipitates",()=>{var s=new SimulationState();s.Reset(Substance.CaOH2);Check(s.DeliverGas(1,true)==ReactionKind.Limewater,"Missing limewater reaction");Near(s.Amount(Substance.CaCO3),1);Near(s.Amount(Substance.CaOH2),0);Near(s.Amount(Substance.H2O),1);}},
            {"FractionalGasDoseIsBounded",()=>{var s=new SimulationState();s.Reset(Substance.CaOH2);for(int i=0;i<100;i++)s.DeliverGas(.03f,true);Near(s.GasDose,1);Near(s.Amount(Substance.CaCO3),1);Check(s.Total<=s.Capacity,"Overflow");}},
            {"WrongMaterialCannotHydrate",()=>{var s=Water();Check(!s.Hydrate(1,"CuO"),"Wrong oxide reacted");}},
            {"CaORequiresWater",()=>{var s=new SimulationState();Check(!s.Hydrate(1),"Dry CaO reacted");}},
            {"CaOFormsBasicMedium",()=>{var s=Water();Check(s.Hydrate(1),"Hydration failed");Check(s.IsBasic,"No basic medium");Near(s.Amount(Substance.CaOH2),1);Near(s.Amount(Substance.H2O),7);}},
            {"CaODoseCannotRepeat",()=>{var s=Water();s.Hydrate(1);Check(!s.Hydrate(1),"Duplicate dose accepted");Near(s.Amount(Substance.CaOH2),1);}},
            {"IndicatorAloneNotPink",()=>{var s=Water();s.AddIndicator();Check(!s.IndicatorPink,"Water indicator pink");}},
            {"IndicatorPinkAfterHydration",()=>{var s=Water();s.AddIndicator();s.Hydrate(1);Check(s.IndicatorPink,"Basic indicator not pink");}},
            {"CuOH2RequiresHeatAndZone",()=>{var s=new SimulationState();s.Reset(Substance.CuOH2);Check(!s.Heat(10,false,true,5),"Off heater reacted");Check(!s.Heat(10,true,false,5),"Wrong zone reacted");Near(s.HeatSeconds,0);}},
            {"OtherSolidDoesNotDecompose",()=>{var s=new SimulationState();s.Reset(Substance.CaO);Check(!s.Heat(10,true,true,5),"Wrong sample decomposed");}},
            {"EarlyHeatPausesProgress",()=>{var s=new SimulationState();s.Reset(Substance.CuOH2);s.Heat(2,true,true,5);s.Heat(10,false,true,5);Near(s.HeatSeconds,2);Near(s.Amount(Substance.CuO),0);}},
            {"HeatResumesAndProducesCuO",()=>{var s=new SimulationState();s.Reset(Substance.CuOH2);s.Heat(2,true,true,5);Check(s.Heat(3,true,true,5),"Did not finish");Near(s.Amount(Substance.CuO),1);Near(s.Amount(Substance.H2O),1);Near(s.Amount(Substance.CuOH2),0);}},
            {"CuODoesNotReverseOrDuplicate",()=>{var s=new SimulationState();s.Reset(Substance.CuOH2);s.Heat(5,true,true,5);s.Heat(10,false,true,5);s.Heat(10,true,true,5);Near(s.Amount(Substance.CuO),1);Near(s.Amount(Substance.CuOH2),0);}},
            {"ResetMakesFreshSample",()=>{var s=new SimulationState();s.Reset(Substance.CuOH2);s.Heat(5,true,true,5);s.AddIndicator();s.Reset(Substance.CuOH2);Near(s.Amount(Substance.CuOH2),1);Near(s.Amount(Substance.CuO),0);Near(s.HeatSeconds,0);Check(!s.IndicatorAdded,"Indicator leaked across reset");}},
            {"AmountRejectsNegativeNaNAndOverflow",()=>{var s=new SimulationState(2);Check(!s.Add(Substance.H2O,-1),"Negative allowed");Check(!s.Add(Substance.H2O,float.NaN),"NaN allowed");Check(!s.Add(Substance.H2O,float.PositiveInfinity),"Infinity allowed");s.Add(Substance.H2O,2);Check(!s.Add(Substance.CO2,1),"Overflow allowed");Near(s.Total,2);}},
            {"GasRejectsNaNAndNegative",()=>{var s=Water();Check(s.DeliverGas(float.NaN,true)==ReactionKind.None,"NaN reacted");Check(s.DeliverGas(-1,true)==ReactionKind.None,"Negative reacted");Near(s.GasDose,0);}},
            {"HeatingPauseZeroDelta",()=>{var s=new SimulationState();s.Reset(Substance.CuOH2);s.Heat(0,true,true,5);Near(s.HeatSeconds,0);}},
            {"BeakersIndependent",()=>{var a=Water();var b=Water();a.DeliverGas(1,true);Check(a.IsAcidic&&!b.IsAcidic,"State leaked between beakers");}},
            {"ScoreAwardedOnce",()=>{var p=new ProgressLedger();Check(p.Complete(1),"First completion missing");Check(!p.Complete(1),"Duplicate awarded");Check(p.Score==100&&p.Count==1,"Wrong score");}},
            {"FiveLessonsIndependentAndClearable",()=>{var p=new ProgressLedger();foreach(int id in ProgressLedger.LessonIds)p.Complete(id);Check(p.Score==500&&p.Count==5,"Wrong five-table total");p.Clear();Check(p.Score==0&&p.Count==0,"Clear failed");}},
            {"TwentyElementsHaveCorrectShells",()=>{var es=ElementCatalog.FirstTwenty();Check(es.Length==20,"Missing elements");for(int i=0;i<20;i++)Check(es[i].Valid()&&es[i].atomicNumber==i+1,"Invalid element "+es[i].symbol);}},
            {"NaClCaGroupsAndShells",()=>{var es=ElementCatalog.FirstTwenty();var na=es[10];var cl=es[16];var ca=es[19];Check(na.group==1&&na.period==3&&na.shells[2]==1,"Na incorrect");Check(cl.group==17&&cl.period==3&&cl.shells[2]==7,"Cl incorrect");Check(ca.group==2&&ca.period==4&&ca.shells[3]==2,"Ca incorrect");}},
            {"RelativeRadiusExamplesAgree",()=>{Check(ElementCatalog.RelativeRadius("Li")<ElementCatalog.RelativeRadius("Na")&&ElementCatalog.RelativeRadius("Na")<ElementCatalog.RelativeRadius("K"),"Group trend incorrect");Check(ElementCatalog.RelativeRadius("Na")>ElementCatalog.RelativeRadius("Mg")&&ElementCatalog.RelativeRadius("Mg")>ElementCatalog.RelativeRadius("Al"),"Period trend incorrect");}},
            {"RadiusOrderRejectsReverseAndDuplicates",()=>{Check(ElementCatalog.IsRadiusOrder(new[]{"Li","Na","K"}),"Correct order failed");Check(!ElementCatalog.IsRadiusOrder(new[]{"K","Na","Li"}),"Reverse accepted");Check(!ElementCatalog.IsRadiusOrder(new[]{"Li","Li","K"}),"Duplicates accepted");}}
        };
    }
}
