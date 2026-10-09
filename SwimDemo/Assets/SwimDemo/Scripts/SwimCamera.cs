using UnityEngine;
namespace SwimDemo
{
    public class SwimCamera : MonoBehaviour
    {
        public Transform Target;
        public WaterSurface Water;
        public bool UnderwaterView { get; private set; }
        float yaw = 145, pitch = 22, distance = 5.8f;
        Camera view;
        public void ToggleView() { UnderwaterView = !UnderwaterView; pitch = UnderwaterView ? -12 : 22; }
        void Awake() { view = GetComponent<Camera>(); }
        void LateUpdate()
        {
            if (Target == null) return;
            if (Input.GetKeyDown(KeyCode.V)) ToggleView();
            if (Time.timeScale > 0)
            {
                if (Input.GetMouseButton(1)) { yaw += Input.GetAxis("Mouse X") * 3; pitch = Mathf.Clamp(pitch - Input.GetAxis("Mouse Y") * 2, -35, 65); }
                distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 3, 3, 11);
            }
            Vector3 target = Target.position;
            Vector3 position = target + Quaternion.Euler(pitch, yaw, 0) * new Vector3(0, 0, -distance);
            float surface = Water == null ? 0 : Water.Height(position.x, position.z);
            position.y = UnderwaterView ? Mathf.Min(position.y, surface - .4f) : Mathf.Max(position.y, surface + .4f);
            // Keep the observation camera inside the pool when viewing underwater.
            if (UnderwaterView) { position.x = Mathf.Clamp(position.x, -8.6f, 8.6f); position.z = Mathf.Clamp(position.z, -5.6f, 5.6f); position.y = Mathf.Max(position.y, -3.2f); }
            transform.SetPositionAndRotation(position, Quaternion.LookRotation(target - position, Vector3.up));
            RenderSettings.fog = true; RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogColor = UnderwaterView ? new Color(.025f, .28f, .36f) : new Color(.64f, .79f, .86f);
            RenderSettings.fogDensity = UnderwaterView ? .045f : .006f;
            if (view != null) view.backgroundColor = RenderSettings.fogColor;
        }
    }
}
