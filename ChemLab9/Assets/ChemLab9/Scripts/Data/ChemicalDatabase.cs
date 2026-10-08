using UnityEngine;
namespace ChemLab9.Data
{
    [CreateAssetMenu(menuName = "ChemLab9/Chemical Database")]
    public class ChemicalDatabase : ScriptableObject
    {
        public ElementRecord[] elements;
        public ReactionData[] reactions;
        public SubstanceData[] substances;
        public LessonData[] lessons;
        public ElementRecord Element(string symbol) => System.Array.Find(elements, e => e.symbol == symbol);
        public LessonData Lesson(int id) => System.Array.Find(lessons, e => e.lessonId == id);
        public ReactionData Reaction(Chemistry.ReactionKind kind) => System.Array.Find(reactions, r => r.kind == kind);
    }
}
