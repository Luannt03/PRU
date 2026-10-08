using UnityEngine;
namespace ChemLab9.Chemistry
{
    public class ChemicalContainer : MonoBehaviour
    {
        public SimulationState State { get; } = new SimulationState();
        public Renderer Liquid, Solid;
        public Substance[] Initial;
        public void Configure(Renderer liquid, Renderer solid, params Substance[] initial)
        { Liquid = liquid; Solid = solid; Initial = initial; ResetContents(); }
        public void ResetContents() { State.Reset(Initial); Refresh(); }
        public void Refresh()
        {
            if (Liquid != null)
            {
                bool hasLiquid = State.Amount(Substance.H2O) + State.Amount(Substance.H2CO3) + State.Amount(Substance.CaOH2) > 0;
                Liquid.gameObject.SetActive(hasLiquid);
                // Acid is colorless: only the paper changes colour in lesson 1.
                Color c = new Color(.65f, .85f, 1f, .38f);
                if (State.Amount(Substance.CaCO3) > 0) c = Color.Lerp(c, new Color(.96f,.96f,.96f,.78f), Mathf.Clamp01(State.Amount(Substance.CaCO3)));
                if (State.LastReaction == ReactionKind.Hydration) c = new Color(.9f,.92f,.92f,.65f);
                if (State.IndicatorPink) c = new Color(1f,.12f,.55f,.7f);
                Liquid.sharedMaterial.color = c;
                float fill = Mathf.Clamp01(State.Total / State.Capacity);
                Vector3 scale = Liquid.transform.localScale; scale.y = .04f + .045f * fill; Liquid.transform.localScale = scale;
            }
            if (Solid != null)
            {
                bool visible = State.Amount(Substance.CaCO3) + State.Amount(Substance.CuOH2) + State.Amount(Substance.CuO) > 0;
                Solid.gameObject.SetActive(visible);
                Color color = State.Amount(Substance.CuO) > 0 ? new Color(.06f,.07f,.08f) : State.Amount(Substance.CuOH2) > 0 ? new Color(0,.55f,.9f) : Color.white;
                Solid.sharedMaterial.color = color;
                if (State.Amount(Substance.CaCO3) > 0)
                {
                    Vector3 scale = Solid.transform.localScale; scale.y = .025f * Mathf.Clamp01(State.Amount(Substance.CaCO3)); Solid.transform.localScale = scale;
                }
            }
        }
    }
}
