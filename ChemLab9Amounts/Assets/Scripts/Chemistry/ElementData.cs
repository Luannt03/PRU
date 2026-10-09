using UnityEngine;

namespace ChemLab.Data
{
    /// <summary>
    /// ScriptableObject chứa thông số chi tiết nguyên tố hóa học.
    /// Phục vụ Bài 30 (Bảng tuần hoàn) và Bài 31 (Sự biến đổi tính chất).
    /// </summary>
    [CreateAssetMenu(fileName = "NewElementData", menuName = "ChemLab/Chemistry/Element Data")]
    public class ElementData : ScriptableObject
    {
        [Header("--- THÔNG TIN CƠ BẢN ---")]
        [Tooltip("Tên tiếng Anh hoặc tên hóa học chuẩn (VD: Copper, Carbon)")]
        public string elementName;

        [Tooltip("Ký hiệu hóa học (VD: Cu, C, Ca)")]
        public string symbol;

        [Tooltip("Số hiệu nguyên tử Z = số proton; số electron = Z với nguyên tử trung hòa")]
        public int atomicNumber;

        [Tooltip("Nguyên tử khối trung bình; không phải số khối đồng vị A")]
        public float atomicMass;

        [Header("--- CẤU HÌNH ELECTRON & VỊ TRÍ ---")]
        [Tooltip("Chu kỳ (Tương ứng số lớp Electron)")]
        public int period;

        [Tooltip("Nhóm IUPAC 1–18; không đồng nhất với số electron lớp ngoài cùng")]
        public int group;

        [Tooltip("Mảng chứa số electron trên từng lớp [Lớp 1, Lớp 2, Lớp 3, ...]")]
        public int[] electronPerShell;

        [Header("--- ĐẶC TÍNH LÝ HÓA (BÀI 31) ---")]
        [Tooltip("Trường dữ liệu cũ, không dùng trong MVP khi chưa có nguồn và định nghĩa bán kính")]
        public float atomicRadius;

        [Tooltip("Độ âm điện theo thang Pauling")]
        public float electronegativity;

        [Tooltip("Màu đại diện của nguyên tố khi render mô hình 3D")]
        public Color elementColor = Color.gray;

        [TextArea(3, 5)]
        [Tooltip("Mô tả vắn tắt kiến thức Hóa học 9 về nguyên tố này")]
        public string description;
    }
}