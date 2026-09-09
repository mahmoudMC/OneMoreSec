using UnityEngine;

/// <summary>
/// Normal gameplay state: player can move, run, aim, jump, crouch, and charge.
/// Camera and movement work at full capacity.
/// </summary>
public class NormalState : IPlayerState
{
    private PlayerStateManager stateManager;
    private PlayerMovement playerMovement;
    private FirstPersonCamera firstPersonCamera;
    private Animator animator;

    public NormalState(PlayerStateManager stateManager, PlayerMovement playerMovement, FirstPersonCamera firstPersonCamera, Animator animator)
    {
        this.stateManager = stateManager;
        this.playerMovement = playerMovement;
        this.firstPersonCamera = firstPersonCamera;
        this.animator = animator;
    }

    public void Enter()
    {
        // Reset animator parameters
        animator.SetBool("isAiming", false);
        animator.SetBool("isCharging", false);
        animator.SetBool("isFrozen", false);

        // Enable full movement and camera control
        playerMovement.enabled = true;
        firstPersonCamera.enabled = true;
    }

    public void Update()
    {
        // Update animator based on movement state
        if (animator != null)
        {
            // These will be set by PlayerMovement's input reading
            // (We'll add animator parameter updates to PlayerMovement)
        }
    }

    public void Exit()
    {
        // Clean up if needed
    }
}
