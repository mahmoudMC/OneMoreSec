using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controls player movement: walk (WASD), run (Shift), crouch (hold C), jump (Space)
/// and oxygen recharge (hold E). Uses CharacterController and the Unity Input System.
/// Recharge integrates with IRechargeService (StartRecharge / CancelRecharge).
/// Coordinates with PlayerStateManager for state-based movement restrictions.
/// Movement speeds, jump height, and gravity are managed by IPlayerAttributes service.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
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
    private bool runningEnabled = true;

    // Input System actions
    private InputAction moveAction;
    private InputAction runAction;
    private InputAction jumpAction;
    private InputAction crouchAction;
    private InputAction chargeAction;

    // Recharge integration
    private IRechargeService rechargeService;
    private bool isCharging;
    private PlayerStateManager stateManager;
    private IPlayerAttributes playerAttributes;
    private Animator animator;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        stateManager = GetComponent<PlayerStateManager>();
        playerAttributes = GetComponent<IPlayerAttributes>();
        animator = GetComponent<Animator>();

        // Auto-create PlayerAttributesService if it doesn't exist
        if (playerAttributes == null)
        {
            Debug.LogWarning("PlayerMovement: PlayerAttributesService not found. Creating one automatically.", this);
            gameObject.AddComponent<PlayerAttributesService>();
            playerAttributes = GetComponent<IPlayerAttributes>();
        }

        if (playerAttributes == null)
        {
            Debug.LogError("PlayerMovement: Failed to create or find IPlayerAttributes service!", this);
            return;
        }

        standHeight = controller.height;
        standCenter = controller.center;

        crouchTargetHeight = Mathf.Min(playerAttributes.CrouchHeight, standHeight);
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
                if (stateManager != null)
                    stateManager.ExitChargeState();
            }
        }

        if (chargeInput && !isCharging)
        {
            // begin recharge
            StartRecharge();
            if (stateManager != null)
                stateManager.EnterChargeState();
        }

        if (!chargeInput && isCharging)
        {
            // released charge
            CancelRecharge();
            if (stateManager != null)
                stateManager.ExitChargeState();
        }

        // compute movement only if movement is enabled and not charging
        Vector3 motion = Vector3.zero;

        if (movementEnabled && !isCharging)
        {
            Vector3 forward = transform.forward;
            Vector3 right = transform.right;

            Vector3 desired = (right * moveInput.x + forward * moveInput.y).normalized;

            float speed = playerAttributes.WalkSpeed;
            if (runInput && runningEnabled) speed = playerAttributes.RunSpeed;
            if (crouchInput) speed *= playerAttributes.CrouchSpeedMultiplier;

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
                verticalVelocity = Mathf.Sqrt(2f * playerAttributes.JumpHeight * -playerAttributes.Gravity);
            }
        }
        else
        {
            verticalVelocity += playerAttributes.Gravity * Time.deltaTime;
        }

        Vector3 finalMotion = motion + Vector3.up * verticalVelocity;
        controller.Move(finalMotion * Time.deltaTime);

        // Update animator parameters for FPS animations
        if (animator != null)
        {
            // Basic movement states
            animator.SetBool("isMoving", moveInput.sqrMagnitude > 0.01f);
            animator.SetBool("isRunning", runInput && moveInput.sqrMagnitude > 0.01f);
            animator.SetBool("isCrouching", crouchInput);
            animator.SetFloat("verticalVelocity", verticalVelocity);
            
            // FPS directional movement (for arm blending)
            // moveY: forward/backward (-1 = back, 0 = idle, 1 = forward)
            // moveX: left/right strafe (-1 = left, 0 = center, 1 = right)
            // moveSpeed: 0-1 normalized magnitude for animation blending
            animator.SetFloat("moveX", moveInput.x);
            animator.SetFloat("moveY", moveInput.y);
            animator.SetFloat("moveSpeed", moveInput.magnitude);
        }

        // crouch height transition
        float targetH = crouchInput ? crouchTargetHeight : standHeight;
        crouchLerp = Mathf.MoveTowards(crouchLerp, 1f, Time.deltaTime / playerAttributes.CrouchTransitionTime);
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

    /// <summary>
    /// Enable/disable running. Used by AimState to prevent running while aiming.
    /// </summary>
    public void SetRunningEnabled(bool enabled)
    {
        runningEnabled = enabled;
    }
}
