using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ChemLab9.Core;
using ChemLab9.Lessons;
namespace ChemLab9.UI
{
    public class LabUI : MonoBehaviour
    {
        TMP_FontAsset font;
        GameManager game;
        RectTransform canvas;
        GameObject stationPanel, pausePanel, resultPanel, optionsPanel, lessonPanel, crosshair;
        TMP_Text hud, prompt, stationTitle, instructions, status, feedback, resultText;
        Button startButton, quizButton;
        GameObject modeButtons;
        StationController station;
        RectTransform answerRoot;
        CanvasGroup stationGroup, resultGroup;
        public void Configure(GameManager manager, TMP_FontAsset fontAsset)
        {
            game = manager; font = fontAsset;
            canvas = GetComponent<RectTransform>();
            var c = gameObject.AddComponent<Canvas>(); c.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = gameObject.AddComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280,720); scaler.matchWidthOrHeight = .5f;
            gameObject.AddComponent<GraphicRaycaster>();
            if (game.IsMenu) BuildMenu(); else BuildGameplay();
            BuildOptions();
        }
        RectTransform Rect(string name, Transform parent, Vector2 size, Vector2 position, Vector2? anchor = null)
        {
            GameObject go = new GameObject(name, typeof(RectTransform)); go.transform.SetParent(parent,false);
            RectTransform r = go.GetComponent<RectTransform>(); Vector2 a = anchor ?? new Vector2(.5f,.5f);
            r.anchorMin = r.anchorMax = a; r.pivot = a; r.sizeDelta = size; r.anchoredPosition = position; return r;
        }
        TMP_Text Text(string name, Transform parent, string value, Vector2 size, Vector2 position, int fontSize = 18, Vector2? anchor = null)
        {
            var r = Rect(name,parent,size,position,anchor); var t = r.gameObject.AddComponent<TextMeshProUGUI>();
            t.font = font; t.text = value; t.fontSize = fontSize; t.color = new Color(.88f,.94f,1f); t.raycastTarget = false;
            t.alignment = TextAlignmentOptions.TopLeft; t.textWrappingMode = TextWrappingModes.Normal; return t;
        }
        GameObject Panel(string name, Vector2 size, Vector2 position, Vector2? anchor = null)
        {
            var r = Rect(name,canvas,size,position,anchor); var image = r.gameObject.AddComponent<Image>(); image.color = new Color(.035f,.065f,.10f,.97f); return r.gameObject;
        }
        Button Button(string name, Transform parent, string label, Vector2 size, Vector2 position, Action callback, Vector2? anchor = null)
        {
            var r = Rect(name,parent,size,position,anchor); var image = r.gameObject.AddComponent<Image>(); image.color = new Color(.13f,.32f,.39f);
            var b = r.gameObject.AddComponent<Button>(); b.targetGraphic = image; b.onClick.AddListener(() => { game.Sounds?.PlayClick(); callback(); });
            var text = Text("Label",r,label,size- new Vector2(12,8),Vector2.zero,17); text.alignment = TextAlignmentOptions.Center;
            return b;
        }
        void BuildMenu()
        {
            GameObject panel = Panel("MainMenuPanel",new Vector2(590,600),Vector2.zero);
            Text("Title",panel.transform,"CHEMLAB9",new Vector2(510,55),new Vector2(0,220),42).alignment = TextAlignmentOptions.Center;
            Text("Subtitle",panel.transform,"Một phòng lab • Năm bàn học\nHóa học lớp 9 • Demo offline",new Vector2(510,70),new Vector2(0,150),21).alignment = TextAlignmentOptions.Center;
            Button("Start",panel.transform,"Bắt đầu",new Vector2(350,52),new Vector2(0,50),() => SceneLoader.StartLab());
            Button("Lessons",panel.transform,"Bài học",new Vector2(350,52),new Vector2(0,-15),() => lessonPanel.SetActive(true));
            Button("Options",panel.transform,"Tùy chọn",new Vector2(350,52),new Vector2(0,-80),() => optionsPanel.SetActive(true));
            Button("Quit",panel.transform,"Thoát",new Vector2(350,52),new Vector2(0,-145),SceneLoader.Quit);
            Text("Progress",panel.transform,"Tiến độ: " + game.Progress.Count + "/5 • Điểm: " + game.Progress.Score,new Vector2(450,35),new Vector2(0,-230),18).alignment = TextAlignmentOptions.Center;
            lessonPanel = Panel("LessonSelection",new Vector2(650,600),Vector2.zero);
            Text("Heading",lessonPanel.transform,"Chọn bàn học trong cùng một phòng",new Vector2(590,55),new Vector2(0,240),25);
            for (int i=0;i<game.Database.lessons.Length;i++)
            {
                var data = game.Database.lessons[i]; int id = data.lessonId;
                Button("Lesson_"+id,lessonPanel.transform,data.title,new Vector2(590,55),new Vector2(0,150-i*67),() => SceneLoader.StartLab(id));
            }
            Button("Back",lessonPanel.transform,"Quay lại",new Vector2(200,45),new Vector2(0,-235),() => lessonPanel.SetActive(false)); lessonPanel.SetActive(false);
        }
        void BuildGameplay()
        {
            hud = Text("HUD",canvas,"",new Vector2(800,46),new Vector2(22,-18),20,new Vector2(0,1));
            prompt = Text("InteractionPrompt",canvas,"",new Vector2(850,55),new Vector2(22,20),18,new Vector2(0,0));
            crosshair = Text("Crosshair",canvas,"+",new Vector2(25,30),Vector2.zero,24).gameObject;
            crosshair.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Center;
            stationPanel = Panel("StationPanel",new Vector2(340,650),new Vector2(-12,-58),new Vector2(1,1));
            stationGroup = stationPanel.AddComponent<CanvasGroup>();
            stationTitle = Text("Title",stationPanel.transform,"",new Vector2(312,62),new Vector2(14,-12),23,new Vector2(0,1));
            // Scroll only the supporting text. Objects remain reachable in the left camera viewport.
            RectTransform scroll = Rect("TaskScroll",stationPanel.transform,new Vector2(314,380),new Vector2(14,-78),new Vector2(0,1));
            scroll.gameObject.AddComponent<Image>().color = new Color(0,0,0,.08f); scroll.gameObject.AddComponent<RectMask2D>();
            ScrollRect sr = scroll.gameObject.AddComponent<ScrollRect>(); sr.horizontal = false;
            RectTransform content = Rect("Content",scroll,new Vector2(300,0),Vector2.zero,new Vector2(0,1));
            VerticalLayoutGroup layout = content.gameObject.AddComponent<VerticalLayoutGroup>(); layout.spacing = 12; layout.childControlWidth = true; layout.childControlHeight = true; layout.childForceExpandHeight = false;
            content.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            instructions = Text("Instructions",content,"",new Vector2(300,0),Vector2.zero,17);
            status = Text("Status",content,"",new Vector2(300,0),Vector2.zero,17);
            feedback = Text("Feedback",content,"",new Vector2(300,0),Vector2.zero,17); feedback.color = new Color(.45f,1f,.75f);
            sr.content = content; sr.viewport = scroll;
            modeButtons = Rect("Modes",stationPanel.transform,new Vector2(310,35),new Vector2(0,-154)).gameObject;
            Button("RadiusMode",modeButtons.transform,"A: Bán kính",new Vector2(148,35),new Vector2(-80,0),() => ((Lesson31Manager)station.Lesson).SetMode(0));
            Button("MetalMode",modeButtons.transform,"B: Tính chất",new Vector2(148,35),new Vector2(80,0),() => ((Lesson31Manager)station.Lesson).SetMode(1));
            startButton = Button("Begin",stationPanel.transform,"Bắt đầu",new Vector2(148,38),new Vector2(-80,-197),() => station.Lesson.Begin());
            Button("Retry",stationPanel.transform,"Thử lại",new Vector2(148,38),new Vector2(80,-197),() => { resultPanel.SetActive(false); station.ResetLesson(); });
            quizButton = Button("Questions",stationPanel.transform,"Kết quả / Câu hỏi",new Vector2(310,38),new Vector2(0,-242),OpenResult);
            Button("ExitStation",stationPanel.transform,"Rời bàn (Esc)",new Vector2(148,35),new Vector2(-80,-286),game.ExitStation);
            Button("Pause",stationPanel.transform,"Tạm dừng",new Vector2(148,35),new Vector2(80,-286),() => game.SetPause(true));
            stationPanel.SetActive(false);
            BuildResult();
            pausePanel = Panel("PauseMenu",new Vector2(530,420),Vector2.zero);
            Text("Title",pausePanel.transform,"Tạm dừng",new Vector2(440,50),new Vector2(0,140),30);
            Button("Resume",pausePanel.transform,"Tiếp tục",new Vector2(350,48),new Vector2(0,70),() => game.SetPause(false));
            Button("Options",pausePanel.transform,"Tùy chọn",new Vector2(350,48),new Vector2(0,10),() => optionsPanel.SetActive(true));
            Button("Menu",pausePanel.transform,"Về menu",new Vector2(350,48),new Vector2(0,-50),SceneLoader.MainMenu);
            Button("Quit",pausePanel.transform,"Thoát",new Vector2(350,48),new Vector2(0,-110),SceneLoader.Quit); pausePanel.SetActive(false);
        }
        void BuildResult()
        {
            resultPanel = Panel("ResultPanel",new Vector2(770,620),new Vector2(-180,0));
            resultGroup = resultPanel.AddComponent<CanvasGroup>();
            resultText = Text("ResultText",resultPanel.transform,"",new Vector2(720,315),new Vector2(0,135),17);
            answerRoot = Rect("Answers",resultPanel.transform,new Vector2(720,220),new Vector2(0,-125));
            Button("Close",resultPanel.transform,"Đóng kết quả",new Vector2(240,42),new Vector2(0,-270),() => resultPanel.SetActive(false)); resultPanel.SetActive(false);
        }
        public void OpenResult()
        {
            if (station == null || !station.Lesson.Ready) return;
            RenderResult(); resultPanel.SetActive(true);
        }
        void RenderResult()
        {
            foreach (Transform child in answerRoot) { child.gameObject.SetActive(false); Destroy(child.gameObject); }
            LessonManager lesson = station.Lesson;
            string text = lesson.Data.title + "\n" + lesson.Feedback + "\n\n";
            if (lesson is Lesson01Manager acid) text += Chemistry.ReactionSystem.Describe(game.Database, acid.Water.State) + "\n" + Chemistry.ReactionSystem.Describe(game.Database, acid.Limewater.State) + "\n\n";
            else if (lesson is Lesson02Manager basic) text += Chemistry.ReactionSystem.Describe(game.Database, basic.Beaker.State) + "\n\n";
            else if (lesson is Lesson08Manager heat) text += Chemistry.ReactionSystem.Describe(game.Database, heat.Sample.State) + "\n\n";
            if (lesson.QuizFinished) resultText.text = text + "Bạn đã hoàn thành tất cả câu hỏi của bàn. Có thể thử lại hoặc khám phá bàn khác.";
            else
            {
                Data.QuizQuestion q = lesson.Data.questions[lesson.QuestionIndex]; resultText.text = text + "Câu " + (lesson.QuestionIndex+1) + "/" + lesson.Data.questions.Length + ": " + q.prompt;
                for (int i=0;i<q.answers.Length;i++) { int index = i; Button("Answer_"+i,answerRoot,q.answers[i],new Vector2(715,54),new Vector2(0,62-i*59),() => { lesson.Answer(index); RenderResult(); }); }
            }
        }
        void BuildOptions()
        {
            optionsPanel = Panel("OptionsPanel",new Vector2(620,520),Vector2.zero);
            Text("Title",optionsPanel.transform,"Tùy chọn",new Vector2(530,50),new Vector2(0,200),28);
            Text("SensitivityLabel",optionsPanel.transform,"Độ nhạy chuột",new Vector2(520,35),new Vector2(0,115),20);
            AddSlider("Sensitivity",optionsPanel.transform,new Vector2(0,70),.3f,5f,PlayerPrefs.GetFloat("ChemLab9.MouseSensitivity",2f),v => { PlayerPrefs.SetFloat("ChemLab9.MouseSensitivity",v); PlayerPrefs.Save(); });
            Text("AudioLabel",optionsPanel.transform,"Âm lượng (nếu có âm thanh)",new Vector2(520,35),new Vector2(0,5),20);
            AddSlider("Volume",optionsPanel.transform,new Vector2(0,-40),0,1,PlayerPrefs.GetFloat("ChemLab9.Volume",1),v => { AudioListener.volume = v; PlayerPrefs.SetFloat("ChemLab9.Volume",v); PlayerPrefs.Save(); });
            Button("ClearProgress",optionsPanel.transform,"Xóa tiến độ 5 bàn",new Vector2(360,45),new Vector2(0,-130),() => { game.ClearProgress(); });
            Button("Close",optionsPanel.transform,"Đóng",new Vector2(360,45),new Vector2(0,-195),() => optionsPanel.SetActive(false)); optionsPanel.SetActive(false);
        }
        void AddSlider(string name, Transform parent, Vector2 position, float min, float max, float value, Action<float> changed)
        {
            RectTransform r = Rect(name,parent,new Vector2(520,25),position); r.gameObject.AddComponent<Image>().color = new Color(.15f,.3f,.35f);
            Slider s = r.gameObject.AddComponent<Slider>(); s.minValue = min; s.maxValue = max;
            var handle = Rect("Handle",r,new Vector2(25,35),Vector2.zero); var image = handle.gameObject.AddComponent<Image>(); image.color = new Color(.4f,1f,.8f);
            s.handleRect = handle; s.targetGraphic = image; s.value = value; s.onValueChanged.AddListener(v => changed(v));
        }
        public void SetPrompt(string text) { if (prompt != null) prompt.text = text; }
        public void OpenStation(StationController selected)
        {
            station = selected; stationPanel.SetActive(true); crosshair.SetActive(false);
            modeButtons.SetActive(selected.Lesson is Lesson31Manager);
            game.Interactor.View.rect = new Rect(0,0,.72f,1);
        }
        public void CloseStation()
        {
            station = null; stationPanel.SetActive(false); resultPanel.SetActive(false); crosshair.SetActive(true);
            game.Interactor.View.rect = new Rect(0,0,1,1);
        }
        public void ShowPause(bool show)
        {
            pausePanel.SetActive(show);
            stationGroup.interactable = resultGroup.interactable = !show;
            stationGroup.blocksRaycasts = resultGroup.blocksRaycasts = !show;
            if (!show) optionsPanel.SetActive(false);
        }
        void LateUpdate()
        {
            if (hud != null) hud.text = "ChemLab9 • Hoàn thành " + game.Progress.Count + "/5 • Điểm " + game.Progress.Score;
            if (station == null) return;
            var lesson = station.Lesson; stationTitle.text = lesson.Data.title + (game.Progress.Contains(lesson.Data.lessonId) ? " ✓" : "");
            instructions.text = lesson.Data.objective + "\n\n" + lesson.Data.instructions;
            status.text = lesson.Status; feedback.text = lesson.Feedback;
            startButton.interactable = !lesson.Started; quizButton.interactable = lesson.Started && lesson.Ready;
        }
    }
}
