using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Keeps a camera at the player's eye level and provides first-person look controls.
/// Horizontal look rotates the player, while vertical look only rotates the camera.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class FirstPersonCamera : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;

    [Header("Look")]
    [SerializeField] private float mouseSensitivity = 0.08f;
    [SerializeField] private float gamepadSensitivity = 180f;
    [SerializeField] private bool invertVerticalLook;
    [SerializeField] private float minPitch = -85f;
    [SerializeField] private float maxPitch = 85f;

    [Header("Camera Position")]
    [Tooltip("Distance below the top of the CharacterController capsule.")]
    [SerializeField] private float eyeOffsetFromTop = 0.12f;
    [SerializeField] private float cameraHeightSmoothTime = 0.06f;
    [SerializeField] private bool hidePlayerRenderers = true;

    private CharacterController controller;
    private InputAction mouseLookAction;
    private InputAction gamepadLookAction;
    private float pitch;
    private float heightVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (playerCamera == null)
            playerCamera = Camera.main;

        if (playerCamera == null)
        {
            Debug.LogError("FirstPersonCamera needs a camera tagged MainCamera.", this);
            enabled = false;
            return;
        }

        mouseLookAction = new InputAction("Mouse Look", InputActionType.Value, "<Mouse>/delta");
        gamepadLookAction = new InputAction("Gamepad Look", InputActionType.Value, "<Gamepad>/rightStick");

        Vector3 currentAngles = playerCamera.transform.eulerAngles;
        pitch = NormalizeAngle(currentAngles.x);

        if (hidePlayerRenderers)
        {
            foreach (Renderer playerRenderer in GetComponentsInChildren<Renderer>())
                playerRenderer.enabled = false;
        }
    }

    private void OnEnable()
    {
        mouseLookAction?.Enable();
        gamepadLookAction?.Enable();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        mouseLookAction?.Disable();
        gamepadLookAction?.Disable();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (Cursor.lockState != CursorLockMode.Locked)
            return;

        Vector2 mouseLook = mouseLookAction.ReadValue<Vector2>() * mouseSensitivity;
        Vector2 gamepadLook = gamepadLookAction.ReadValue<Vector2>() * (gamepadSensitivity * Time.deltaTime);
        Vector2 look = mouseLook + gamepadLook;

        float verticalMultiplier = invertVerticalLook ? 1f : -1f;
        pitch = Mathf.Clamp(pitch + look.y * verticalMultiplier, minPitch, maxPitch);
        transform.Rotate(Vector3.up * look.x);
        playerCamera.transform.rotation = Quaternion.Euler(pitch, transform.eulerAngles.y, 0f);
    }

    private void LateUpdate()
    {
        if (playerCamera == null)
            return;

        float targetHeight = controller.center.y + controller.height * 0.5f - eyeOffsetFromTop;
        float smoothHeight = Mathf.SmoothDamp(
            playerCamera.transform.position.y,
            transform.TransformPoint(new Vector3(0f, targetHeight, 0f)).y,
            ref heightVelocity,
            cameraHeightSmoothTime);

        Vector3 cameraPosition = transform.TransformPoint(new Vector3(0f, targetHeight, 0f));
        cameraPosition.y = smoothHeight;
        playerCamera.transform.position = cameraPosition;
    }

    private static float NormalizeAngle(float angle)
    {
        return angle > 180f ? angle - 360f : angle;
    }
}
