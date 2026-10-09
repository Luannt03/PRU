"""Validate the authored project/reference graph, without claiming to run Unity's importers."""
from pathlib import Path
import hashlib
import json
import re
import yaml

root = Path(__file__).resolve().parents[1] / 'SwimDemo'
assets = root / 'Assets'

def require(condition, message):
    if not condition:
        raise AssertionError(message)

def documents(path):
    source = re.sub(r'^%.*\n', '', path.read_text(), flags=re.M)
    source = re.sub(r'^--- !u!(\d+) &(-?\d+).*$', r'---\n__class__: \1\n__id__: \2', source, flags=re.M)
    return list(yaml.load_all(source, Loader=yaml.BaseLoader))

guids = {}
for path in assets.rglob('*'):
    if path.suffix == '.meta':
        match = re.search(r'^guid: ([a-f0-9]{32})$', path.read_text(), re.M)
        require(match is not None, f'Missing GUID: {path}')
        require(match[1] not in guids, f'Duplicate GUID: {path}')
        target = Path(str(path)[:-5])
        require(target.exists(), f'Orphan metadata: {path}')
        guids[match[1]] = target
    else:
        require(Path(str(path) + '.meta').is_file(), f'Untracked metadata: {path}')

yaml_files = list(assets.rglob('*.asset')) + list(assets.rglob('*.unity'))
for path in yaml_files:
    rows = documents(path)
    ids = [row['__id__'] for row in rows]
    require(len(ids) == len(set(ids)), f'Duplicate local ID: {path}')
    def visit(value):
        if isinstance(value, dict):
            if value.get('fileID', '0') != '0':
                if 'guid' in value:
                    target = guids.get(value['guid'])
                    require(target is not None, f'Broken GUID in {path}: {value}')
                    if target.suffix == '.asset':
                        require(value['fileID'] in [r['__id__'] for r in documents(target)], f'Broken asset ID: {target}')
                    elif target.suffix in ('.cs', '.shader', '.ttf'):
                        expected = {'.cs': '11500000', '.shader': '4800000', '.ttf': '12800000'}[target.suffix]
                        require(value['fileID'] == expected, f'Wrong importer main ID: {target}')
                else:
                    require(value['fileID'] in ids, f'Broken local reference in {path}: {value}')
            for item in value.values():
                visit(item)
        elif isinstance(value, list):
            for item in value:
                visit(item)
    for row in rows:
        visit(row)

scene_path = assets / 'SwimDemo/Scenes/Swimming.unity'
scene = documents(scene_path)
bootstrap = next(row['MonoBehaviour'] for row in scene if 'MonoBehaviour' in row)
for field in ('Settings', 'UIFont', 'SolidShader', 'WaterShader', 'BubbleShader'):
    require(bootstrap[field]['fileID'] != '0', f'Missing bootstrap reference: {field}')
build = documents(root / 'ProjectSettings/EditorBuildSettings.asset')[0]['EditorBuildSettings']['m_Scenes']
require(len(build) == 1 and build[0]['enabled'] == '1' and guids[build[0]['guid']] == scene_path, 'Incorrect build scene')
graphics = documents(root / 'ProjectSettings/GraphicsSettings.asset')[0]['GraphicsSettings']
require(graphics['m_CustomRenderPipeline']['fileID'] == '0' and not graphics['m_RenderPipelineGlobalSettingsMap'], 'Expected Built-in graphics')
quality = documents(root / 'ProjectSettings/QualitySettings.asset')[0]['QualitySettings']
require(all(q['customRenderPipeline']['fileID'] == '0' for q in quality['m_QualitySettings']), 'Unexpected quality pipeline')
player = documents(root / 'ProjectSettings/ProjectSettings.asset')[0]['PlayerSettings']
require(player['activeInputHandler'] == '0', 'Expected Legacy Input Manager')
axes = documents(root / 'ProjectSettings/InputManager.asset')[0]['InputManager']['m_Axes']
require({'Horizontal', 'Vertical', 'Mouse X', 'Mouse Y', 'Mouse ScrollWheel'} <= {a['m_Name'] for a in axes}, 'Missing input axes')
manifest = json.loads((root / 'Packages/manifest.json').read_text())['dependencies']
require(not any('render-pipelines' in p or p == 'com.unity.inputsystem' for p in manifest), 'Unexpected SRP/Input System dependency')
for path in assets.rglob('*.asmdef'):
    assembly = json.loads(path.read_text())
    known = {json.loads(p.read_text())['name'] for p in assets.rglob('*.asmdef')}
    require(all(r in known for r in assembly.get('references', [])), f'Missing assembly: {path}')
fbx = assets / 'SwimDemo/Characters/YBot_Swim.fbx'
require(hashlib.sha256(fbx.read_bytes()).hexdigest() == '67d55dc7d65a5dd7c135303fd7890dff9298dab6c1777c858f0403b1e20e1e79', 'The uploaded FBX was changed')
require(fbx.read_bytes().startswith(b'Kaydara FBX Binary'), 'Expected binary FBX')
require('6000.6.0f1' in (root / 'ProjectSettings/ProjectVersion.txt').read_text(), 'Wrong Unity version')
print(f'PASS {len(guids)} metadata entries, {len(yaml_files)} authored Unity assets/scenes, scene/font/shader references, build/input/pipeline/assembly settings and original uploaded FBX checksum. Unity import and shader compilation are separate checks.')
