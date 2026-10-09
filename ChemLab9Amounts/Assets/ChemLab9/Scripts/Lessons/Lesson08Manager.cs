using ChemLab9.Chemistry;
using ChemLab9.Equipment;
using ChemLab9.Interaction;
namespace ChemLab9.Lessons
{
    public class Lesson08Manager : LessonManager
    {
        public ChemicalContainer Sample;
        public HeatingController Heater;
        public override bool Ready => Sample.State.Amount(Substance.CuO) > 0 && !Heater.IsOn;
        public override string Status => "Gia nhiệt: " + (Heater.Progress * 100).ToString("F0") + "% (thời gian gameplay)\nThiết bị: " + (Heater.IsOn ? "đang bật" : "đã tắt") + "\nCu(OH)2 —nhiệt→ CuO + H2O\nTắt sớm: tạm dừng tiến trình. CuO không tự đổi lại thành Cu(OH)2.";
        public override bool ApplyDrop(DraggableObject item, DropZone zone)
        {
            if (!Started || item.Kind != ToolKind.CopperSample || Heater.IsOn) { Feedback = "Tắt thiết bị trước khi di chuyển mẫu."; return false; }
            item.Snap(zone); Heater.InZone = true; Feedback = "Ống đã cố định trong kẹp. Click thiết bị gia nhiệt."; return true;
        }
        public override void StopEquipment() => Heater.StopHeat();
        public override void ResetLesson() { base.ResetLesson(); Heater.InZone = false; Sample.ResetContents(); }
    }
}
