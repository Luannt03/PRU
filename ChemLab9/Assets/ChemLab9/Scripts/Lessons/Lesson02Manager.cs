using UnityEngine;
using ChemLab9.Chemistry;
using ChemLab9.Interaction;
namespace ChemLab9.Lessons
{
    public class Lesson02Manager : LessonManager
    {
        public ChemicalContainer Beaker;
        public GameObject CaOSample;
        public ParticleSystem Steam;
        public Transform ThermometerFill;
        public TMPro.TMP_Text ThermometerText;
        public float InitialTemperature = 25f, DemonstrationRise = 25f, ReactionSeconds = 3f;
        public bool ShowSteam = true;
        float elapsed;
        bool hydrated, adding;
        Vector3 sampleScale;
        void Start() { sampleScale = CaOSample.transform.localScale; }
        public float Temperature => InitialTemperature + (hydrated ? Mathf.Clamp01(elapsed / ReactionSeconds) * DemonstrationRise : 0);
        public override bool Ready => hydrated && elapsed >= ReactionSeconds && Beaker.State.IndicatorPink;
        public override string Status => "Nhiệt độ mô phỏng: " + Temperature.ToString("F1") + " °C\nCaO + H2O → Ca(OH)2 (tỏa nhiệt)\nChỉ thị: " + (Beaker.State.IndicatorPink ? "hồng" : "không màu") + "\nCa(OH)2 ít tan. Hơi nước minh họa không phải khí sản phẩm.";
        public override bool ApplyDrop(DraggableObject item, DropZone zone)
        {
            if (!Started || adding) return false;
            if (item.Kind == ToolKind.CaOSpoon)
            {
                if (hydrated || Beaker.State.Amount(Substance.H2O) < 1f) { Feedback = "Mẫu đã dùng hoặc thiếu nước. Thử lại để lấy mẫu mới."; return false; }
                adding = true;
                item.AnimatePour(zone.SnapPoint, () => { hydrated = Beaker.State.Hydrate(1f); adding = false; Beaker.Refresh(); }); return true;
            }
            if (item.Kind == ToolKind.IndicatorBottle)
            {
                if (hydrated && elapsed < ReactionSeconds) { Feedback = "Đợi mô phỏng phản ứng kết thúc trước khi nhỏ chỉ thị."; return false; }
                adding = true; item.AnimatePour(zone.SnapPoint, () => { Beaker.State.AddIndicator(); Beaker.Refresh(); adding = false; Feedback = Beaker.State.IndicatorPink ? "Phenolphthalein hóa hồng trong môi trường bazơ." : "Nước + chỉ thị không hồng. Cần cho CaO tác dụng với nước."; }); return true;
            }
            return false;
        }
        void Update()
        {
            float progress = hydrated ? Mathf.Clamp01(elapsed / ReactionSeconds) : 0;
            if (ThermometerText != null) ThermometerText.text = Temperature.ToString("F1") + " °C\n(mô phỏng)";
            if (ThermometerFill != null)
            {
                Vector3 scale = ThermometerFill.localScale; scale.y = .08f + .32f * progress; ThermometerFill.localScale = scale;
                Vector3 position = ThermometerFill.localPosition; position.y = 1.1f + scale.y * .5f; ThermometerFill.localPosition = position;
            }
            if (!hydrated || !Station.Active || Time.timeScale == 0 || elapsed >= ReactionSeconds) return;
            elapsed = Mathf.Min(ReactionSeconds, elapsed + Time.deltaTime);
            CaOSample.transform.localScale = sampleScale * (1f - Mathf.Clamp01(elapsed / ReactionSeconds));
            if (elapsed >= ReactionSeconds) CaOSample.SetActive(false);
            if (ShowSteam && Steam != null && !Steam.isPlaying) Steam.Play();
            if (elapsed >= ReactionSeconds && Steam != null) Steam.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
        public override void StopEquipment() { if (Steam != null) Steam.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); }
        public override void ResetLesson() { base.ResetLesson(); Beaker.ResetContents(); elapsed = 0; hydrated = adding = false; CaOSample.transform.localScale = sampleScale; CaOSample.SetActive(true); }
    }
}
