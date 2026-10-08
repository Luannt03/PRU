using UnityEngine;
using ChemLab9.Chemistry;
namespace ChemLab9.Equipment
{
    public class GasDeliveryController : MonoBehaviour
    {
        public float DosePerSecond = .35f;
        public ChemicalContainer Target;
        public bool IsOn { get; private set; }
        public ParticleSystem Bubbles;
        public Core.StationController Station;
        public void Toggle()
        {
            if (Target == null) { Station.Lesson.Feedback = "Đặt đầu ống vào cốc trước khi bật nguồn CO2."; return; }
            if (!IsOn && Target.State.GasDose >= 1f) { Station.Lesson.Feedback = "Cốc đã nhận đủ một liều CO2; MVP không dẫn dư."; return; }
            IsOn = !IsOn;
            if (!IsOn) StopGas();
        }
        public bool Connect(ChemicalContainer target)
        {
            if (IsOn) { Station.Lesson.Feedback = "Tắt nguồn khí trước khi đổi cốc."; return false; }
            Target = target; return true;
        }
        public void StopGas() { IsOn = false; if (Bubbles != null) Bubbles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); }
        void Update()
        {
            if (!IsOn || !Station.Active || Time.timeScale == 0) return;
            if (Target == null) { StopGas(); return; }
            ReactionKind reaction = Target.State.DeliverGas(Time.deltaTime * DosePerSecond, true);
            if (reaction == ReactionKind.None) { StopGas(); return; }
            if (Bubbles != null) { Bubbles.transform.position = Target.transform.position + Vector3.up * .08f; if (!Bubbles.isPlaying) Bubbles.Play(); }
            Target.Refresh();
            if (Target.State.GasDose >= .9999f) { Station.Lesson.Feedback = "Đã đủ liều CO2. Nguồn khí tự dừng."; StopGas(); }
        }
    }
}
