using UnityEngine;

namespace ChemLab.Data
{
    /// <summary>
    /// Trạng thái tồn tại của hóa chất.
    /// </summary>
    public enum ChemicalState
    {
        Solid,      // Chất rắn (CaO, Cu(OH)2, CuO)
        Liquid,     // Dung dịch/Chất lỏng (H2O, Ca(OH)2, Phenolphthalein)
        Gas         // Khí (CO2, Hơi nước)
    }

    /// <summary>
    /// ScriptableObject định nghĩa thuộc tính hiển thị và phản ứng của hóa chất.
    /// </summary>
    [CreateAssetMenu(fileName = "NewChemicalData", menuName = "ChemLab/Chemistry/Chemical Data")]
    public class ChemicalData : ScriptableObject
    {
        [Header("--- NHẬN DẠNG HÓA CHẤT ---")]
        [Tooltip("ID định danh duy nhất (VD: CO2, CaO, CuOH2)")]
        public string chemicalId;

        [Tooltip("Công thức hóa học hiển thị (VD: CO₂, Ca(OH)₂)")]
        public string formula;

        [Tooltip("Tên hóa học/Tên thông thường (VD: Khí Cacbonic, Vôi sống)")]
        public string chemicalName;

        [Tooltip("Trạng thái vật lý ban đầu")]
        public ChemicalState state;

        [Header("--- THUỘC TÍNH HIỂN THỊ 3D ---")]
        [Tooltip("Màu sắc đại diện cho dung dịch hoặc kết tủa trong ly/ống nghiệm")]
        public Color liquidColor = new Color(1f, 1f, 1f, 0.3f);

        [Tooltip("Màu biến đổi khi tiếp xúc chất chỉ thị (VD: Quỳ tím -> Đỏ, Phenol -> Hồng)")]
        public Color indicatorColor = Color.clear;

        [Tooltip("Có tạo hiện tượng vẩn đục/kết tủa không? (VD: CaCO3)")]
        public bool isCloudy = false;

        [Header("--- THÔNG SỐ ĐẶC TÍNH ---")]
        [Range(0f, 14f)]
        [Tooltip("Giá trị pH minh họa cũ; không dùng để quyết định phản ứng trong ChemLab9")]
        public float phValue = 7.0f;

        [Tooltip("Prefab hiệu ứng đi kèm (VD: Khói nóng, Bong bóng khí sủi bọt)")]
        public GameObject vfxPrefab;

        // Biến phụ trợ đồng bộ giao tiếp code
        public string chemicalFormula => formula;
    }
}