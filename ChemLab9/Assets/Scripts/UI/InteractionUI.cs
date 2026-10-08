using UnityEngine;
using TMPro;

namespace ChemLab.UI
{
    /// <summary>
    /// Quản lý việc hiển thị UI gợi ý tương tác (Prompt Text) và Tâm ngắm (Crosshair).
    /// </summary>
    public class InteractionUI : MonoBehaviour
    {
        [Header("--- THÀNH PHẦN UI ---")]
        [SerializeField] private TextMeshProUGUI promptText; // Khung chữ hiển thị hướng dẫn
        [SerializeField] private GameObject crosshair;      // Hình ảnh tâm ngắm ở giữa màn hình

        /// <summary>
        /// Cập nhật văn bản gợi ý lên màn hình.
        /// Pass chuỗi rỗng "" hoặc null để ẩn gợi ý.
        /// </summary>
        public void ShowPrompt(string message)
        {
            if (promptText == null) return;

            if (string.IsNullOrEmpty(message))
            {
                promptText.gameObject.SetActive(false);
            }
            else
            {
                promptText.text = message;
                promptText.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Bật/Tắt tâm ngắm khi cần (VD: Mở menu thì ẩn tâm ngắm).
        /// </summary>
        public void SetCrosshairActive(bool active)
        {
            if (crosshair != null)
            {
                crosshair.SetActive(active);
            }
        }
    }
}