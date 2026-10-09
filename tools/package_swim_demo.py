"""Create the standalone Unity source archive; never include local caches or build output."""
from pathlib import Path
import zipfile

repo = Path(__file__).resolve().parents[1]
project = repo / 'SwimDemo'
destination = repo / 'downloads/SwimDemo_Unity6000.6.zip'
excluded = {'Library', 'Temp', 'Obj', 'Logs', 'Builds', 'Build', 'UserSettings', '.vs', '.idea', 'bin', 'obj', '__pycache__'}
destination.parent.mkdir(exist_ok=True)
files = sorted(p for p in project.rglob('*') if p.is_file() and not any(part in excluded for part in p.relative_to(project).parts))
with zipfile.ZipFile(destination, 'w', compression=zipfile.ZIP_DEFLATED, compresslevel=9) as archive:
    for path in files:
        entry = zipfile.ZipInfo(path.relative_to(repo).as_posix(), date_time=(2026, 10, 9, 0, 0, 0))
        entry.compress_type = zipfile.ZIP_DEFLATED
        entry.external_attr = 0o644 << 16
        archive.writestr(entry, path.read_bytes())
with zipfile.ZipFile(destination) as archive:
    assert archive.testzip() is None, 'ZIP checksum failure'
    required = {
        'SwimDemo/Assets/SwimDemo/Scenes/Swimming.unity',
        'SwimDemo/Assets/SwimDemo/Characters/YBot_Swim.fbx',
        'SwimDemo/Assets/SwimDemo/Characters/YBot_Swim.fbx.meta',
        'SwimDemo/Packages/manifest.json',
        'SwimDemo/ProjectSettings/ProjectVersion.txt',
        'SwimDemo/README.md',
        'SwimDemo/docs/fbx-inspection.json'
    }
    assert required <= set(archive.namelist()), 'Incomplete Unity project archive'
    for path in files:
        assert archive.read(path.relative_to(repo).as_posix()) == path.read_bytes(), f'Archive differs: {path}'
print(f'PASS standalone source archive: {len(files)} files, {destination.stat().st_size:,} bytes; all contents match the source project. {destination}')
