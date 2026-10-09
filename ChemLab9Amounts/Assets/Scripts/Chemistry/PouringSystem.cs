using UnityEngine;
using ChemLab.Data;
using ChemLab.Equipment;
namespace ChemLab.Chemistry
{
    // Compatibility for the user's original Classroom scene. New lessons use SimulationState.
    public class PouringSystem : MonoBehaviour
    {
        public static PouringSystem Instance { get; private set; }
        void Awake() { if (Instance != null && Instance != this) { Destroy(gameObject); return; } Instance = this; }
        void OnDestroy() { if (Instance == this) Instance = null; }
        public void PourChemical(Beaker source, Beaker target)
        {
            if (source == null || target == null || source == target || source.GetChemical() == null) return;
            ChemicalData a = source.GetChemical(), b = target.GetChemical();
            if (b == null) { target.SetChemical(a); source.SetChemical(null); return; }
            // An indicator reports a basic medium; it does not turn arbitrary mixtures pink.
            if (a.chemicalId == "Phenolphthalein" && b.chemicalId == "CaOH2")
            { target.SetCustomColor(new Color(1f,.12f,.55f,.7f)); source.SetChemical(null); return; }
            if (a.chemicalId == "Phenolphthalein" && b.chemicalId == "H2O")
            { source.SetChemical(null); return; }
            Debug.Log("[ChemLab] Cảnh Classroom chỉ hỗ trợ chuyển cốc trống và chất chỉ thị. Mở ChemistryLab để chạy năm bài có kiểm tra điều kiện và lượng chất.");
        }
    }
}
