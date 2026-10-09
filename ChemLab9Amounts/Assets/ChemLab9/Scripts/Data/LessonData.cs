using UnityEngine;
namespace ChemLab9.Data
{
    [System.Serializable]
    public class QuizQuestion
    {
        public string prompt;
        public string[] answers;
        public int correctIndex;
        public string explanation;
    }
    [CreateAssetMenu(menuName = "ChemLab9/Lesson")]
    public class LessonData : ScriptableObject
    {
        public int lessonId;
        public string title;
        [TextArea] public string objective, instructions;
        public QuizQuestion[] questions;
    }
}
