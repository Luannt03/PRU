"""Run with Blender, read the user's FBX and inspect actual bones/skinning/pose movement."""
import argparse
import hashlib
import json
from pathlib import Path
import sys
import bpy

args = sys.argv[sys.argv.index('--') + 1:] if '--' in sys.argv else []
parser = argparse.ArgumentParser()
parser.add_argument('--output', default='SwimDemo/docs/fbx-inspection.json')
options = parser.parse_args(args)
repo = Path(__file__).resolve().parents[1]
source = repo / 'SwimDemo/Assets/SwimDemo/Characters/YBot_Swim.fbx'
bpy.ops.wm.read_factory_settings(use_empty=True)
bpy.ops.import_scene.fbx(filepath=str(source))
armatures = [o for o in bpy.data.objects if o.type == 'ARMATURE']
assert len(armatures) == 1, 'Expected one skeleton'
arm = armatures[0]
for name in ['Hips', 'Head', 'LeftArm', 'RightArm', 'LeftForeArm', 'RightForeArm', 'LeftUpLeg', 'RightUpLeg', 'LeftLeg', 'RightLeg']:
    assert 'mixamorig:' + name in arm.pose.bones, 'Missing joint ' + name
meshes = [o for o in bpy.data.objects if o.type == 'MESH']
assert len(meshes) == 2, 'Expected the uploaded Y Bot body and joints meshes'
assert all(any(m.type == 'ARMATURE' and m.object == arm for m in o.modifiers) for o in meshes), 'Meshes are not skinned to the skeleton'
assert len(bpy.data.actions) == 1, 'Expected the uploaded single animation'
samples = []
for frame in [1, 15, 30, 45, 60]:
    bpy.context.scene.frame_set(frame)
    hips = arm.matrix_world @ arm.pose.bones['mixamorig:Hips'].head
    points = {}
    for name in ['LeftHand', 'RightHand', 'LeftFoot', 'RightFoot']:
        points[name] = list((arm.matrix_world @ arm.pose.bones['mixamorig:' + name].head) - hips)
    samples.append({'frame': frame, 'hips_world_blender': list(hips), 'joints_relative_to_hips': points})
delta = sum((samples[-1]['joints_relative_to_hips']['LeftHand'][i] - samples[0]['joints_relative_to_hips']['LeftHand'][i]) ** 2 for i in range(3)) ** .5
assert delta > .05, 'Clip must change joint pose, not only move the whole root'
report = {
    'verification': 'Blender FBX import and sampled pose only; Unity Humanoid import/Play Mode/build have not been run.',
    'blender_version': bpy.app.version_string,
    'source': source.relative_to(repo).as_posix(),
    'sha256': hashlib.sha256(source.read_bytes()).hexdigest(),
    'bone_count': len(arm.data.bones),
    'meshes': [{'name': m.name, 'vertices': len(m.data.vertices), 'materials': [x.name for x in m.data.materials]} for m in meshes],
    'actions': [{'name': a.name, 'blender_frame_range': list(a.frame_range), 'curve_count': len(a.fcurves)} for a in bpy.data.actions],
    'hand_relative_pose_change_metres': delta,
    'samples_in_blender_coordinates': samples
}
destination = Path(options.output)
if not destination.is_absolute(): destination = repo / destination
destination.parent.mkdir(parents=True, exist_ok=True)
destination.write_text(json.dumps(report, indent=2), encoding='utf-8')
print(f'PASS original FBX: {len(arm.data.bones)} bones, {len(meshes)} skinned meshes, one animation; left-hand pose change relative to hips {delta:.3f} m. Report: {destination}')
