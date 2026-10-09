using ChemLab9.Data;
namespace ChemLab9.Chemistry
{
    public static class ReactionSystem
    {
        // The MVP supports these four explicit rules. Data supplies equations and explanations;
        // condition checks and state changes remain in the tested simulation rather than VFX.
        public static ReactionData Result(ChemicalDatabase database, SimulationState state)
        {
            return state.LastReaction == ReactionKind.None ? null : database.Reaction(state.LastReaction);
        }
        public static string Describe(ChemicalDatabase database, SimulationState state)
        {
            ReactionData data = Result(database,state);
            return data == null ? "Chưa xảy ra phản ứng phù hợp." : data.equation + "\n" + data.explanation;
        }
    }
}
