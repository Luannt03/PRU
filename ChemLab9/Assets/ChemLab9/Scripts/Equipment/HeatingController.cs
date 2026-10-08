using UnityEngine;
using ChemLab9.Chemistry;
namespace ChemLab9.Equipment
{
    public class HeatingController : MonoBehaviour
    {
        [Min(.1f)] public float RequiredSeconds = 5f;
        public Core.StationController Station;
        public ChemicalContainer Sample;
        public bool InZone, IsOn;
        public GameObject HeatVisual;
        public ParticleSystem Steam;
        public float Progress => Sample == null ? 0 : Mathf.Clamp01(Sample.State.HeatSeconds / RequiredSeconds);
        public void Toggle()
        {
            if (!IsOn && !InZone) { Station.Lesson.Feedback = "Đặt ống nghiệm vào kẹp của vùng nung trước."; return; }
            if (!IsOn && Sample.State.Amount(Substance.CuOH2) <= 0) { Station.Lesson.Feedback = "Mẫu đã thành CuO. Thử lại để tạo mẫu mới."; return; }
            IsOn = !IsOn; if (HeatVisual != null) HeatVisual.SetActive(IsOn);
            if (!IsOn && Steam != null) Steam.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
        public void StopHeat() { IsOn = false; if (HeatVisual != null) HeatVisual.SetActive(false); if (Steam != null) Steam.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); }
        void Update()
        {
            if (!IsOn || !InZone || !Station.Active || Time.timeScale == 0) return;
            bool finished = Sample.State.Heat(Time.deltaTime, true, InZone, RequiredSeconds);
            Sample.Refresh();
            if (!finished && Sample.Solid != null) Sample.Solid.sharedMaterial.color = Color.Lerp(new Color(0,.55f,.9f), new Color(.06f,.07f,.08f), Progress);
            if (Steam != null && !Steam.isPlaying) Steam.Play();
            if (finished) { Station.Lesson.Feedback = "Đã tạo CuO màu đen. Click thiết bị để tắt nhiệt trước khi trả lời."; }
        }
    }
}
