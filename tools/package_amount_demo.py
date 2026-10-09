"""Package the separate five-station source project with no local caches/build output."""
from pathlib import Path
import zipfile
repo=Path(__file__).resolve().parents[1]
project=repo/'ChemLab9Amounts'
destination=repo/'downloads/ChemLab9Amounts_Unity6000.6.zip'
excluded={'Library','Temp','Obj','Logs','Builds','Build','UserSettings','.vs','.idea','bin','obj','__pycache__'}
files=sorted(p for p in project.rglob('*') if p.is_file() and not any(part in excluded for part in p.relative_to(project).parts))
destination.parent.mkdir(exist_ok=True)
with zipfile.ZipFile(destination,'w',compression=zipfile.ZIP_DEFLATED,compresslevel=9) as z:
    for p in files:
        entry=zipfile.ZipInfo(p.relative_to(repo).as_posix(),date_time=(2026,10,9,0,0,0));entry.compress_type=zipfile.ZIP_DEFLATED;entry.external_attr=0o644<<16
        z.writestr(entry,p.read_bytes())
with zipfile.ZipFile(destination) as z:
    assert z.testzip() is None
    required={'ChemLab9Amounts/README.md','ChemLab9Amounts/Assets/ChemLab9/Scenes/MainMenu.unity','ChemLab9Amounts/Assets/ChemLab9/Scenes/ChemistryLab.unity','ChemLab9Amounts/Packages/manifest.json','ChemLab9Amounts/ProjectSettings/ProjectVersion.txt'}
    assert required<=set(z.namelist())
    for p in files: assert z.read(p.relative_to(repo).as_posix())==p.read_bytes(),f'Mismatched file: {p}'
print(f'PASS standalone ChemLab9Amounts source archive: {len(files)} files, {destination.stat().st_size:,} bytes. All contents match source.')
