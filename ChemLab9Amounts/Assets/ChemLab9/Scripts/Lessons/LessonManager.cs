using UnityEngine;
using ChemLab9.Core;
using ChemLab9.Data;
using ChemLab9.Interaction;
namespace ChemLab9.Lessons
{
    public abstract class LessonManager : MonoBehaviour
    {
        public StationController Station { get; private set; }
        public LessonData Data { get; private set; }
        public bool Started { get; private set; }
        public string Feedback = "Nhấn Bắt đầu, rồi thao tác trên dụng cụ 3D.";
        public int QuestionIndex { get; private set; }
        public bool QuizFinished => QuestionIndex >= Data.questions.Length;
        public abstract bool Ready { get; }
        public abstract string Status { get; }
        public void Configure(StationController station, LessonData data) { Station = station; Data = data; }
        public void Begin() { Started = true; Feedback = "Hãy thao tác trên vật thể 3D theo hướng dẫn."; }
        public virtual bool ApplyDrop(DraggableObject item, DropZone zone) { Feedback = "Dụng cụ không phù hợp với vị trí này."; return false; }
        public virtual void StopEquipment() { }
        public virtual void ResetLesson() { StopEquipment(); Started = true; QuestionIndex = 0; Feedback = "Đã tạo mẫu mới. Tiến độ hoàn thành đã lưu vẫn được giữ."; }
        public bool Answer(int index)
        {
            if (GameManager.Instance.Paused || !Started || !Ready || QuizFinished) return false;
            QuizQuestion q = Data.questions[QuestionIndex];
            if (index != q.correctIndex) { GameManager.Instance.Sounds?.PlayAnswer(false); Feedback = "Chưa đúng. " + q.explanation; return false; }
            GameManager.Instance.Sounds?.PlayAnswer(true);
            Feedback = "Đúng. " + q.explanation; QuestionIndex++;
            if (QuizFinished) { GameManager.Instance.Complete(Data.lessonId); Feedback += "\nHoàn thành bàn! Điểm chỉ được cộng lần đầu."; }
            return true;
        }
    }
}
