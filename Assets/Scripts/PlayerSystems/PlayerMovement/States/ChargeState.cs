using UnityEngine;

/// <summary>
/// Charge state: player is charging oxygen.
/// Movement is disabled, camera can still look around, shader applied for visual feedback.
/// Transitions back to Normal when E is released or when player tries to move.
/// </summary>
public class ChargeState : IPlayerState
{
    private PlayerStateManager stateManager;
    private PlayerMovement playerMovement;
    private FirstPersonCamera firstPersonCamera;
    private Animator animator;

    [SerializeField] private Material chargeShader; // Assign in inspector or set dynamically

    public ChargeState(PlayerStateManager stateManager, PlayerMovement playerMovement, FirstPersonCamera firstPersonCamera, Animator animator)
    {
        this.stateManager = stateManager;
        this.playerMovement = playerMovement;
        this.firstPersonCamera = firstPersonCamera;
        this.animator = animator;
    }

    public void Enter()
    {
        // Set animator parameter
        animator.SetBool("isCharging", true);
        animator.SetBool("isAiming", false);
        animator.SetBool("isFrozen", false);

        // Disable movement but allow camera control
        playerMovement.SetMovementEnabled(false);
        firstPersonCamera.enabled = true;

        // Apply charge shader (optional for now)
        ApplyChargeShader();
    }

    public void Update()
    {
        // Charge state is mostly passive - it transitions back via PlayerMovement/StateManager
        // when E is released or on movement attempt
    }

    public void Exit()
    {
        // Re-enable movement
        playerMovement.SetMovementEnabled(true);

        // Remove charge shader
        RemoveChargeShader();
    }

    private void ApplyChargeShader()
    {
        // TODO: Apply shader to player character or screen for visual feedback
        // For now, this is a placeholder for later implementation
        Debug.Log("Charge shader applied (placeholder)");
    }

    private void RemoveChargeShader()
    {
        // TODO: Remove charge shader
        Debug.Log("Charge shader removed (placeholder)");
    }
}
