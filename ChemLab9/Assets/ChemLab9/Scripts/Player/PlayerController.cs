using UnityEngine;

/// <summary>
/// Điều khiển di chuyển di chuyển WASD và xoay góc nhìn camera theo chuột.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Mouse Look Settings")]
    [SerializeField] private float mouseSensitivity = 2.0f;
    [SerializeField] private float topClamp = 80.0f;
    [SerializeField] private float bottomClamp = -80.0f;

    [Header("References")]
    [SerializeField] private Transform cameraHolder;

    private CharacterController controller;
    private Vector3 velocity;
    private float cameraPitch = 0.0f;
    private bool controlEnabled = true;

    public void SetControl(bool enabled)
    {
        controlEnabled = enabled;
        Cursor.lockState = enabled ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !enabled;
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        // Khóa con trỏ chuột vào giữa màn hình khi chơi
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cameraHolder == null)
        {
            Camera ownCamera = GetComponentInChildren<Camera>();
            if (ownCamera != null) cameraHolder = ownCamera.transform;
        }
    }

    private void Update()
    {
        if (!controlEnabled || Time.timeScale == 0f) return;
        mouseSensitivity = PlayerPrefs.GetFloat("ChemLab9.MouseSensitivity", 2f);
        HandleLook();
        HandleMove();
    }

    private void HandleLook()
    {
        if (cameraHolder == null) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Xoay Camera lên/xuống (Pitch)
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, bottomClamp, topClamp);
        cameraHolder.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);

        // Xoay thân Player trái/phải (Yaw)
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleMove()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = Vector3.ClampMagnitude(transform.right * moveX + transform.forward * moveZ, 1f);
        controller.Move(move * moveSpeed * Time.deltaTime);

        // Xử lý trọng lực cơ bản
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
