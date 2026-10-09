using UnityEngine;
using ChemLab9.Chemistry;
using ChemLab9.Interaction;
namespace ChemLab9.Lessons
{
    public class Lesson02Manager : LessonManager
    {
        public ChemicalContainer Beaker;
        public GameObject CaOSample;
        public DraggableObject CaOSpoon, IndicatorTool;
        public ParticleSystem Steam;
        public Transform ThermometerFill;
        public TMPro.TMP_Text ThermometerText;
        public float InitialTemperature = 25f, DemonstrationRise = 5f;
        [Min(.1f)] public float ReactionSeconds = 3f;
        public bool ShowSteam = true;
        public float SelectedMassGrams { get; private set; } = 1f;
        public bool MassInputValid { get; private set; } = true;
        public bool CanChangeMass => !hydrated && !adding && (CaOSpoon == null || !CaOSpoon.Busy) && (IndicatorTool == null || !IndicatorTool.Busy);
        public bool ReactionComplete => hydrated && elapsed >= Mathf.Max(.1f, ReactionSeconds);
        public HydrationYield Yield { get; private set; }
        public float Progress => hydrated ? Mathf.Clamp01(elapsed / Mathf.Max(.1f, ReactionSeconds)) : 0;
        public float Temperature => InitialTemperature + Progress * DemonstrationRise * (float)Yield.CaOReacted;
        public override bool Ready => ReactionComplete && Beaker.State.IndicatorPink && !adding
            && (CaOSpoon == null || !CaOSpoon.Busy) && (IndicatorTool == null || !IndicatorTool.Busy);
        public override string Status => "CaO đã chọn: " + SelectedMassGrams.ToString("F2") + " g\n"
            + "Phản ứng: " + Mathf.RoundToInt(Progress * 100) + "%\n"
            + "Nhiệt độ minh họa: " + Temperature.ToString("F1") + " °C\n"
            + "Chỉ thị: " + (Beaker.State.IndicatorPink ? "hồng" : "không màu")
            + "\nCa(OH)2 ít tan; hỗn hợp có thể đục. Hơi nước chỉ minh họa.";
        public string FinalQuantityResult => "CaO + H2O -> Ca(OH)2 (tỏa nhiệt)\n"
            + "CaO phản ứng: " + Yield.CaOReacted.ToString("F3") + " g\n"
            + "H2O tiêu thụ: " + Yield.WaterConsumed.ToString("F3") + " g\n"
            + "Ca(OH)2 tạo thành: " + Yield.CaOH2Produced.ToString("F3") + " g\n"
            + "Lý thuyết; gồm phần tan và không tan.\nKhông có khí sản phẩm.";
        Vector3 sampleScale;
        float elapsed;
        bool hydrated, adding;
        void Start() { sampleScale = CaOSample.transform.localScale; RefreshSample(); }
        void RefreshSample()
        {
            if (CaOSample == null || sampleScale == Vector3.zero) return;
            CaOSample.transform.localScale = sampleScale * Mathf.Pow(SelectedMassGrams, 1f / 3f);
        }
        public bool SetMassFromText(string text)
        {
            if (!CanChangeMass) return false;
            MassInputValid = QuantityCalculations.TryReadCaOMass(text, out float mass);
            if (MassInputValid) { SelectedMassGrams = mass; RefreshSample(); }
            return MassInputValid;
        }
        public bool SetMassFromSlider(float value)
        {
            if (!CanChangeMass || float.IsNaN(value) || float.IsInfinity(value)) return false;
            SelectedMassGrams = Mathf.Clamp(Mathf.Round(value * 100) / 100, QuantityCalculations.MinCaOGrams, QuantityCalculations.MaxCaOGrams);
            MassInputValid = true; RefreshSample(); return true;
        }
        public override bool ApplyDrop(DraggableObject item, DropZone zone)
        {
            if (!Started || adding || (CaOSpoon != null && CaOSpoon.Busy) || (IndicatorTool != null && IndicatorTool.Busy)) return false;
            if (item.Kind == ToolKind.CaOSpoon)
            {
                if (!MassInputValid) { Feedback = "Nhập khối lượng CaO từ 0,10 đến 5,00 g trước khi đổ."; return false; }
                if (hydrated || Beaker.State.Amount(Substance.H2O) < 1f)
                { Feedback = "Mẫu đã dùng hoặc thiếu nước. Nhấn Thử lại để lấy mẫu mới."; return false; }
                // Capture the selected dose before the pour. Do not read UI again in the callback.
                HydrationYield batch = QuantityCalculations.Hydrate(SelectedMassGrams, QuantityCalculations.WaterGrams);
                adding = true; Feedback = "Đưa lòng thìa trên miệng cốc rồi nghiêng để đổ mẫu CaO.";
                item.AnimatePour(zone.SnapPoint, () =>
                {
                    hydrated = Beaker.State.Hydrate((float)batch.CaOReacted / QuantityCalculations.MaxCaOGrams);
                    if (hydrated) { Yield = batch; CaOSample.SetActive(false); }
                    adding = false; Beaker.Refresh();
                    Feedback = hydrated ? "CaO đã vào cốc. Đợi phản ứng mô phỏng hoàn tất để xem bảng kết quả." : "Thiếu nước hoặc mẫu không hợp lệ. Nhấn Thử lại.";
                });
                return true;
            }
            if (item.Kind == ToolKind.IndicatorBottle)
            {
                if (hydrated && !ReactionComplete) { Feedback = "Đợi phản ứng kết thúc trước khi nhỏ chỉ thị."; return false; }
                adding = true; Feedback = "Mở nắp, đưa miệng lọ trên cốc và nghiêng để nhỏ chỉ thị.";
                item.AnimatePour(zone.SnapPoint, () =>
                {
                    Beaker.State.AddIndicator(); Beaker.Refresh(); adding = false;
                    Feedback = Beaker.State.IndicatorPink ? "Phenolphthalein hóa hồng trong môi trường bazơ." : "Nước + chỉ thị không hồng. Cần cho CaO tác dụng với nước.";
                });
                return true;
            }
            return false;
        }
        void Update()
        {
            if (hydrated && Station.Active && Time.timeScale > 0 && !ReactionComplete)
                elapsed = Mathf.Min(Mathf.Max(.1f, ReactionSeconds), elapsed + Time.deltaTime);
            if (ThermometerText != null) ThermometerText.text = Temperature.ToString("F1") + " °C\n(minh họa)";
            if (ThermometerFill != null)
            {
                Vector3 scale = ThermometerFill.localScale; scale.y = .08f + .32f * Mathf.Clamp01((Temperature - InitialTemperature) / 25f); ThermometerFill.localScale = scale;
                Vector3 position = ThermometerFill.localPosition; position.y = 1.1f + scale.y * .5f; ThermometerFill.localPosition = position;
            }
            bool steaming = hydrated && Station.Active && Time.timeScale > 0 && !ReactionComplete && ShowSteam;
            if (Steam != null)
            {
                if (steaming && !Steam.isPlaying) Steam.Play();
                else if (!steaming && Steam.isPlaying) Steam.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }
        public override void StopEquipment()
        {
            if (Steam != null) Steam.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            if (CaOSpoon != null && CaOSpoon.Busy) CaOSpoon.ResetObject();
            if (IndicatorTool != null && IndicatorTool.Busy) IndicatorTool.ResetObject();
            if (adding)
            {
                adding = false;
                if (!hydrated) { CaOSample.SetActive(true); RefreshSample(); }
                Feedback = "Đã hủy thao tác đổ chưa hoàn tất; chưa thêm chất vào cốc.";
            }
        }
        public override void ResetLesson()
        {
            base.ResetLesson(); Beaker.ResetContents(); elapsed = 0; hydrated = adding = false;
            Yield = default; MassInputValid = true; CaOSample.SetActive(true); RefreshSample();
            Feedback = "Mẫu mới: chọn khối lượng CaO rồi kéo thìa vào cốc. Giữ khối lượng đã chọn để dễ thử lại.";
        }
    }
}
