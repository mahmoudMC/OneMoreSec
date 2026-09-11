using UnityEngine;

/// <summary>
/// Frozen state: player spawned but game hasn't started yet.
/// Player can look around but cannot move, run, jump, aim, or charge.
/// Can only transition via Paused state or external SetFrozen(false).
/// </summary>
public class FrozenState : IPlayerState
{
    private PlayerStateManager stateManager;
    private PlayerMovement playerMovement;
    private FirstPersonCamera firstPersonCamera;
    private Animator animator;

    public FrozenState(PlayerStateManager stateManager, PlayerMovement playerMovement, FirstPersonCamera firstPersonCamera, Animator animator)
    {
        this.stateManager = stateManager;
        this.playerMovement = playerMovement;
        this.firstPersonCamera = firstPersonCamera;
        this.animator = animator;
    }

    public void Enter()
    {
        // Set animator parameter
        animator.SetBool("isFrozen", true);
        animator.SetBool("isCharging", false);
        animator.SetBool("isAiming", false);

        // Disable all movement
        playerMovement.SetMovementEnabled(false);
        playerMovement.enabled = false;

        // Allow camera control (player can look around)
        firstPersonCamera.enabled = true;

        Debug.Log("Player in Frozen state - game not yet started");
    }

    public void Update()
    {
        // Frozen state is passive - only responds to pause input or external SetFrozen call
    }

    public void Exit()
    {
        // Re-enable movement
        playerMovement.enabled = true;
        playerMovement.SetMovementEnabled(true);
    }
}
