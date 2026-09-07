using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controls player movement: walk (WASD), run (Shift), crouch (hold C), jump (Space)
/// and oxygen recharge (hold E). Uses CharacterController and the Unity Input System.
/// Recharge integrates with IRechargeService (StartRecharge / CancelRecharge).
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Speeds")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float runSpeed = 7f;
    [SerializeField] [Range(0f, 1f)] private float crouchSpeedMultiplier = 0.5f;

    [Header("Jump / Gravity")]
    [SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private float gravity = -20f;

    [Header("Crouch")]
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float crouchTransitionTime = 0.12f;

    [Header("General")]
    [SerializeField] private bool enableAirControl = true;

    private CharacterController controller;
    private Vector2 moveInput;
    private bool runInput;
    private bool jumpInput;
    private bool crouchInput;
    private bool chargeInput;

    private float verticalVelocity;
    private float standHeight;
    private Vector3 standCenter;
    private float crouchTargetHeight;
    private Vector3 crouchTargetCenter;
    private float crouchLerp;

    private bool movementEnabled = true;

    // Input System actions
    private InputAction moveAction;
    private InputAction runAction;
    private InputAction jumpAction;
    private InputAction crouchAction;
    private InputAction chargeAction;

    // Recharge integration
    private IRechargeService rechargeService;
    private bool isCharging;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        standHeight = controller.height;
        standCenter = controller.center;

        crouchTargetHeight = Mathf.Min(crouchHeight, standHeight);
        crouchTargetCenter = new Vector3(standCenter.x, standCenter.y - (standHeight - crouchTargetHeight) / 2f, standCenter.z);

        // create simple InputActions at runtime so this script works without a PlayerInput asset
        moveAction = new InputAction("Move", InputActionType.Value);
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");
        moveAction.AddBinding("<Gamepad>/leftStick");

        runAction = new InputAction("Run", InputActionType.Button);
        runAction.AddBinding("<Keyboard>/leftShift");
        runAction.AddBinding("<Gamepad>/leftStickPress");

        jumpAction = new InputAction("Jump", InputActionType.Button);
        jumpAction.AddBinding("<Keyboard>/space");
        jumpAction.AddBinding("<Gamepad>/buttonSouth");

        crouchAction = new InputAction("Crouch", InputActionType.Button);
        crouchAction.AddBinding("<Keyboard>/c");
        crouchAction.AddBinding("<Gamepad>/buttonEast");

        chargeAction = new InputAction("Charge", InputActionType.Button);
        chargeAction.AddBinding("<Keyboard>/e");
        chargeAction.AddBinding("<Gamepad>/buttonWest");

        rechargeService = GetComponent<IRechargeService>();
    }

    private void OnEnable()
    {
        moveAction.Enable();
        runAction.Enable();
        jumpAction.Enable();
        crouchAction.Enable();
        chargeAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        runAction.Disable();
        jumpAction.Disable();
        crouchAction.Disable();
        chargeAction.Disable();
    }

    private void Update()
    {
        // read inputs
        moveInput = moveAction.ReadValue<Vector2>();
        runInput = runAction.ReadValue<float>() > 0.5f;
        jumpInput = jumpAction.triggered;
        crouchInput = crouchAction.ReadValue<float>() > 0.5f;
        chargeInput = chargeAction.ReadValue<float>() > 0.5f;

        // handle charge start/cancel
        float moveMagnitude = moveInput.sqrMagnitude;

        if (isCharging)
        {
            // if player moves while charging -> cancel
            if (moveMagnitude > 0.01f)
            {
                CancelRecharge();
            }
        }

        if (chargeInput && !isCharging)
        {
            // begin recharge
            StartRecharge();
        }

        if (!chargeInput && isCharging)
        {
            // released charge
            CancelRecharge();
        }

        // compute movement only if movement is enabled and not charging
        Vector3 motion = Vector3.zero;

        if (movementEnabled && !isCharging)
        {
            Vector3 forward = transform.forward;
            Vector3 right = transform.right;

            Vector3 desired = (right * moveInput.x + forward * moveInput.y).normalized;

            float speed = walkSpeed;
            if (runInput) speed = runSpeed;
            if (crouchInput) speed *= crouchSpeedMultiplier;

            // allow reduced control in air if desired
            if (!controller.isGrounded && !enableAirControl)
            {
                // minimal air control: keep horizontal velocity
            }
            else
            {
                motion = desired * speed;
            }
        }

        // handle jump / gravity
        if (controller.isGrounded)
        {
            if (verticalVelocity < 0f)
                verticalVelocity = -2f; // small downward force to keep grounded

            if (jumpInput && movementEnabled && !isCharging)
            {
                verticalVelocity = Mathf.Sqrt(2f * jumpHeight * -gravity);
            }
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        Vector3 finalMotion = motion + Vector3.up * verticalVelocity;
        controller.Move(finalMotion * Time.deltaTime);

        // crouch height transition
        float targetH = crouchInput ? crouchTargetHeight : standHeight;
        crouchLerp = Mathf.MoveTowards(crouchLerp, 1f, Time.deltaTime / crouchTransitionTime);
        float t = Mathf.Lerp(0f, 1f, crouchLerp);

        // lerp height and center smoothly
        controller.height = Mathf.Lerp(controller.height, targetH, t);
        controller.center = Vector3.Lerp(controller.center, crouchInput ? crouchTargetCenter : standCenter, t);

        // reset lerp when target changes
        if ((crouchInput && controller.height <= crouchTargetHeight + 0.001f) || (!crouchInput && controller.height >= standHeight - 0.001f))
            crouchLerp = 0f;
    }

    private void StartRecharge()
    {
        if (rechargeService != null)
        {
            rechargeService.StartRecharge();
            isCharging = true;
        }
    }

    private void CancelRecharge()
    {
        if (rechargeService != null && isCharging)
        {
            rechargeService.CancelRecharge();
        }

        isCharging = false;
    }

    /// <summary>
    /// Enable/disable player movement externally.
    /// </summary>
    public void SetMovementEnabled(bool enabled)
    {
        movementEnabled = enabled;
    }
}
