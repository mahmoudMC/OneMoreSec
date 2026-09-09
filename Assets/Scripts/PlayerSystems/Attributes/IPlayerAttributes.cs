/// <summary>
/// Defines all player attributes that can be modified or upgraded.
/// Other systems access player stats through this interface without needing to reference PlayerMovement directly.
/// </summary>
public interface IPlayerAttributes
{
    #region Movement Speeds
    float WalkSpeed { get; }
    float RunSpeed { get; }
    float CrouchSpeedMultiplier { get; }
    #endregion

    #region Jump & Gravity
    float JumpHeight { get; }
    float Gravity { get; }
    #endregion

    #region Crouch
    float CrouchHeight { get; }
    float CrouchTransitionTime { get; }
    #endregion

    #region Combat
    float BaseDamage { get; }
    float DamageMultiplier { get; }
    #endregion

    #region General
    float MaxHealth { get; }
    float MaxOxygen { get; }
    #endregion

    #region Modifier Methods
    /// <summary>
    /// Apply a modifier to an attribute. Modifiers stack multiplicatively.
    /// Example: ApplyModifier("walkSpeed", 1.5f) increases walk speed by 50%
    /// Pass a modifierKey to identify and remove it later.
    /// </summary>
    void ApplyModifier(string attributeName, float modifier, string modifierKey = "");

    /// <summary>
    /// Remove a modifier by key. Returns true if modifier was found and removed.
    /// </summary>
    bool RemoveModifier(string attributeName, string modifierKey);

    /// <summary>
    /// Get total modifier multiplier for an attribute (product of all modifiers)
    /// </summary>
    float GetTotalModifier(string attributeName);

    /// <summary>
    /// Reset all attributes to base values and clear all modifiers.
    /// </summary>
    void ResetToDefaults();

    /// <summary>
    /// Get the base value of an attribute before modifiers.
    /// </summary>
    float GetBaseValue(string attributeName);
    #endregion
}
