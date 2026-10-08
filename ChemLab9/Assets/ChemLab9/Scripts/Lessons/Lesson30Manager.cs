using ChemLab9.Data;
using ChemLab9.PeriodicTable;
namespace ChemLab9.Lessons
{
    public class Lesson30Manager : LessonManager
    {
        public AtomVisualizer Atom;
        public ElementRecord Selected { get; private set; }
        public int TaskIndex { get; private set; }
        public override bool Ready => TaskIndex >= 3;
        public override string Status
        {
            get
            {
                string[] task = { "Click ô Na trên bảng 3D.", "Click nguyên tố có Z = 17.", "Click Ca và quan sát số lớp electron.", "Đã khám phá Na, Cl, Ca. Trả lời câu hỏi để hoàn thành." };
                return task[TaskIndex] + "\n" + (Selected == null ? "Chưa chọn nguyên tố." : ElementInfo(Selected));
            }
        }
        public static string ElementInfo(ElementRecord e) => e.name + " (" + e.symbol + ") • Z = " + e.atomicNumber + "\nĐiện tích hạt nhân: +" + e.atomicNumber + "e • p = e = " + e.atomicNumber + " (trung hòa)\nNhóm " + e.group + " / " + e.oldGroup + " • Chu kỳ " + e.period + "\nElectron theo lớp: " + string.Join(", ", e.shells) + "\nNguyên tử khối trung bình ≈ " + e.atomicMass + " u\nMô hình lớp electron minh họa, không phải quỹ đạo thực. Không hiển thị neutron.";
        public void Select(ElementRecord element)
        {
            if (!Started || !element.Valid()) return;
            Selected = element; Atom.Show(element);
            bool correct = TaskIndex == 0 && element.symbol == "Na" || TaskIndex == 1 && element.atomicNumber == 17 || TaskIndex == 2 && element.symbol == "Ca";
            if (correct) { TaskIndex++; Feedback = "Đã chọn đúng nguyên tố cho nhiệm vụ."; }
            else Feedback = "Bạn có thể khám phá mọi ô. Đọc nhiệm vụ để chọn nguyên tố cần tìm.";
        }
        public override void ResetLesson() { base.ResetLesson(); TaskIndex = 0; Selected = null; Atom.Clear(); }
    }
}
