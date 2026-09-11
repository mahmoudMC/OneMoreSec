using UnityEngine;

/// <summary>
/// Paused state: game is paused.
/// All movement and camera control disabled, cursor is visible and unlocked.
/// Player can interact with pause menu or press ESC to resume.
/// </summary>
public class PausedState : IPlayerState
{
    private PlayerStateManager stateManager;
    private PlayerMovement playerMovement;
    private FirstPersonCamera firstPersonCamera;
    private Animator animator;

    public PausedState(PlayerStateManager stateManager, PlayerMovement playerMovement, FirstPersonCamera firstPersonCamera, Animator animator)
    {
        this.stateManager = stateManager;
        this.playerMovement = playerMovement;
        this.firstPersonCamera = firstPersonCamera;
        this.animator = animator;
    }

    public void Enter()
    {
        // Disable all movement and camera
        playerMovement.SetMovementEnabled(false);
        playerMovement.enabled = false;
        firstPersonCamera.enabled = false;

        // Pause animations
        if (animator != null)
            animator.speed = 0f;

        // Unlock cursor for menu interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Game Paused");
    }

    public void Update()
    {
        // Paused state is mostly passive - only responds to pause input (ESC)
    }

    public void Exit()
    {
        // Resume animations
        if (animator != null)
            animator.speed = 1f;

        // Re-enable movement and camera
        playerMovement.enabled = true;
        playerMovement.SetMovementEnabled(true);
        firstPersonCamera.enabled = true;

        // Lock cursor again
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("Game Resumed");
    }
}
