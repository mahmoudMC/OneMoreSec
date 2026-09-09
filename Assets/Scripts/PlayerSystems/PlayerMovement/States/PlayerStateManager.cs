using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Manages the player's Finite State Machine for gameplay states:
/// Normal, Aim, Charge, Frozen, and Paused.
/// Handles state transitions based on input and coordinates with PlayerMovement and FirstPersonCamera.
/// </summary>
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(FirstPersonCamera))]
[RequireComponent(typeof(Animator))]
public class PlayerStateManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;

    [Header("Input")]
    [SerializeField] private bool useGamepadAim = true;

    [Header("Frozen State")]
    [SerializeField] private float frozenDuration = 3f; // Auto-transition from Frozen to Normal after 3 seconds

    private PlayerMovement playerMovement;
    private FirstPersonCamera firstPersonCamera;
    
    private IPlayerState currentState;
    private IPlayerState nextState;

    // State references
    private NormalState normalState;
    private AimState aimState;
    private ChargeState chargeState;
    private FrozenState frozenState;
    private PausedState pausedState;

    // Input tracking
    private InputAction aimAction;
    private InputAction pauseAction;
    private Vector2 moveInput;
    private bool runInput;
    private bool jumpInput;
    private bool crouchInput;
    private bool chargeInput;

    private float frozenTimer;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        firstPersonCamera = GetComponent<FirstPersonCamera>();

        if (animator == null)
            animator = GetComponent<Animator>();

        // Initialize all states
        normalState = new NormalState(this, playerMovement, firstPersonCamera, animator);
        aimState = new AimState(this, playerMovement, firstPersonCamera, animator);
        chargeState = new ChargeState(this, playerMovement, firstPersonCamera, animator);
        frozenState = new FrozenState(this, playerMovement, firstPersonCamera, animator);
        pausedState = new PausedState(this, playerMovement, firstPersonCamera, animator);

        // Create input actions
        aimAction = new InputAction("Aim", InputActionType.Button);
        aimAction.AddBinding("<Mouse>/rightButton");
        if (useGamepadAim)
            aimAction.AddBinding("<Gamepad>/rightTrigger");

        pauseAction = new InputAction("Pause", InputActionType.Button);
        pauseAction.AddBinding("<Keyboard>/escape");

        // Start in Frozen state
        ChangeState(frozenState);
    }

    private void OnEnable()
    {
        aimAction?.Enable();
        pauseAction?.Enable();
    }

    private void OnDisable()
    {
        aimAction?.Disable();
        pauseAction?.Disable();
    }

    private void Update()
    {
        // Auto-transition from Frozen to Normal after 3 seconds
        if (currentState == frozenState)
        {
            frozenTimer += Time.deltaTime;
            if (frozenTimer >= frozenDuration)
            {
                ChangeState(normalState);
                frozenTimer = 0f;
            }
        }

        HandleStateTransitions();
        currentState?.Update();
    }

    /// <summary>
    /// Handles transitions between states based on input.
    /// </summary>
    private void HandleStateTransitions()
    {
        bool aimPressed = aimAction.triggered;
        bool pausePressed = pauseAction.triggered;

        // Always allow pause toggle
        if (pausePressed)
        {
            if (currentState == pausedState)
            {
                ChangeState(normalState);
            }
            else
            {
                ChangeState(pausedState);
            }
            return;
        }

        // Paused state blocks other transitions
        if (currentState == pausedState)
            return;

        // Frozen state: can only unpause
        if (currentState == frozenState)
            return;

        // Normal → Aim, Charge, or stay
        if (currentState == normalState)
        {
            if (aimPressed)
                ChangeState(aimState);
        }

        // Aim → Normal or back to Aim
        if (currentState == aimState)
        {
            if (aimPressed)
                ChangeState(normalState);
        }

        // Charge state is managed internally by ChargeState
        // It transitions back to Normal when E is released or on movement
    }

    /// <summary>
    /// Changes to a new state. Called by states or externally.
    /// </summary>
    public void ChangeState(IPlayerState newState)
    {
        if (newState == currentState)
            return;

        currentState?.Exit();
        currentState = newState;
        currentState.Enter();

        // Reset frozen timer when entering frozen state
        if (newState == frozenState)
            frozenTimer = 0f;
    }

    /// <summary>
    /// Transitions to Charge state. Can be called by external systems.
    /// </summary>
    public void EnterChargeState()
    {
        if (currentState != pausedState && currentState != frozenState)
        {
            ChangeState(chargeState);
        }
    }

    /// <summary>
    /// Transitions back to Normal state from Charge. Called by ChargeState.
    /// </summary>
    public void ExitChargeState()
    {
        if (currentState == chargeState)
        {
            ChangeState(normalState);
        }
    }

    /// <summary>
    /// Allows external systems to transition to Frozen state (e.g., at game start).
    /// </summary>
    public void SetFrozen(bool frozen)
    {
        if (frozen && currentState != frozenState)
        {
            ChangeState(frozenState);
        }
        else if (!frozen && currentState == frozenState)
        {
            ChangeState(normalState);
        }
    }

    // Getters for state queries
    public bool IsNormal => currentState == normalState;
    public bool IsAiming => currentState == aimState;
    public bool IsCharging => currentState == chargeState;
    public bool IsFrozen => currentState == frozenState;
    public bool IsPaused => currentState == pausedState;
}
