"""Create project-owned visual wrappers while retaining publisher meshes/material references."""
from pathlib import Path
import re
from seed_project import A,N,guid
header='%YAML 1.1\n%TAG !u! tag:unity3d.com,2011:\n'
common='  m_ObjectHideFlags: 0\n  m_CorrespondingSourceObject: {fileID: 0}\n  m_PrefabInstance: {fileID: 0}\n  m_PrefabAsset: {fileID: 0}\n'
def go(id,name,components):
 return f'--- !u!1 &{id}\nGameObject:\n'+common+'  serializedVersion: 6\n  m_Component:\n'+''.join(f'  - component: {{fileID: {c}}}\n' for c in components)+f'  m_Layer: 0\n  m_Name: {name}\n  m_TagString: Untagged\n  m_Icon: {{fileID: 0}}\n  m_NavMeshLayer: 0\n  m_StaticEditorFlags: 0\n  m_IsActive: 1\n'
def tr(id,owner,parent=0,children=(),rect=False):
 return f'--- !u!{224 if rect else 4} &{id}\n'+('RectTransform' if rect else 'Transform')+':\n'+common+f'  m_GameObject: {{fileID: {owner}}}\n  serializedVersion: 2\n  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}\n  m_LocalPosition: {{x: 0, y: 0, z: 0}}\n  m_LocalScale: {{x: 1, y: 1, z: 1}}\n  m_ConstrainProportionsScale: 0\n  m_Children: '+ ('\n'+''.join(f'  - {{fileID: {v}}}\n' for v in children) if children else '[]\n')+f'  m_Father: {{fileID: {parent}}}\n  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}\n'+('  m_AnchorMin: {x: 0, y: 0}\n  m_AnchorMax: {x: 1, y: 1}\n  m_AnchoredPosition: {x: 0, y: 0}\n  m_SizeDelta: {x: 0, y: 0}\n  m_Pivot: {x: 0.5, y: 0.5}\n' if rect else '')
def source_root(t):
 for b in re.split(r'(?=--- !u!4 &)',t):
  if b.startswith('--- !u!4') and 'm_Father: {fileID: 0}' in b:
   return int(re.search(r'--- !u!4 &(-?\d+)',b).group(1)),int(re.search(r'm_GameObject: \{fileID: (-?\d+)\}',b).group(1))
 raise ValueError('No model root')
models=A/'3D Laboratory Environment with Appratus/Prefabs'
map={'Beaker':'Beaker','TestTube':'Glass_Lab_test_tube','ChemicalBottle':'Erlenmeyer_flask','GasSource':'florence_flask','Heater':'Spirit_Lamp with water','ExperimentTable':'table with drawers'}
for name in ['Player','Beaker','TestTube','ChemicalBottle','GasSource','GasTube','Heater','ExperimentTable','ElementTile','AtomModel','Liquid','Precipitate','ResultPanel']:
 p=N/'Prefabs'/f'{name}.prefab'
 if name=='Player' and p.exists(): continue
 root=9000000000000000100;rt=root+1
 if name in map:
  t=(models/(map[name]+'.prefab')).read_text();tid,gid=source_root(t)
  # Rename only the model root and parent it under our own wrapper.
  blocks=re.split(r'(?=--- !u!)',t)
  for i,b in enumerate(blocks):
   if b.startswith(f'--- !u!4 &{tid}\n'): blocks[i]=b.replace('m_Father: {fileID: 0}',f'm_Father: {{fileID: {rt}}}')
   if b.startswith(f'--- !u!1 &{gid}\n'): blocks[i]=re.sub(r'  m_Name: .+','  m_Name: Model',b)
  t=''.join(blocks)+go(root,name,[rt])+tr(rt,root,children=[tid])
 elif name=='ResultPanel':
  t=header+go(root,name,[rt,root+2,root+3])+tr(rt,root,rect=True)
  t+=f'--- !u!222 &{root+2}\nCanvasRenderer:\n'+common+f'  m_GameObject: {{fileID: {root}}}\n  m_CullTransparentMesh: 1\n'
  t+=f'--- !u!114 &{root+3}\nMonoBehaviour:\n'+common+f'  m_GameObject: {{fileID: {root}}}\n  m_Enabled: 1\n  m_EditorHideFlags: 0\n  m_Script: {{fileID: 11500000, guid: fe87c0e1cc204ed48ad3b37840f39efc, type: 3}}\n  m_Name: \n  m_EditorClassIdentifier: \n  m_Material: {{fileID: 0}}\n  m_Color: {{r: 0.035, g: 0.065, b: 0.1, a: 0.97}}\n  m_RaycastTarget: 1\n  m_Maskable: 1\n  m_Sprite: {{fileID: 0}}\n  m_Type: 0\n  m_FillCenter: 1\n  m_FillAmount: 1\n  m_PreserveAspect: 0\n'
 elif name=='Player':
  t=header+go(root,name,[rt,root+2])+tr(rt,root)
  t+=f'--- !u!143 &{root+2}\nCharacterController:\n'+common+f'  m_GameObject: {{fileID: {root}}}\n  m_Material: {{fileID: 0}}\n  m_IsTrigger: 0\n  m_Enabled: 1\n  serializedVersion: 3\n  m_Height: 1.8\n  m_Radius: 0.3\n  m_SlopeLimit: 45\n  m_StepOffset: 0.25\n  m_SkinWidth: 0.08\n  m_MinMoveDistance: 0\n  m_Center: {{x: 0, y: 0.9, z: 0}}\n'
 else:
  t=header+go(root,name,[rt,root+2,root+3])+tr(rt,root)
  mesh=10207 if name=='AtomModel' else 10206 if name in ['Liquid','Precipitate','GasTube'] else 10202
  t+=f'--- !u!33 &{root+2}\nMeshFilter:\n'+common+f'  m_GameObject: {{fileID: {root}}}\n  m_Mesh: {{fileID: {mesh}, guid: 0000000000000000e000000000000000, type: 0}}\n'
  t+=f'--- !u!23 &{root+3}\nMeshRenderer:\n'+common+f'  m_GameObject: {{fileID: {root}}}\n  m_Enabled: 1\n  m_CastShadows: 1\n  m_ReceiveShadows: 1\n  m_Materials:\n  - {{fileID: 2100000, guid: {guid(N/"Materials/Opaque.mat")}, type: 2}}\n  m_SortingLayerID: 0\n  m_SortingOrder: 0\n'
 p.write_text(t);guid(p)
print('Created 13 project wrapper/template prefabs, with publisher files unchanged.')
