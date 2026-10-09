using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using ChemLab9.Data;
using ChemLab9.Chemistry;
using ChemLab9.Equipment;
using ChemLab9.Interaction;
using ChemLab9.Lessons;
using ChemLab9.PeriodicTable;
using ChemLab9.UI;
namespace ChemLab9.Core
{
    // Scene roots and configuration are serialized. Apparatus and UI are assembled on Play,
    // so all required references are assigned in one repeatable place rather than by hand.
    public class ChemLabBootstrap : MonoBehaviour
    {
        public bool MainMenu;
        public ChemicalDatabase Database;
        public Font VietnameseFont;
        public TMP_FontAsset FontShaderReference;
        public AudioClip ClickSound, CorrectSound, IncorrectSound;
        public Material OpaqueMaterial, GlassMaterial;
        public GameObject TableModel, BeakerModel, TubeModel, HeaterModel, ShelfModel;
        public static LabFactory Factory { get; private set; }
        GameManager game;
        public void Build()
        {
            if (game != null) return;
            if (Database == null || VietnameseFont == null || OpaqueMaterial == null || GlassMaterial == null)
            { Debug.LogError("ChemLab9: thiếu Database/Font/Material trên Systems. Khôi phục scene từ bản project hoặc chạy ChemLab9/Validate Project.",this); enabled=false; return; }
            TMP_FontAsset font = TMP_FontAsset.CreateFontAsset(VietnameseFont, 48, 5, UnityEngine.TextCore.LowLevel.GlyphRenderMode.SDFAA, 1024, 1024, AtlasPopulationMode.Dynamic, true);
            if (FontShaderReference != null) font.material.shader = FontShaderReference.material.shader;
            Factory = new LabFactory(font,OpaqueMaterial,GlassMaterial);
            game = gameObject.AddComponent<GameManager>(); game.Configure(Database,MainMenu);
            game.Sounds = gameObject.AddComponent<AudioFeedback>(); game.Sounds.Configure(ClickSound, CorrectSound, IncorrectSound);
            if (EventSystem.current == null)
            {
                GameObject eventSystem = GameObject.Find("EventSystem") ?? new GameObject("EventSystem");
                if (eventSystem.GetComponent<EventSystem>() == null) eventSystem.AddComponent<EventSystem>();
                if (eventSystem.GetComponent<StandaloneInputModule>() == null) eventSystem.AddComponent<StandaloneInputModule>();
            }
            GameObject canvas = GameObject.Find("Canvas") ?? new GameObject("Canvas", typeof(RectTransform));
            LabUI ui = canvas.AddComponent<LabUI>(); ui.Configure(game,font);
            AudioListener.volume = PlayerPrefs.GetFloat("ChemLab9Amounts.Volume",1);
            if (MainMenu)
            {
                GameObject camera = new GameObject("MenuCamera",typeof(Camera),typeof(AudioListener)); camera.tag = "MainCamera";
                camera.GetComponent<Camera>().backgroundColor = new Color(.035f,.07f,.10f); camera.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
                game.Attach(ui,null,null); return;
            }
            BuildRoom();
            GameObject player = GameObject.Find("Player") ?? new GameObject("Player"); player.transform.position = new Vector3(0,.05f,0);
            CharacterController cc = player.AddComponent<CharacterController>(); cc.height = 1.8f; cc.radius = .3f; cc.center = new Vector3(0,.9f,0); cc.stepOffset=.25f;
            GameObject view = Factory.Empty("FirstPersonCamera",player.transform,new Vector3(0,1.6f,0)); view.tag="MainCamera";
            Camera cam=view.AddComponent<Camera>(); cam.fieldOfView=65; cam.nearClipPlane=.03f; cam.farClipPlane=60; cam.backgroundColor = new Color(.07f,.11f,.15f); view.AddComponent<AudioListener>();
            PlayerController controller=player.AddComponent<PlayerController>(); PlayerInteractor interactor=player.AddComponent<PlayerInteractor>(); interactor.View=cam;
            game.Attach(ui,controller,interactor);
            Transform stations = (GameObject.Find("Stations") ?? new GameObject("Stations")).transform;
            foreach (int id in ProgressLedger.LessonIds)
            {
                string name=StationName(id); Transform root=stations.Find(name);
                if (root == null) { root=Factory.Empty(name,stations,StationPosition(id)).transform; root.localRotation = Quaternion.Euler(0,id>=30?180:0,0); }
                BuildStation(root,id);
            }
            if (GameManager.RequestedLesson != 0)
            {
                int id=GameManager.RequestedLesson; Transform root=stations.Find(StationName(id));
                if (root!=null) { cc.enabled=false; player.transform.position=root.TransformPoint(new Vector3(0,.05f,-2.5f)); player.transform.rotation=root.rotation; cc.enabled=true; }
                GameManager.RequestedLesson=0;
            }
        }
        void Awake() => Build();
        public static string StationName(int id)
        {
            switch(id) { case 1:return "Station_Lesson01_AcidicOxide"; case 2:return "Station_Lesson02_BasicOxide"; case 8:return "Station_Lesson08_InsolubleBase"; case 30:return "Station_Lesson30_PeriodicTable"; default:return "Station_Lesson31_PeriodicTrends"; }
        }
        public static Vector3 StationPosition(int id)
        {
            switch(id) { case 1:return new Vector3(-4.2f,0,3.6f); case 2:return new Vector3(0,0,3.6f); case 8:return new Vector3(4.2f,0,3.6f); case 30:return new Vector3(-3f,0,-3.6f); default:return new Vector3(3f,0,-3.6f); }
        }
        void BuildRoom()
        {
            Transform root=(GameObject.Find("LabRoom") ?? new GameObject("LabRoom")).transform;
            Factory.Shape("Floor",root,new Vector3(0,-.1f,0),new Vector3(16,.2f,12),new Color(.23f,.31f,.34f),PrimitiveType.Cube,true);
            Factory.Shape("NorthWall",root,new Vector3(0,2f,6),new Vector3(16,4,.2f),new Color(.64f,.73f,.75f),PrimitiveType.Cube,true);
            Factory.Shape("SouthWall",root,new Vector3(0,2f,-6),new Vector3(16,4,.2f),new Color(.64f,.73f,.75f),PrimitiveType.Cube,true);
            Factory.Shape("EastWall",root,new Vector3(8,2,0),new Vector3(.2f,4,12),new Color(.55f,.68f,.7f),PrimitiveType.Cube,true);
            Factory.Shape("WestWall",root,new Vector3(-8,2,0),new Vector3(.2f,4,12),new Color(.55f,.68f,.7f),PrimitiveType.Cube,true);
            Factory.Shape("Ceiling",root,new Vector3(0,4,0),new Vector3(16,.1f,12),new Color(.75f,.8f,.82f));
            GameObject light=Factory.Empty("LabLight",root,Vector3.up*3.8f); light.transform.rotation=Quaternion.Euler(65,-30,0);
            var directional=light.AddComponent<Light>(); directional.type=LightType.Directional; directional.intensity=1.5f;
            RenderSettings.ambientLight=new Color(.62f,.68f,.75f);
            for(int i=0;i<3;i++)
            {
                GameObject lamp=Factory.Empty("CeilingLamp_"+i,root,new Vector3((i-1)*4,3.7f,0)); var l=lamp.AddComponent<Light>(); l.type=LightType.Point; l.range=8; l.intensity=2;
                Factory.Shape("LampCover_"+i,root,lamp.transform.localPosition+Vector3.up*.12f,new Vector3(1.4f,.07f,.6f),Color.white);
            }
            if (ShelfModel!=null)
            {
                GameObject shelf=Factory.Model("EquipmentShelf",ShelfModel,root,new Vector3(-7.2f,.1f,0),new Vector3(.8f,2.2f,2.5f)); shelf.transform.localRotation=Quaternion.Euler(0,90,0);
            }
        }
        void BuildStation(Transform root,int id)
        {
            StationController station=root.gameObject.AddComponent<StationController>();
            Factory.Model("ExperimentTable",TableModel,root,Vector3.zero,new Vector3(3.5f,1.1f,1.4f));
            var tableCollider=root.gameObject.AddComponent<BoxCollider>(); tableCollider.center=new Vector3(0,.55f,0); tableCollider.size=new Vector3(3.5f,1.1f,1.4f);
            GameObject nameplate=Factory.Shape("StationSign",root,new Vector3(0,.65f,-.72f),new Vector3(3,.45f,.035f),new Color(.04f,.12f,.18f),PrimitiveType.Cube,true);
            Factory.Label("LessonTitle",root,new Vector3(0,.68f,-.75f),Database.Lesson(id).title,.12f);
            LessonManager lesson;
            switch(id)
            {
                case 1: lesson=root.gameObject.AddComponent<Lesson01Manager>(); break;
                case 2: lesson=root.gameObject.AddComponent<Lesson02Manager>(); break;
                case 8: lesson=root.gameObject.AddComponent<Lesson08Manager>(); break;
                case 30: lesson=root.gameObject.AddComponent<Lesson30Manager>(); break;
                default: lesson=root.gameObject.AddComponent<Lesson31Manager>(); break;
            }
            station.Configure(lesson); lesson.Configure(station,Database.Lesson(id));
            if(id==1) BuildAcid(station,(Lesson01Manager)lesson);
            else if(id==2) BuildBasic(station,(Lesson02Manager)lesson);
            else if(id==8) BuildHeating(station,(Lesson08Manager)lesson);
            else if(id==30)
            {
                root.gameObject.AddComponent<PeriodicTableBuilder>().Build(station,(Lesson30Manager)lesson,Database);
                GameObject atom=Factory.Empty("AtomModel",root,new Vector3(0,2.05f,-.2f)); ((Lesson30Manager)lesson).Atom=atom.AddComponent<AtomVisualizer>();
            }
            else
            {
                PeriodicTrendVisualizer trends=root.gameObject.AddComponent<PeriodicTrendVisualizer>(); ((Lesson31Manager)lesson).Visualizer=trends; trends.Build(station,(Lesson31Manager)lesson,Database);
            }
        }
        ChemicalContainer Container(string name, StationController station, Vector3 position, bool tube, params Substance[] initial)
        {
            Vector3 size=tube?new Vector3(.12f,.4f,.12f):new Vector3(.3f,.35f,.3f);
            GameObject root=Factory.Model(name,tube?TubeModel:BeakerModel,station.transform,position,size,true);
            Renderer liquid=Factory.Shape("Liquid",root.transform,new Vector3(0,.13f,0),new Vector3(tube ? .08f:.23f,.065f,tube ? .08f:.23f),new Color(.65f,.85f,1f,.35f),PrimitiveType.Cylinder,false,true).GetComponent<Renderer>();
            Renderer solid=Factory.Shape("Precipitate",root.transform,new Vector3(0,.035f,0),new Vector3(tube ? .075f:.22f,.025f,tube ? .075f:.22f),Color.white,PrimitiveType.Cylinder).GetComponent<Renderer>();
            ChemicalContainer c=root.AddComponent<ChemicalContainer>(); c.Configure(liquid,solid,initial); return c;
        }
        DropZone Zone(string name,StationController station,Transform parent,Vector3 position,ToolKind[] accepted)
        {
            GameObject go=Factory.Empty("DropZone_"+name,parent,position); var c=go.AddComponent<BoxCollider>(); c.size=new Vector3(.42f,.42f,.42f); c.center=Vector3.up*.15f;
            DropZone zone=go.AddComponent<DropZone>();
            Renderer ring=Factory.Shape("Highlight",go.transform,new Vector3(0,.005f,0),new Vector3(.4f,.006f,.4f),new Color(.2f,.6f,.65f),PrimitiveType.Cylinder).GetComponent<Renderer>();
            zone.SnapPoint=Factory.Empty("SnapPoint",go.transform,new Vector3(0,.18f,0)).transform;
            zone.Configure(station,name,accepted,ring); return zone;
        }
        DraggableObject Tool(string name,StationController station,Vector3 position,ToolKind kind,string label,GameObject root=null,string caption=null)
        {
            if(root==null) root=Factory.Empty(name,station.transform,position);
            var box=root.AddComponent<BoxCollider>();
            box.size=kind==ToolKind.Litmus ? new Vector3(.14f,.24f,.08f) : new Vector3(.25f,.3f,.25f);
            box.center=Vector3.up*(kind==ToolKind.Litmus ? .05f : .1f);
            var tool=root.AddComponent<DraggableObject>(); tool.Configure(station,kind,label);
            Factory.EquipmentLabel(name+"Label",station.transform,position+new Vector3(0,.24f,-.06f),caption ?? label);
            return tool;
        }
        GameObject Button3D(string name,StationController station,Vector3 p,string label,System.Action action,GameObject prefab=null,string caption=null)
        {
            GameObject root=Factory.Model(name,prefab,station.transform,p,new Vector3(.35f,.32f,.35f));
            var col=root.AddComponent<BoxCollider>(); col.center=Vector3.up*.16f; col.size=new Vector3(.4f,.35f,.4f);
            var click=root.AddComponent<Interactable>(); click.Station=station; click.Label=label; click.Click=action;
            Factory.EquipmentLabel(name+"Label",station.transform,p+new Vector3(0,.47f,0),caption ?? label); return root;
        }
        ParticleSystem Particles(string name,Transform parent,Vector3 p,bool bubbles)
        {
            var go=Factory.Empty(name,parent,p); var ps=go.AddComponent<ParticleSystem>(); ps.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
            var main=ps.main; main.playOnAwake=false; main.startLifetime=bubbles ? .55f:1f; main.startSpeed=.25f; main.startSize=bubbles ? .025f:.09f; main.maxParticles=60;
            main.simulationSpace=ParticleSystemSimulationSpace.World; main.startColor=new Color(1f,1f,1f,.4f);
            var emission=ps.emission; emission.rateOverTime=bubbles?22:10;
            var shape=ps.shape; shape.shapeType=ParticleSystemShapeType.Cone; shape.angle=12; shape.radius=.045f;
            go.transform.localRotation=Quaternion.Euler(-90,0,0);
            var renderer=go.GetComponent<ParticleSystemRenderer>(); renderer.sharedMaterial=Factory.Material(new Color(1,1,1,.35f),true);
            ps.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear); return ps;
        }
        void BuildAcid(StationController station,Lesson01Manager lesson)
        {
            lesson.Water=Container("Beaker_Water",station,new Vector3(-.15f,1.1f,.08f),false,Substance.H2O);
            lesson.Limewater=Container("Beaker_Limewater",station,new Vector3(.9f,1.1f,.08f),false,Substance.CaOH2);
            Zone("water",station,lesson.Water.transform,Vector3.zero,new[]{ToolKind.GasTube,ToolKind.Litmus});
            Zone("lime",station,lesson.Limewater.transform,Vector3.zero,new[]{ToolKind.GasTube});
            Factory.EquipmentLabel("WaterLabel",station.transform,new Vector3(-.15f,1.56f,.08f),"Nước H2O");
            Factory.EquipmentLabel("LimeLabel",station.transform,new Vector3(.9f,1.56f,.08f),"Nước vôi trong");
            GasDeliveryController gas=station.gameObject.AddComponent<GasDeliveryController>(); gas.Station=station; gas.Bubbles=Particles("CO2Bubbles",station.transform,Vector3.zero,true); lesson.Gas=gas;
            GameObject source=Button3D("GasSource",station,new Vector3(-1.2f,1.1f,.05f),"Click: CO2 bật/tắt",gas.Toggle,caption:"Nguồn CO2");
            DraggableObject tip=Tool("GasTube",station,new Vector3(-.85f,1.18f,-.45f),ToolKind.GasTube,"Kéo đầu ống vào cốc",caption:"Đầu ống CO2");
            Factory.Shape("TubeTip",tip.transform,Vector3.up*.08f,new Vector3(.045f,.1f,.045f),new Color(.2f,.7f,.75f),PrimitiveType.Cylinder);
            station.gameObject.AddComponent<GasTubeVisual>().Configure(source.transform,tip.transform);
            DraggableObject paper=Tool("LitmusPaper",station,new Vector3(.38f,1.18f,-.45f),ToolKind.Litmus,"Kéo quỳ vào cốc nước",caption:"Giấy quỳ tím");
            lesson.Litmus=Factory.Shape("Paper",paper.transform,Vector3.up*.05f,new Vector3(.06f,.16f,.012f),new Color(.6f,.2f,.8f)).GetComponent<Renderer>();
            lesson.LitmusTool=paper; gas.CanOperate=() => !paper.Busy;
        }
        void BuildBasic(StationController station,Lesson02Manager lesson)
        {
            lesson.Beaker=Container("Beaker_Water",station,new Vector3(0,1.1f,.1f),false,Substance.H2O);
            Zone("water",station,lesson.Beaker.transform,Vector3.zero,new[]{ToolKind.CaOSpoon,ToolKind.IndicatorBottle});
            DraggableObject spoon=Tool("CaOSpoon",station,new Vector3(-1,1.18f,-.25f),ToolKind.CaOSpoon,"Kéo thìa CaO vào cốc",caption:"Thìa CaO");
            Factory.Shape("Handle",spoon.transform,new Vector3(0,.04f,-.1f),new Vector3(.035f,.025f,.3f),new Color(.6f,.7f,.75f));
            Factory.Shape("SpoonBowl",spoon.transform,new Vector3(0,.055f,.05f),new Vector3(.22f,.025f,.24f),new Color(.65f,.75f,.8f),PrimitiveType.Sphere);
            lesson.CaOSample=Factory.Shape("CaO_Dose",spoon.transform,new Vector3(0,.08f,.05f),new Vector3(.1f,.035f,.12f),Color.white,PrimitiveType.Sphere);
            lesson.CaOSpoon=spoon; spoon.ConfigurePour(new Vector3(.10f,.055f,.05f),sample:lesson.CaOSample);
            Factory.Model("CaO_Bottle",null,station.transform,new Vector3(-1.35f,1.1f,.2f),new Vector3(.22f,.25f,.22f));
            GameObject bottle=Factory.Model("ChemicalBottle",null,station.transform,new Vector3(1f,1.18f,-.2f),new Vector3(.19f,.25f,.19f),true);
            DraggableObject indicator=Tool("Phenolphthalein",station,bottle.transform.localPosition,ToolKind.IndicatorBottle,"Kéo phenolphthalein vào cốc",bottle,"Phenolphthalein");
            Factory.Shape("BottleNeck",bottle.transform,new Vector3(0,.255f,0),new Vector3(.07f,.035f,.07f),new Color(.7f,.9f,1f,.3f),PrimitiveType.Cylinder,false,true);
            GameObject cap=Factory.Shape("BottleCap",bottle.transform,new Vector3(0,.30f,0),new Vector3(.1f,.04f,.1f),new Color(.75f,.25f,.65f),PrimitiveType.Cylinder);
            lesson.IndicatorTool=indicator; indicator.ConfigurePour(new Vector3(0,.29f,0),cap:cap);
            lesson.Steam=Particles("WaterSteam_Illustration",station.transform,new Vector3(0,1.48f,.1f),false);
            Factory.Shape("VirtualThermometer",station.transform,new Vector3(.45f,1.32f,.3f),new Vector3(.075f,.44f,.05f),new Color(.7f,.8f,.85f));
            lesson.ThermometerFill=Factory.Shape("ThermometerFill",station.transform,new Vector3(.45f,1.14f,.265f),new Vector3(.04f,.08f,.03f),new Color(.9f,.3f,.15f)).transform;
            lesson.ThermometerText=Factory.EquipmentLabel("Temperature",station.transform,new Vector3(.45f,1.65f,.26f),"25.0 °C (mô phỏng)",.055f);
        }
        void BuildHeating(StationController station,Lesson08Manager lesson)
        {
            ChemicalContainer sample=Container("TestTube_CuOH2",station,new Vector3(-.42f,1.18f,-.15f),true,Substance.CuOH2); lesson.Sample=sample;
            Tool("TestTube",station,sample.transform.localPosition,ToolKind.CopperSample,"Kéo ống vào kẹp nung",sample.gameObject,"Ống Cu(OH)2");
            HeatingController heater=station.gameObject.AddComponent<HeatingController>(); heater.Sample=sample; heater.Station=station; lesson.Heater=heater;
            Button3D("Heater",station,new Vector3(0,1.1f,.06f),"Click: nhiệt bật/tắt",heater.Toggle,HeaterModel,"Đèn cồn bật/tắt");
            DropZone zone=Zone("heating",station,station.transform,new Vector3(0,1.455f,.06f),new[]{ToolKind.CopperSample});
            zone.SnapPoint.localPosition=Vector3.zero; zone.SnapPoint.localRotation=Quaternion.Euler(0,0,-22f);
            Factory.Shape("ClampSupport",station.transform,new Vector3(.32f,1.50f,.14f),new Vector3(.025f,.8f,.025f),new Color(.5f,.65f,.7f));
            Factory.Shape("Clamp",station.transform,new Vector3(.21f,1.72f,.06f),new Vector3(.28f,.03f,.045f),new Color(.5f,.65f,.7f));
            heater.HeatVisual=Factory.Shape("HeatZone",station.transform,new Vector3(0,1.425f,.06f),new Vector3(.065f,.07f,.065f),new Color(1f,.35f,.1f),PrimitiveType.Sphere); heater.HeatVisual.SetActive(false);
            Vector3 tubeMouth=zone.SnapPoint.localPosition+zone.SnapPoint.localRotation*(Vector3.up*.4f);
            heater.Steam=Particles("WaterVapor",zone.transform,tubeMouth,false);
        }
        void OnDestroy() { Factory?.Dispose(); Factory=null; }
    }
}
