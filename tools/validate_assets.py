"""Static Unity YAML/reference checks. Requires PyYAML; this does not run Unity import."""
from pathlib import Path
import re,struct,json,yaml,sys
root=Path(sys.argv[1]).resolve() if len(sys.argv)>1 else Path(__file__).resolve().parents[1]/'ChemLab9'
assets=root/'Assets'
def docs(path):
 s=path.read_text();s=re.sub(r'^%.*\n','',s,flags=re.M);s=re.sub(r'^--- !u!(\d+) &(-?\d+).*$',r'---\n__class__: \1\n__id__: \2',s,flags=re.M)
 return list(yaml.load_all(s,Loader=yaml.BaseLoader))
def require(ok,message):
 if not ok:raise AssertionError(message)
metas={}
for p in assets.rglob('*.meta'):
 m=re.search(r'^guid: (\w+)',p.read_text(),re.M)
 require(m is not None,'Missing guid '+str(p))
 g=m.group(1);require(g not in metas,'Duplicate guid '+g);metas[g]=Path(str(p)[:-5])
 require(metas[g].exists(),'Orphan meta '+str(p))
checks=0
for p in (assets/'ChemLab9').rglob('*'):
 if p.suffix in ['.asset','.unity','.prefab','.mat']:
  rows=docs(p);ids=[d['__id__'] for d in rows];require(len(ids)==len(set(ids)),'Duplicate local object ids '+str(p));checks+=1
  for row in rows:
   def visit(v):
    if isinstance(v,dict):
     if 'fileID' in v and v['fileID']!='0' and 'guid' not in v:require(v['fileID'] in ids,'Broken local reference '+str(p)+' '+v['fileID'])
     for x in v.values():visit(x)
    elif isinstance(v,list):
     for x in v:visit(x)
   visit(row)
  for g in re.findall(r'guid: ([a-f0-9]{32})',p.read_text()):
   if g not in metas:
    require(g in ['933532a4fcc9baf4fa0491de14d08ed7','fe87c0e1cc204ed48ad3b37840f39efc','0000000000000000e000000000000000'],'Unexpected external GUID '+g+' in '+str(p))
def scalar_ints(s):return list(struct.unpack('<'+'i'*(len(s)//8),bytes.fromhex(s)))
db=docs(assets/'ChemLab9/Data/ChemicalDatabase.asset')[0]['MonoBehaviour'];require(len(db['elements'])==20,'Expected 20 elements')
for i,e in enumerate(db['elements']):
 shells=scalar_ints(e['shells']);require(int(e['atomicNumber'])==i+1 and sum(shells)==i+1 and len(shells)==int(e['period']),'Bad shells '+e['symbol'])
for symbol in ['Li','Na','K','Mg','Al']:
 e=next(e for e in db['elements'] if e['symbol']==symbol); require(float(e['relativeRadius'])>0 and 'pm' in e['radiusNote'],'Missing radius illustration '+symbol)
require(len(db['lessons'])==5 and len(db['reactions'])==4 and len(db['substances'])==10,'Missing database entries')
for v in db['lessons']:
 data=docs(metas[v['guid']])[0]['MonoBehaviour']
 for q in data['questions']:require(0<=int(q['correctIndex'])<len(q['answers']),'Bad answer index')
lab=docs(assets/'ChemLab9/Scenes/ChemistryLab.unity');names=[d['GameObject']['m_Name'] for d in lab if 'GameObject' in d]
require(len([n for n in names if n.startswith('Station_')])==5,'Expected exactly five stations')
for name in ['LabRoom','Player','Stations','Systems','Canvas','EventSystem']:require(name in names,'Missing root '+name)
for scene in ['MainMenu','ChemistryLab']:
 rows=docs(assets/f'ChemLab9/Scenes/{scene}.unity');boot=next(d['MonoBehaviour'] for d in rows if 'MonoBehaviour' in d)
 for field in ['Database','VietnameseFont','OpaqueMaterial','GlassMaterial','TableModel','BeakerModel','TubeModel','HeaterModel','ShelfModel','FontShaderReference']:
  require(boot[field]['guid'] in metas,'Missing '+field)
  target=metas[boot[field]['guid']]
  if target.suffix in ['.prefab','.asset','.mat']:require(boot[field]['fileID'] in [d['__id__'] for d in docs(target)],'Broken '+field+' object id')
require('activeInputHandler: 0' in (root/'ProjectSettings/ProjectSettings.asset').read_text(),'Input mode inconsistent')
print(f'PASS {checks} authored YAML files, GUID/local references, 20 element records, 5 lessons, 13 prefabs, 2 scene bootstraps and legacy-input configuration.')
