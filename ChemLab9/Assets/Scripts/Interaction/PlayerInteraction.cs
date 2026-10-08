using UnityEngine;
using UnityEngine.EventSystems;
using ChemLab.Interaction;

namespace ChemLab.Interaction
{
    /// <summary>
    /// Xử lý việc bấm E để nhặt/đặt vật thể và Nhấp chuột trái để sử dụng dụng cụ.
    /// </summary>
    public class PlayerInteraction : MonoBehaviour
    {
        [Header("--- CẤU HÌNH RAYCAST ---")]
        [SerializeField] private Camera playerCamera;
        [SerializeField] private float interactDistance = 3.0f;
        [SerializeField] private LayerMask interactableLayer;
        [SerializeField] private Transform handPosition;

        [Header("--- TRẠNG THÁI HIỆN TẠI ---")]
        private IInteractable currentTarget;
        private IInteractable heldItem;

        private void Update()
        {
            if (Time.timeScale == 0f || (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())) return;
            CheckForInteractable();
            HandleInput();
        }

        private void CheckForInteractable()
        {
            if (playerCamera == null) return;

            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactableLayer))
            {
                IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
                if (interactable != null)
                {
                    currentTarget = interactable;
                    // In gợi ý ra Console (Sau này nối vào UI Canvas)
                    // Debug.Log(currentTarget.GetInteractPrompt());
                    return;
                }
            }

            currentTarget = null;
        }

        private void HandleInput()
        {
            // Phím E: Cầm hoặc Đặt vật thể
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (heldItem != null)
                {
                    // Nếu đang cầm vật thể -> Bấm E để thả ra
                    heldItem.Interact(handPosition);
                    heldItem = null;
                }
                else if (currentTarget != null)
                {
                    // Nếu nhìn vào vật thể và chưa cầm gì -> Bấm E để nhặt
                    heldItem = currentTarget;
                    heldItem.Interact(handPosition);
                }
            }

            // Chuột trái (Mouse 0): Rót dung dịch / Kích hoạt hành động khi đang cầm đồ
            if (Input.GetMouseButtonDown(0) && heldItem != null)
            {
                heldItem.UseAction();
            }
        }
    }
}