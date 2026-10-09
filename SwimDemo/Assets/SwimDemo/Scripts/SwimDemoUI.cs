using UnityEngine;
namespace SwimDemo
{
    public class SwimDemoUI : MonoBehaviour
    {
        public Font Font;
        public SwimmerController Swimmer;
        public SwimCamera CameraControl;
        public string SetupError;
        bool paused;
        GUIStyle title, body, button;
        public void SetPause(bool value) { paused = value; Time.timeScale = value ? 0 : 1; }
        void Update() { if (Input.GetKeyDown(KeyCode.Escape)) SetPause(!paused); }
        void EnsureStyles()
        {
            if (title != null) return;
            title = new GUIStyle(GUI.skin.label) { font = Font, fontSize = 23, fontStyle = FontStyle.Bold, wordWrap = true };
            body = new GUIStyle(GUI.skin.label) { font = Font, fontSize = 16, wordWrap = true };
            button = new GUIStyle(GUI.skin.button) { font = Font, fontSize = 15, wordWrap = true };
            title.normal.textColor = body.normal.textColor = Color.white;
        }
        void OnGUI()
        {
            EnsureStyles();
            float scale = Mathf.Min(Screen.width / 1280f, Screen.height / 720f);
            Matrix4x4 previous = GUI.matrix; GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * Mathf.Max(scale, .1f));
            GUI.Box(new Rect(12, 12, 448, 194), "");
            GUI.Label(new Rect(24, 20, 425, 32), "SWIM DEMO — Y Bot", title);
            if (!string.IsNullOrEmpty(SetupError))
            {
                GUI.Label(new Rect(24, 62, 423, 138), SetupError, body); GUI.matrix = previous; return;
            }
            GUI.Label(new Rect(24, 60, 425, 85), "W / S: tiến / lùi     A / D: quay trái / phải\nSpace: bơi lên     C: lặn xuống\nGiữ chuột phải: xoay camera; con lăn: zoom\nV: góc nhìn trên / dưới nước     Esc: tạm dừng", body);
            GUI.Label(new Rect(24, 154, 425, 46), "Độ sâu: " + Swimmer.State.Depth.ToString("F2") + " m  |  " + (Swimmer.State.Automatic ? "Tự bơi" : "Điều khiển bằng phím") + "\nM: bật/tắt tự bơi     R: về vị trí ban đầu", body);
            if (GUI.Button(new Rect(16, 214, 136, 35), paused ? "Tiếp tục" : "Tạm dừng", button)) SetPause(!paused);
            if (GUI.Button(new Rect(160, 214, 144, 35), "Đổi góc nhìn", button)) CameraControl.ToggleView();
            if (GUI.Button(new Rect(312, 214, 144, 35), Swimmer.State.Automatic ? "Điều khiển tay" : "Tự bơi", button)) Swimmer.State.Automatic = !Swimmer.State.Automatic;
            GUI.Label(new Rect(20, 261, 220, 26), "Tốc độ bơi: " + Swimmer.Speed.ToString("F1") + " m/s", body);
            Swimmer.Speed = GUI.HorizontalSlider(new Rect(234, 273, 215, 18), Swimmer.Speed, .5f, 4f);
            if (GUI.Button(new Rect(16, 303, 136, 35), "Làm lại", button)) { SetPause(false); Swimmer.ResetSwimmer(); }
            if (GUI.Button(new Rect(160, 303, 144, 35), "Thoát", button)) Quit();
            if (paused)
            {
                float width = Screen.width / Mathf.Max(scale, .1f), height = Screen.height / Mathf.Max(scale, .1f);
                GUI.Box(new Rect(width * .5f - 180, height * .5f - 70, 360, 140), "");
                GUI.Label(new Rect(width * .5f - 145, height * .5f - 50, 300, 35), "Đã tạm dừng", title);
                if (GUI.Button(new Rect(width * .5f - 130, height * .5f, 260, 42), "Tiếp tục", button)) SetPause(false);
            }
            // Space is a swim control, not a shortcut for the last clicked GUI button.
            GUI.FocusControl(null);
            GUI.matrix = previous;
        }
        static void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        void OnDestroy() { Time.timeScale = 1; }
    }
}
