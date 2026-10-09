using UnityEngine;
namespace SwimDemo
{
    public class SwimmerController : MonoBehaviour
    {
        public SwimKinematics State { get; } = new SwimKinematics();
        public SwimSettings Settings;
        public WaterSurface Water;
        public Transform VisualPivot;
        public ImportedSwimAnimator Animation;
        public float Speed { get; set; } = 1.8f;
        public void ResetSwimmer()
        {
            if (VisualPivot != null) VisualPivot.localRotation = Quaternion.identity;
            State.Reset(); ApplyPose(0); Animation?.ResetPose();
            foreach (ParticleSystem particles in GetComponentsInChildren<ParticleSystem>()) particles.Clear();
        }
        void Update()
        {
            if (Time.timeScale <= 0 || Time.deltaTime <= 0 || Settings == null) return;
            if (Input.GetKeyDown(KeyCode.M)) State.Automatic = !State.Automatic;
            if (Input.GetKeyDown(KeyCode.R)) { ResetSwimmer(); return; }
            float forward = Input.GetAxisRaw("Vertical"), turn = Input.GetAxisRaw("Horizontal");
            float rise = (Input.GetKey(KeyCode.Space) ? 1 : 0) - (Input.GetKey(KeyCode.C) ? 1 : 0);
            State.Step(Time.deltaTime, forward, turn, rise, Speed, Settings.VerticalSpeed);
            ApplyPose(rise);
            if (Animation != null) Animation.PlaybackSpeed = Settings.AnimationSpeed * (.7f + .3f * Mathf.Clamp01(State.LastSpeed / Mathf.Max(Speed, .1f)));
        }
        void ApplyPose(float rise)
        {
            float surface = Water == null ? 0 : Water.Height(State.X, State.Z);
            transform.SetPositionAndRotation(new Vector3(State.X, surface - State.Depth, State.Z), Quaternion.Euler(0, State.Yaw, 0));
            if (VisualPivot != null)
                VisualPivot.localRotation = Quaternion.Slerp(VisualPivot.localRotation, Quaternion.Euler(-rise * 12f, 0, 0), Mathf.Clamp01(Time.deltaTime * 5f));
        }
    }
}
