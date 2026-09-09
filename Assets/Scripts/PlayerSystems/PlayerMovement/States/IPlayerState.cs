using UnityEngine;

/// <summary>
/// Base interface for all player states in the FSM.
/// Each state handles its own Enter, Update, and Exit logic.
/// </summary>
public interface IPlayerState
{
    /// <summary>
    /// Called when the state is entered. Use this to initialize state-specific setup.
    /// </summary>
    void Enter();

    /// <summary>
    /// Called every frame while this state is active.
    /// </summary>
    void Update();

    /// <summary>
    /// Called when transitioning away from this state.
    /// Use this to clean up and reset values.
    /// </summary>
    void Exit();
}
