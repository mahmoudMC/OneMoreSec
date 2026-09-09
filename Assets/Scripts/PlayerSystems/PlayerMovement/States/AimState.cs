using UnityEngine;

/// <summary>
/// Aim state: player can look and walk, but NOT run, jump, or charge.
/// Used when aiming weapons (though weapon system is not yet implemented).
/// </summary>
public class AimState : IPlayerState
{
    private PlayerStateManager stateManager;
    private PlayerMovement playerMovement;
    private FirstPersonCamera firstPersonCamera;
    private Animator animator;

    public AimState(PlayerStateManager stateManager, PlayerMovement playerMovement, FirstPersonCamera firstPersonCamera, Animator animator)
    {
        this.stateManager = stateManager;
        this.playerMovement = playerMovement;
        this.firstPersonCamera = firstPersonCamera;
        this.animator = animator;
    }

    public void Enter()
    {
        // Set animator parameter
        animator.SetBool("isAiming", true);
        animator.SetBool("isCharging", false);
        animator.SetBool("isFrozen", false);

        // Enable movement and camera, but movement will be restricted to walk speed
        playerMovement.enabled = true;
        firstPersonCamera.enabled = true;

        // Disable running in PlayerMovement (we'll add a method for this)
        playerMovement.SetRunningEnabled(false);
    }

    public void Update()
    {
        // Animator updates handled by PlayerMovement
    }

    public void Exit()
    {
        // Re-enable running when leaving aim state
        playerMovement.SetRunningEnabled(true);
    }
}
