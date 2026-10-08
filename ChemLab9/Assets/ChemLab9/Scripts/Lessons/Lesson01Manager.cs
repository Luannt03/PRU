using UnityEngine;
using ChemLab9.Chemistry;
using ChemLab9.Equipment;
using ChemLab9.Interaction;
namespace ChemLab9.Lessons
{
    public class Lesson01Manager : LessonManager
    {
        public ChemicalContainer Water, Limewater;
        public GasDeliveryController Gas;
        public Renderer Litmus;
        public DraggableObject LitmusTool;
        bool checkedAcid;
        public override bool Ready => checkedAcid && Limewater.State.GasDose >= .9999f && !Gas.IsOn && (LitmusTool == null || !LitmusTool.Busy);
        public override string Status => "Nước nhận CO2: " + Mathf.RoundToInt(Water.State.GasDose * 100) + "%\nNước vôi nhận CO2: " + Mathf.RoundToInt(Limewater.State.GasDose * 100) + "%\nQuỳ sau CO2: " + (checkedAcid ? "đỏ" : "chưa kiểm tra đủ") + "\nCO2 không màu; bọt khí chỉ minh họa. Nước không đổi thành màu đỏ.";
        public override bool ApplyDrop(DraggableObject item, DropZone zone)
        {
            if (!Started) return false;
            if (LitmusTool != null && LitmusTool.Busy) { Feedback = "Đợi quỳ được nhúng và đưa ra trước bàn để quan sát."; return false; }
            ChemicalContainer target = zone.ZoneId == "water" ? Water : Limewater;
            if (item.Kind == ToolKind.GasTube)
            {
                if (!Gas.Connect(target)) return false;
                item.Snap(zone); Feedback = "Đầu ống đã vào cốc. Click nguồn CO2 để bật/tắt."; return true;
            }
            if (item.Kind == ToolKind.Litmus && target == Water)
            {
                if (Gas.IsOn) { Feedback = "Dừng dẫn khí trước khi kiểm tra quỳ."; return false; }
                Feedback = "Đang nhúng quỳ vào nước rồi đưa ra giữa để quan sát.";
                item.AnimateDipAndPresent(zone.SnapPoint, new Vector3(0, 1.23f, -.62f), () =>
                {
                    bool acid = Water.State.IsAcidic && Water.State.GasDose >= .9999f;
                    Litmus.sharedMaterial.color = IndicatorController.LitmusColor(Water.State, Water.State.GasDose >= .9999f);
                    checkedAcid = acid;
                    Feedback = acid ? "Quỳ hóa đỏ: CO2 + H2O ⇌ H2CO3. Quan sát giấy quỳ phía trước, giữa bàn." : "Quỳ vẫn tím. Hãy dẫn đủ CO2 vào nước trước rồi kiểm tra lại.";
                });
                return true;
            }
            return false;
        }
        public override void StopEquipment()
        {
            Gas.StopGas();
            if (LitmusTool != null && LitmusTool.Busy) LitmusTool.ResetObject();
        }
        public override void ResetLesson() { base.ResetLesson(); Gas.Target = null; Water.ResetContents(); Limewater.ResetContents(); checkedAcid = false; Litmus.sharedMaterial.color = new Color(.6f,.2f,.8f); }
    }
}
