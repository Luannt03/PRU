"""Regenerate authored MVP scenes/data from the documented seed; not required to open Unity.
Run only when intentionally rebuilding seed data. Original uploaded scenes/models are untouched.
"""
from pathlib import Path
import re, uuid, json, math
ROOT = Path(__file__).resolve().parents[1] / 'ChemLab9'
A = ROOT / 'Assets'
N = A / 'ChemLab9'
def guid(path):
    path = Path(path)
    meta = Path(str(path)+'.meta')
    if meta.exists(): return re.search(r'^guid: (\w+)', meta.read_text(),re.M).group(1)
    value = uuid.uuid5(uuid.NAMESPACE_URL,'ChemLab9/'+str(path.relative_to(ROOT))).hex
    if path.is_dir(): body='folderAsset: yes\nDefaultImporter:\n  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n'
    elif path.suffix == '.cs': body='MonoImporter:\n  externalObjects: {}\n  serializedVersion: 2\n  defaultReferences: []\n  executionOrder: 0\n  icon: {instanceID: 0}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n'
    elif path.suffix in ['.asset','.mat']: body='NativeFormatImporter:\n  externalObjects: {}\n  mainObjectFileID: '+('2100000' if path.suffix=='.mat' else '11400000')+'\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n'
    elif path.suffix=='.prefab': body='PrefabImporter:\n  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n'
    elif path.suffix=='.asmdef': body='AssemblyDefinitionImporter:\n  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n'
    else: body='DefaultImporter:\n  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n'
    meta.write_text('fileFormatVersion: 2\nguid: '+value+'\n'+body)
    return value
def q(s): return json.dumps(s,ensure_ascii=False)
def ints(values): return ''.join(int(v).to_bytes(4,'little',signed=True).hex() for v in values)
def ref(path, id=11400000, typ=2): return '{fileID: '+str(id)+', guid: '+guid(path)+', type: '+str(typ)+'}'
def asset(path,script,name,body):
    path.parent.mkdir(parents=True,exist_ok=True)
    path.write_text('%YAML 1.1\n%TAG !u! tag:unity3d.com,2011:\n--- !u!114 &11400000\nMonoBehaviour:\n  m_ObjectHideFlags: 0\n  m_CorrespondingSourceObject: {fileID: 0}\n  m_PrefabInstance: {fileID: 0}\n  m_PrefabAsset: {fileID: 0}\n  m_GameObject: {fileID: 0}\n  m_Enabled: 1\n  m_EditorHideFlags: 0\n  m_Script: '+ref(N/'Scripts/Data'/script,11500000,3)+'\n  m_Name: '+q(name)+'\n  m_EditorClassIdentifier: \n'+body)
    guid(path)
lessons = [
(1,'Bài 1: Oxit axit','Nhận biết CO2 là oxit axit qua quỳ tím và nước vôi trong.',
'1. Kéo đầu ống vào cốc nước.\n2. Click nguồn CO2, đợi đủ liều và nguồn dừng.\n3. Kéo quỳ vào nước: quỳ hóa đỏ.\n4. Kéo đầu ống vào nước vôi trong, bật khí: kết tủa trắng.\n5. Mở câu hỏi. Thả sai trả dụng cụ về vị trí cũ.',[
('Vì sao quỳ tím hóa đỏ sau khi dẫn CO2 vào nước?', ['CO2 hòa tan tạo môi trường axit yếu.', 'CO2 có màu đỏ.', 'Nước ban đầu là bazơ.'],0,'CO2 + H2O ⇌ H2CO3. Axit cacbonic yếu làm quỳ hóa đỏ; nước vẫn không màu.'),
('Vì sao nước vôi trong vẩn đục?', ['CO2 có màu trắng.', 'Tạo kết tủa CaCO3 trắng.', 'Tạo khí O2.'],1,'CO2 + Ca(OH)2 → CaCO3↓ + H2O. MVP giới hạn một liều CO2, không dẫn dư.')]),
(2,'Bài 2: Oxit bazơ','Cho CaO tác dụng với nước và kiểm tra môi trường bazơ.',
'1. Kéo thìa CaO vào cốc nước.\n2. Đợi mô phỏng nhiệt độ tăng.\n3. Kéo lọ phenolphthalein vào cốc.\n4. Quan sát màu hồng và trả lời câu hỏi.\nCó thể thử chỉ thị với nước trước để kiểm tra trường hợp không hồng.',[
('CaO + nước tạo chất gì và có hiện tượng nào?', ['CaCO3 và khí CO2.', 'CuO màu đen.', 'Ca(OH)2 và tỏa nhiệt.'],2,'CaO + H2O → Ca(OH)2, tỏa nhiệt. Ca(OH)2 ít tan, hỗn hợp có thể hơi đục; không phải mọi oxit bazơ đều phản ứng với nước.'),
('Phenolphthalein hồng vì sao? Hơi minh họa là gì?', ['Môi trường bazơ; hơi nước minh họa, không phải khí sản phẩm.', 'Nước luôn làm chỉ thị hồng; tạo O2.', 'CaO có màu hồng; tạo CO2.'],0,'Chỉ thị nhận biết môi trường bazơ. Nhiệt độ là tham số mô phỏng gameplay, không phải phép tính thực nghiệm.')]),
(8,'Bài 8: Bazơ không tan','Nung Cu(OH)2 màu xanh để thu CuO màu đen.',
'1. Kéo ống nghiệm xanh vào kẹp phía trên thiết bị.\n2. Click thiết bị để bật nhiệt.\n3. Đợi đủ thời gian; chất rắn chuyển đen.\n4. Click thiết bị để tắt.\n5. Mở câu hỏi. Tắt sớm sẽ tạm dừng thời gian tích lũy.',[
('Sau khi nung Cu(OH)2, chất rắn đen là gì?', ['CuO.', 'CO2.', 'CaCO3.'],0,'Cu(OH)2 —nhiệt→ CuO + H2O. Chỉ phần chất rắn đổi màu; thủy tinh không đổi đen.'),
('Điều gì xảy ra khi tắt nhiệt sau phản ứng?', ['CuO tự chuyển lại Cu(OH)2.', 'CuO vẫn tồn tại; Thử lại tạo mẫu mới.', 'Tạo khí O2.'],1,'Đây là ví dụ nhiệt phân Cu(OH)2. Không suy rộng mọi bazơ đều nhiệt phân như nhau; thời gian nung là tham số gameplay.')]),
(30,'Bài 30: Bảng tuần hoàn','Khám phá 20 nguyên tố đầu và mô hình lớp electron.',
'1. Nhấn Bắt đầu.\n2. Click Na.\n3. Click nguyên tố có Z = 17.\n4. Click Ca và quan sát mô hình.\n5. Mở câu hỏi.\nVị trí ô theo nhóm IUPAC và chu kỳ; tên/màu phân loại có chú giải.',[
('Nguyên tử Na trung hòa có bao nhiêu proton và electron?', ['11 proton, 10 electron.', '11 proton, 11 electron.', '12 proton, 11 electron.'],1,'Na có Z = 11, điện tích hạt nhân +11e; p = e = 11, electron theo lớp 2, 8, 1.'),
('Ca có bao nhiêu lớp electron?', ['2 lớp: 2, 18.', '3 lớp: 2, 8, 10.', '4 lớp: 2, 8, 8, 2.'],2,'Ca có Z = 20, thuộc chu kỳ 4, nhóm 2 / IIA. Mô hình lớp electron là minh họa học tập.'),
('Na và Cl cùng chu kỳ 3 có điểm chung nào?', ['Có 3 lớp electron; electron ngoài cùng lần lượt 1 và 7.', 'Cùng số proton.', 'Cùng nhóm IUPAC 1.'],0,'Na: 2,8,1; Cl: 2,8,7. Không suy ra neutron từ nguyên tử khối trung bình; cần đồng vị và số khối A.')]),
(31,'Bài 31: Xu hướng tuần hoàn','So sánh bán kính và xu hướng tính kim loại/phi kim.',
'1. Nhấn Bắt đầu.\n2. Ở chế độ A, click Li, Na, K theo bán kính tăng.\n3. Quan sát Na–Mg–Al trong chu kỳ 3.\n4. Chọn B: Tính chất để xem mũi tên và thanh minh họa.\n5. Trả lời câu hỏi. Không dùng chỉ số hoạt động hóa học chung.',[
('Trong ví dụ Na–Mg–Al, bán kính giảm theo chiều nào?', ['Al → Mg → Na.', 'Na → Mg → Al.', 'Không có xu hướng.'],1,'Trong một chu kỳ, bán kính nhìn chung giảm từ trái sang phải. Kích thước ở đây là minh họa, không có đơn vị pm.'),
('Trong một nhóm chính, tính kim loại nhìn chung tăng theo chiều nào?', ['Từ dưới lên trên.', 'Không đổi.', 'Từ trên xuống dưới.'],2,'Tính kim loại nhìn chung tăng xuống nhóm chính, tính phi kim giảm. Khí hiếm được giải thích riêng; không đồng nhất xu hướng này với hoạt động hóa học.')])]
lessonpaths=[]
for id,title,obj,instructions,questions in lessons:
 p=N/'Data/Lessons'/f'Lesson{id:02}.asset';body=f'  lessonId: {id}\n  title: {q(title)}\n  objective: {q(obj)}\n  instructions: {q(instructions)}\n  questions:\n'
 for prompt,answers,index,explanation in questions:
  body+=f'  - prompt: {q(prompt)}\n    answers:\n'+''.join(f'    - {q(a)}\n' for a in answers)+f'    correctIndex: {index}\n    explanation: {q(explanation)}\n'
 asset(p,'LessonData.cs',f'Lesson{id:02}',body);lessonpaths.append(p)
substances=[('CO2','CO2','Cacbon đioxit','Khí',False),('H2O','H2O','Nước','Lỏng',False),('H2CO3','H2CO3','Axit cacbonic','Môi trường nước',False),('CaO','CaO','Canxi oxit','Rắn',False),('CaOH2','Ca(OH)2','Canxi hiđroxit','Ít tan trong nước',False),('CaCO3','CaCO3','Canxi cacbonat','Kết tủa rắn',False),('CuOH2','Cu(OH)2','Đồng(II) hiđroxit','Kết tủa rắn',False),('CuO','CuO','Đồng(II) oxit','Rắn',False),('Litmus','Quỳ tím','Giấy quỳ tím','Vật dụng kiểm tra',True),('Phenolphthalein','Phenolphthalein','Phenolphthalein','Chỉ thị',True)]
subpaths=[]
for id,formula,name,state,indicator in substances:
 p=N/'Data/Substances'/f'{id}.asset';color='{r: 0, g: 0.55, b: 0.9, a: 1}' if id=='CuOH2' else '{r: 0.06, g: 0.07, b: 0.08, a: 1}' if id=='CuO' else '{r: 1, g: 1, b: 1, a: 1}'
 asset(p,'SubstanceData.cs',id,f'  id: {q(id)}\n  formula: {q(formula)}\n  substanceName: {q(name)}\n  physicalState: {q(state)}\n  displayColor: {color}\n  isIndicator: {int(indicator)}\n');subpaths.append(p)
react=[(1,'CarbonicAcid',[0,1],[2],'CO2 + H2O ⇌ H2CO3','CO2 được dẫn vào nước','CO2 hòa tan tạo môi trường axit yếu. Nước không có màu đỏ.'),(2,'Limewater',[0,4],[5,1],'CO2 + Ca(OH)2 → CaCO3↓ + H2O','CO2 giới hạn một liều','Kết tủa trắng làm nước vôi trong vẩn đục. MVP không dẫn dư.'),(3,'Hydration',[3,1],[4],'CaO + H2O → Ca(OH)2','Có nước và mẫu CaO','Phản ứng tỏa nhiệt. Ca(OH)2 ít tan; nhiệt độ là giá trị mô phỏng.'),(4,'Decomposition',[6],[7,1],'Cu(OH)2 —nhiệt→ CuO + H2O','Cu(OH)2 đúng vùng nung, đủ thời gian gia nhiệt mô phỏng','Chất rắn xanh thành CuO đen; H2O là sản phẩm, không phải CO2/O2.')]
reactionpaths=[]
for kind,name,rs,ps,eq,condition,explanation in react:
 p=N/'Data/Reactions'/f'{name}.asset'; body=f'  kind: {kind}\n  reactants:\n'+''.join(f'  - {v}\n' for v in rs)+'  products:\n'+''.join(f'  - {v}\n' for v in ps)+'  reactantCoefficients: '+ints([1]*len(rs))+'\n  productCoefficients: '+ints([1]*len(ps))+'\n'+f'  equation: {q(eq)}\n  condition: {q(condition)}\n  explanation: {q(explanation)}\n'
 asset(p,'ReactionData.cs',name,body);reactionpaths.append(p)
source=(N/'Scripts/Data/ElementCatalog.cs').read_text()
records=re.findall(r'E\("(\w+)", "([^"\n]+)", (\d+), ([\d.]+)f, (\d+), (\d+), ElementCategory\.(\w+), ([\d, ]+)\)',source)
assert len(records)==20
categories=['AlkaliMetal','AlkalineEarth','Metal','Metalloid','Nonmetal','Halogen','NobleGas'];old=['IA','IIA','IIIA','IVA','VA','VIA','VIIA','VIIIA']
radii=dict(re.findall(r'case "(\w+)": return ([\d.]+)f;',source))
body='  elements:\n'
for symbol,name,z,mass,group,period,category,shellstr in records:
 group=int(group);shells=[int(v) for v in shellstr.split(',')];assert sum(shells)==int(z) and len(shells)==int(period)
 body+=f'  - symbol: {q(symbol)}\n    name: {q(name)}\n    oldGroup: {old[group-1 if group<=2 else group-11]}\n    atomicNumber: {z}\n    group: {group}\n    period: {period}\n    atomicMass: {mass}\n    relativeRadius: {radii.get(symbol,0)}\n    radiusNote: {q("Mô hình minh họa, không theo tỉ lệ thực; không có đơn vị pm." if symbol in radii else "Chưa triển khai bán kính.")}\n    shells: '+ints(shells)+f'\n    category: {categories.index(category)}\n'
body+='  reactions:\n'+''.join('  - '+ref(p)+'\n' for p in reactionpaths)+'  substances:\n'+''.join('  - '+ref(p)+'\n' for p in subpaths)+'  lessons:\n'+''.join('  - '+ref(p)+'\n' for p in lessonpaths)
db=N/'Data/ChemicalDatabase.asset';asset(db,'ChemicalDatabase.cs','ChemicalDatabase',body)
# Derived project materials: do not modify publisher materials.
for name,transparent in [('Opaque',False),('Glass',True)]:
 p=N/'Materials'/f'{name}.mat'
 p.write_text('''%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!21 &2100000
Material:
  serializedVersion: 8
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_Name: '''+name+'''
  m_Shader: {fileID: 4800000, guid: 933532a4fcc9baf4fa0491de14d08ed7, type: 3}
  m_Parent: {fileID: 0}
  m_ValidKeywords: '''+('[_SURFACE_TYPE_TRANSPARENT]' if transparent else '[]')+'''
  m_InvalidKeywords: []
  m_LightmapFlags: 4
  m_EnableInstancingVariants: 0
  m_DoubleSidedGI: 0
  m_CustomRenderQueue: '''+('3000' if transparent else '-1')+'''
  stringTagMap: {RenderType: '''+('Transparent' if transparent else 'Opaque')+'''}
  disabledShaderPasses: []
  m_LockedProperties: 
  m_SavedProperties:
    serializedVersion: 3
    m_TexEnvs:
    - _BaseMap:
        m_Texture: {fileID: 0}
        m_Scale: {x: 1, y: 1}
        m_Offset: {x: 0, y: 0}
    m_Ints: []
    m_Floats:
    - _Surface: '''+str(int(transparent))+'''
    - _Blend: 0
    - _SrcBlend: '''+('5' if transparent else '1')+'''
    - _DstBlend: '''+('10' if transparent else '0')+'''
    - _ZWrite: '''+('0' if transparent else '1')+'''
    - _Cull: 0
    - _Metallic: 0
    - _Smoothness: 0.3
    - _AlphaClip: 0
    m_Colors:
    - _BaseColor: {r: 0.7, g: 0.9, b: 1, a: '''+('0.23' if transparent else '1')+'''}
    - _Color: {r: 0.7, g: 0.9, b: 1, a: '''+('0.23' if transparent else '1')+'''}
  m_BuildTextureStacks: []
''');guid(p)
models=A/'3D Laboratory Environment with Appratus/Prefabs'
def prefabroot(path):
 t=path.read_text();blocks=re.split(r'(?=--- !u!4 &)',t)
 for b in blocks:
  if b.startswith('--- !u!4') and 'm_Father: {fileID: 0}' in b: return int(re.search(r'm_GameObject: \{fileID: (-?\d+)\}',b).group(1))
 raise ValueError(path)
fields=''.join('  '+key+': '+ref(N/'Audio'/f'{name}.wav',8300000,3)+'\n' for key,name in [('ClickSound','Click'),('CorrectSound','Correct'),('IncorrectSound','Incorrect')])+'  FontShaderReference: '+ref(A/'Fonts/Roboto-Dynamic SDF.asset')+'\n  Database: '+ref(db)+'\n  VietnameseFont: '+ref(A/'Fonts/BeVietnamPro-Regular.ttf',12800000,3)+'\n  OpaqueMaterial: '+ref(N/'Materials/Opaque.mat',2100000)+'\n  GlassMaterial: '+ref(N/'Materials/Glass.mat',2100000)+'\n'
for key,name in [('TableModel','table with drawers'),('BeakerModel','Beaker'),('TubeModel','Glass_Lab_test_tube'),('HeaterModel','Spirit_Lamp with water'),('ShelfModel','shelf')]:
 wrapper={'TableModel':'ExperimentTable','BeakerModel':'Beaker','TubeModel':'TestTube','HeaterModel':'Heater'}.get(key)
 p=N/'Prefabs'/f'{wrapper}.prefab' if wrapper else models/f'{name}.prefab';fields+=f'  {key}: '+ref(p,prefabroot(p),3)+'\n'
stationnames=['Station_Lesson01_AcidicOxide','Station_Lesson02_BasicOxide','Station_Lesson08_InsolubleBase','Station_Lesson30_PeriodicTable','Station_Lesson31_PeriodicTrends']
positions=[(-4.2,0,3.6),(0,0,3.6),(4.2,0,3.6),(-3,0,-3.6),(3,0,-3.6)]
def scene(path,menu):
 names=['Systems','Canvas','EventSystem'] if menu else ['Systems','LabRoom','Player','Stations','Canvas','EventSystem']+stationnames
 ids={name:100+i*10 for i,name in enumerate(names)}; t='%YAML 1.1\n%TAG !u! tag:unity3d.com,2011:\n'
 for name in names:
  id=ids[name];parent=ids['Stations']+1 if name in stationnames else 0
  idx=stationnames.index(name) if name in stationnames else -1;pos=positions[idx] if idx>=0 else (0,0,0);rotation='{x: 0, y: 1, z: 0, w: 0}' if idx>=3 else '{x: 0, y: 0, z: 0, w: 1}'
  t+=f'''--- !u!1 &{id}
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: {id+1}}}
'''+(f'  - component: {{fileID: {id+2}}}\n' if name=='Systems' else '')+f'''  m_Layer: 0
  m_Name: {name}
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
'''
  # Canvas must already have a RectTransform before adding Canvas at runtime.
  rect=name=='Canvas';t+=f'''--- !u!{224 if rect else 4} &{id+1}
{'RectTransform' if rect else 'Transform'}:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {id}}}
  serializedVersion: 2
  m_LocalRotation: {rotation}
  m_LocalPosition: {{x: {pos[0]}, y: {pos[1]}, z: {pos[2]}}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children: '''+('\n'+''.join(f'  - {{fileID: {ids[n]+1}}}\n' for n in stationnames) if name=='Stations' else '[]\n')+f'  m_Father: {{fileID: {parent}}}\n  m_LocalEulerAnglesHint: {{x: 0, y: {180 if idx>=3 else 0}, z: 0}}\n'
  if rect:t+='  m_AnchorMin: {x: 0, y: 0}\n  m_AnchorMax: {x: 1, y: 1}\n  m_AnchoredPosition: {x: 0, y: 0}\n  m_SizeDelta: {x: 0, y: 0}\n  m_Pivot: {x: 0.5, y: 0.5}\n'
  if name=='Systems':t+=f'''--- !u!114 &{id+2}
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {id}}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: '''+ref(N/'Scripts/Core/ChemLabBootstrap.cs',11500000,3)+f'\n  m_Name: \n  m_EditorClassIdentifier: \n  MainMenu: {int(menu)}\n'+fields
 t+='--- !u!1660057539 &9223372036854775807\nSceneRoots:\n  m_ObjectHideFlags: 0\n  m_Roots:\n'+''.join(f'  - {{fileID: {ids[name]+1}}}\n' for name in names if name not in stationnames)
 path.write_text(t);guid(path)
scene(N/'Scenes/MainMenu.unity',True);scene(N/'Scenes/ChemistryLab.unity',False)
p=ROOT/'ProjectSettings/EditorBuildSettings.asset';s=p.read_text();a=s.index('  m_Scenes:');b=s.index('  m_configObjects:',a);s=s[:a]+'  m_Scenes:\n'+''.join(f'  - enabled: 1\n    path: Assets/ChemLab9/Scenes/{name}.unity\n    guid: '+guid(N/'Scenes'/f'{name}.unity')+'\n' for name in ['MainMenu','ChemistryLab'])+s[b:];p.write_text(s)
p=ROOT/'ProjectSettings/ProjectSettings.asset';s=p.read_text().replace('  activeInputHandler: 2','  activeInputHandler: 0');s=re.sub(r'^  productName:.*$', '  productName: ChemLab9',s,flags=re.M);p.write_text(s)
p=ROOT/'ProjectSettings/GraphicsSettings.asset';s=p.read_text().replace('  m_CustomRenderPipeline: {fileID: 0}','  m_CustomRenderPipeline: '+ref(A/'Settings/PC_RPAsset.asset'));p.write_text(s)
p=ROOT/'ProjectSettings/QualitySettings.asset';s=p.read_text();s=re.sub(r'customRenderPipeline: \{[^\n]+\}', 'customRenderPipeline: '+ref(A/'Settings/PC_RPAsset.asset'),s);p.write_text(s)
for p in sorted(N.rglob('*')):
 if p.suffix!='.meta': guid(p)
guid(N)
print('Seeded two scenes, five lessons, four reactions, ten substances and twenty elements.')
