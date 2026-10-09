using UnityEngine;

namespace ChemLab.Interaction
{
    /// <summary>
    /// Interface chung cho tất cả các vật thể có thể tương tác trong phòng thí nghiệm.
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// Tên hoặc văn bản gợi ý hiển thị lên UI khi Player nhìn vào vật thể (VD: "Nhấn E để cầm Cốc Thí Nghiệm").
        /// </summary>
        string GetInteractPrompt();

        /// <summary>
        /// Kích hoạt hành động tương tác chính (như Cầm lên / Đặt xuống / Bấm nút).
        /// </summary>
        void Interact(Transform handPosition);

        /// <summary>
        /// Kích hoạt hành động phụ khi đang cầm vật thể (VD: Rót dung dịch, Đốt lửa).
        /// </summary>
        void UseAction();
    }
}