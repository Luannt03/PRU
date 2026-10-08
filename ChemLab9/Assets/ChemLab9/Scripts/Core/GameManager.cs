using UnityEngine;
using ChemLab9.UI;
using ChemLab9.Interaction;
using ChemLab9.Data;
namespace ChemLab9.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public ChemicalDatabase Database { get; private set; }
        public LabUI UI { get; private set; }
        public PlayerController Player { get; private set; }
        public PlayerInteractor Interactor { get; private set; }
        public ProgressLedger Progress { get; } = new ProgressLedger();
        public StationController ActiveStation { get; private set; }
        public bool Paused { get; private set; }
        public bool IsMenu { get; private set; }
        public AudioFeedback Sounds;
        public static int RequestedLesson;
        public static readonly string ProgressKey = "ChemLab9.Completed.";
        Vector3 previousPosition;
        Quaternion previousRotation, previousCameraRotation;
        public void Configure(ChemicalDatabase database, bool menu)
        {
            Instance = this; Database = database; IsMenu = menu; Time.timeScale = 1; AudioListener.pause = false;
            foreach (int id in ProgressLedger.LessonIds) if (PlayerPrefs.GetInt(ProgressKey + id, 0) == 1) Progress.Complete(id);
        }
        public void Attach(LabUI ui, PlayerController player, PlayerInteractor interactor)
        {
            UI = ui; Player = player; Interactor = interactor;
            if (player != null) player.SetControl(true);
            else { Cursor.lockState = CursorLockMode.None; Cursor.visible = true; }
        }
        void Update()
        {
            if (IsMenu || !Input.GetKeyDown(KeyCode.Escape)) return;
            if (Paused) SetPause(false);
            else if (ActiveStation != null) ExitStation();
            else SetPause(true);
        }
        public void EnterStation(StationController station)
        {
            if (Paused || ActiveStation != null || Player == null) return;
            previousPosition = Player.transform.position; previousRotation = Player.transform.rotation;
            previousCameraRotation = Interactor.View.transform.localRotation;
            ActiveStation = station; Player.SetControl(false);
            var cc = Player.GetComponent<CharacterController>(); cc.enabled = false;
            Player.transform.position = station.transform.TransformPoint(new Vector3(0, 0.05f, -2.4f));
            Player.transform.rotation = station.transform.rotation; cc.enabled = true;
            Interactor.View.transform.localRotation = Quaternion.Euler(station.Lesson.Data.lessonId >= 30 ? -7f : 13f, 0, 0);
            UI.OpenStation(station);
        }
        public void ExitStation()
        {
            if (ActiveStation == null) return;
            Interactor.CancelDrag(); ActiveStation.StopEquipment();
            ActiveStation = null; UI.CloseStation();
            var cc = Player.GetComponent<CharacterController>(); cc.enabled = false;
            Player.transform.SetPositionAndRotation(previousPosition, previousRotation); cc.enabled = true;
            Interactor.View.transform.localRotation = previousCameraRotation;
            Player.SetControl(!Paused);
        }
        public void SetPause(bool paused)
        {
            Paused = paused; Time.timeScale = paused ? 0 : 1;
            AudioListener.pause = paused;
            if (Player != null) Player.SetControl(!paused && ActiveStation == null);
            UI.ShowPause(paused);
        }
        public void Complete(int lessonId)
        {
            if (!Progress.Complete(lessonId)) return;
            PlayerPrefs.SetInt(ProgressKey + lessonId, 1); PlayerPrefs.Save();
        }
        public void ClearProgress()
        {
            Progress.Clear();
            foreach (int id in ProgressLedger.LessonIds) PlayerPrefs.DeleteKey(ProgressKey + id);
            PlayerPrefs.Save();
        }
        void OnDestroy()
        {
            if (Instance != this) return;
            Instance = null; Time.timeScale = 1; AudioListener.pause = false; Cursor.lockState = CursorLockMode.None; Cursor.visible = true;
        }
    }
}
