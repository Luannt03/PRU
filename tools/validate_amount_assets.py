"""Validate common authored Unity assets, isolated progress and the preserved three lessons."""
from pathlib import Path
import runpy
import sys

repo = Path(__file__).resolve().parents[1]
original = repo / 'ChemLab9'
project = repo / 'ChemLab9Amounts'
sys.argv = ['validate_assets.py', str(project)]
runpy.run_path(str(repo / 'tools/validate_assets.py'), run_name='__main__')
for path in (original / 'Assets/ChemLab9/Scripts/PeriodicTable').glob('*.cs'):
    assert path.read_bytes() == (project / path.relative_to(original)).read_bytes(), f'Periodic visualizer changed: {path.name}'
for lesson in ['Lesson08Manager.cs', 'Lesson30Manager.cs', 'Lesson31Manager.cs']:
    rel = Path('Assets/ChemLab9/Scripts/Lessons') / lesson
    assert (original / rel).read_bytes() == (project / rel).read_bytes(), f'Unrequested lesson changed: {lesson}'
assert '"ChemLab9Amounts.Completed."' in (project / 'Assets/ChemLab9/Scripts/Core/GameManager.cs').read_text()
assert 'productName: ChemLab9Amounts' in (project / 'ProjectSettings/ProjectSettings.asset').read_text()
for file in (project / 'Assets').rglob('*.cs'):
    assert Path(str(file) + '.meta').exists(), f'Missing script metadata: {file}'
original_assets = original / 'Assets'
preserved_count = 0
for path in original_assets.rglob('*'):
    if path.is_file() and path.suffix != '.cs' and path.relative_to(original_assets).parts[0] not in {'ChemLab9'} and not path.name.startswith('InputSystem_Actions.inputactions'):
        assert path.read_bytes() == (project / path.relative_to(original)).read_bytes(), f'Original imported asset changed: {path}'
        preserved_count += 1
print(f'PASS project identity, isolated progress, source for Lessons 8/30/31 and {preserved_count} preserved imported/font/assets files. This is static validation, not Unity import or Play Mode.')
