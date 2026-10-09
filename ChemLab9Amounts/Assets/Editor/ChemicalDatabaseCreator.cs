using ChemLab.Data;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Công cụ Editor: Khởi tạo sẵn các ScriptableObject dữ liệu Hóa học 9.
/// Đặt trong Assets/Editor/. Chạy từ menu ChemLab trên thanh menu.
/// </summary>
public static class ChemicalDatabaseCreator
{
    private const string ChemicalPath = "Assets/ScriptableObjects/Chemicals";
    private const string ElementPath = "Assets/ScriptableObjects/Elements";

    [MenuItem("ChemLab/3. Generate Class 9 Chemistry Assets")]
    public static void GenerateChemistryData()
    {
        EnsureDirectoryExists(ChemicalPath);
        EnsureDirectoryExists(ElementPath);

        // --- BÀI 1 & BÀI 2 & BÀI 8: TẠO HÓA CHẤT MẪU ---
        CreateChemicalAsset("CO2", "CO₂", "Khí Cacbonic", ChemicalState.Gas, new Color(0.9f, 0.9f, 0.9f, 0.1f), 5.5f);
        CreateChemicalAsset("CaO", "CaO", "Vôi sống", ChemicalState.Solid, Color.white, 12.0f);
        CreateChemicalAsset("CaOH2", "Ca(OH)₂", "Dung dịch Nước vôi trong", ChemicalState.Liquid, new Color(1f, 1f, 1f, 0.2f), 12.5f);
        CreateChemicalAsset("CuOH2", "Cu(OH)₂", "Đồng(II) Hydroxit (Kết tủa)", ChemicalState.Solid, new Color(0f, 0.6f, 0.8f, 0.9f), 7.0f);
        CreateChemicalAsset("CuO", "CuO", "Đồng(II) Oxit", ChemicalState.Solid, new Color(0.1f, 0.1f, 0.1f, 1f), 7.0f);
        CreateChemicalAsset("Phenolphthalein", "C₂₀H₁₄O₄", "Phenolphthalein", ChemicalState.Liquid, new Color(1f, 1f, 1f, 0.1f), 7.0f);

        CreateChemicalAsset("H2O", "H₂O", "Nước", ChemicalState.Liquid, new Color(1f,1f,1f,.2f), 7f);
        CreateChemicalAsset("H2CO3", "H₂CO₃", "Axit cacbonic", ChemicalState.Liquid, new Color(1f,1f,1f,.2f), 7f);
        CreateChemicalAsset("CaCO3", "CaCO₃", "Canxi cacbonat", ChemicalState.Solid, Color.white, 7f);
        foreach (var e in ChemLab9.Data.ElementCatalog.FirstTwenty())
            CreateElementAsset(e.name, e.symbol, e.atomicNumber, e.atomicMass, e.period, e.group, e.shells, 0f, Color.gray);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[ChemLab] Đã khởi tạo hoàn tất toàn bộ dữ liệu Hóa chất & Nguyên tố Hóa 9!");
    }

    private static void CreateChemicalAsset(string id, string formula, string name, ChemicalState state, Color color, float ph)
    {
        string assetPath = $"{ChemicalPath}/Data_{id}.asset";
        if (AssetDatabase.LoadAssetAtPath<ChemicalData>(assetPath) != null) return;

        ChemicalData data = ScriptableObject.CreateInstance<ChemicalData>();
        data.chemicalId = id;
        data.formula = formula;
        data.chemicalName = name;
        data.state = state;
        data.liquidColor = color;
        data.phValue = ph;

        AssetDatabase.CreateAsset(data, assetPath);
        Debug.Log($"[ChemLab] Đã tạo ChemicalAsset: {id}");
    }

    private static void CreateElementAsset(string name, string symbol, int atomicNum, float mass, int period, int group, int[] shells, float radius, Color color)
    {
        string assetPath = $"{ElementPath}/Element_{symbol}.asset";
        if (AssetDatabase.LoadAssetAtPath<ElementData>(assetPath) != null) return;

        ElementData data = ScriptableObject.CreateInstance<ElementData>();
        data.elementName = name;
        data.symbol = symbol;
        data.atomicNumber = atomicNum;
        data.atomicMass = mass;
        data.period = period;
        data.group = group;
        data.electronPerShell = shells;
        data.atomicRadius = radius;
        data.elementColor = color;

        AssetDatabase.CreateAsset(data, assetPath);
        Debug.Log($"[ChemLab] Đã tạo ElementAsset: {symbol}");
    }

    private static void EnsureDirectoryExists(string path)
    {
        if (!AssetDatabase.IsValidFolder(path))
        {
            string parent = System.IO.Path.GetDirectoryName(path).Replace('\\', '/');
            string folderName = System.IO.Path.GetFileName(path);
            EnsureDirectoryExists(parent);
            AssetDatabase.CreateFolder(parent, folderName);
        }
    }
}
