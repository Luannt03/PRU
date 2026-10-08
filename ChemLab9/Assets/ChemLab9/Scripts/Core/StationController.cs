using UnityEngine;
using ChemLab9.Lessons;
using ChemLab9.Interaction;
namespace ChemLab9.Core
{
    public class StationController : MonoBehaviour
    {
        public LessonManager Lesson { get; private set; }
        public float WorkHeight = 1.18f;
        public string Prompt => "[E] " + (Lesson != null ? Lesson.Data.title : "Bàn học");
        public bool Active => GameManager.Instance != null && GameManager.Instance.ActiveStation == this;
        public void Configure(LessonManager lesson) => Lesson = lesson;
        public void Enter() => GameManager.Instance.EnterStation(this);
        public void StopEquipment() { if (Lesson != null) Lesson.StopEquipment(); }
        public void ResetLesson()
        {
            GameManager.Instance.Interactor.CancelDrag();
            foreach (DraggableObject item in GetComponentsInChildren<DraggableObject>()) item.ResetObject();
            Lesson.ResetLesson();
        }
    }
}
