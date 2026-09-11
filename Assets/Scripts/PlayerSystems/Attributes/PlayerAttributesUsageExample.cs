using UnityEngine;

/// <summary>
/// EXAMPLE: How to use PlayerAttributesService to modify player attributes
/// This shows common patterns for upgrades, temporary effects, and stat modifications.
/// </summary>
public class PlayerAttributesUsageExample : MonoBehaviour
{
    private IPlayerAttributes playerAttributes;

    private void Start()
    {
        // Get reference to the attributes service
        playerAttributes = GetComponent<IPlayerAttributes>();
        
        // Or if this script is on a different GameObject:
        // playerAttributes = FindObjectOfType<PlayerAttributesService>();
        // Or from another GameObject:
        // playerAttributes = playerGameObject.GetComponent<IPlayerAttributes>();
    }

    #region EXAMPLE 1: Apply Permanent Speed Boost
    /// <summary>
    /// Example: Speed Upgrade permanently increases walk/run speeds
    /// </summary>
    private void ApplySpeedUpgrade()
    {
        // Apply 1.2x multiplier to walkSpeed (20% faster)
        playerAttributes.ApplyModifier("walkSpeed", 1.2f, "speedUpgrade_001");
        playerAttributes.ApplyModifier("runSpeed", 1.2f, "speedUpgrade_001");
        
        Debug.Log($"Walk Speed: {playerAttributes.GetBaseValue("walkSpeed")} → {playerAttributes.WalkSpeed}");
    }
    #endregion

    #region EXAMPLE 2: Apply Temporary Effect
    /// <summary>
    /// Example: Temporary speed boost that wears off after 5 seconds
    /// </summary>
    private void ApplyTemporarySpeedBoost()
    {
        string boostKey = "tempSpeedBoost_" + Time.time;
        
        playerAttributes.ApplyModifier("walkSpeed", 1.5f, boostKey);
        playerAttributes.ApplyModifier("runSpeed", 1.5f, boostKey);
        
        // Remove the boost after 5 seconds
        Invoke(nameof(RemoveSpeedBoost), 5f);
    }

    private void RemoveSpeedBoost()
    {
        playerAttributes.RemoveModifier("walkSpeed", "tempSpeedBoost");
        playerAttributes.RemoveModifier("runSpeed", "tempSpeedBoost");
    }
    #endregion

    #region EXAMPLE 3: Damage Upgrade
    /// <summary>
    /// Example: Damage Boost upgrade increases damage by 25%
    /// </summary>
    private void ApplyDamageUpgrade()
    {
        playerAttributes.ApplyModifier("baseDamage", 1.25f, "damageUpgrade_001");
        Debug.Log($"Base Damage: {playerAttributes.BaseDamage}");
    }
    #endregion

    #region EXAMPLE 4: Stack Multiple Modifiers
    /// <summary>
    /// Example: Multiple upgrades stack multiplicatively
    /// If you have: 1.2x speed + 1.1x speed = 1.32x total
    /// </summary>
    private void StackMultipleUpgrades()
    {
        playerAttributes.ApplyModifier("walkSpeed", 1.2f, "speedUpgrade_1");
        playerAttributes.ApplyModifier("walkSpeed", 1.1f, "speedUpgrade_2");
        playerAttributes.ApplyModifier("walkSpeed", 1.05f, "speedUpgrade_3");
        
        // Total: 1.2 * 1.1 * 1.05 = 1.386x
        Debug.Log($"Total Speed Multiplier: {playerAttributes.GetTotalModifier("walkSpeed")}x");
        Debug.Log($"Final Walk Speed: {playerAttributes.WalkSpeed}");
    }
    #endregion

    #region EXAMPLE 5: Remove Specific Modifier
    /// <summary>
    /// Example: Remove a specific upgrade
    /// </summary>
    private void RemoveUpgrade(string upgradeName)
    {
        bool success = playerAttributes.RemoveModifier("walkSpeed", upgradeName);
        if (success)
            Debug.Log($"Removed {upgradeName}");
        else
            Debug.LogWarning($"Could not find {upgradeName}");
    }
    #endregion

    #region EXAMPLE 6: Jump Height Modification (for future abilities)
    /// <summary>
    /// Example: Ability grants higher jump
    /// </summary>
    private void GrantJumpBoost()
    {
        playerAttributes.ApplyModifier("jumpHeight", 1.5f, "jumpAbility_enhanced");
        playerAttributes.ApplyModifier("gravity", 0.9f, "jumpAbility_enhanced"); // Slightly reduce gravity
    }
    #endregion

    #region EXAMPLE 7: Debug - Print All Modifiers
    /// <summary>
    /// Example: Debug method to see all active modifiers
    /// </summary>
    private void DebugPrintAllModifiers()
    {
        if (playerAttributes is PlayerAttributesService service)
        {
            service.PrintModifiers();
        }
    }
    #endregion

    #region EXAMPLE 8: From Upgrade System (Real-world pattern)
    /// <summary>
    /// How an Upgrade would apply changes
    /// </summary>
    public void OnUpgradeActivated(string upgradeType, float modifier)
    {
        switch (upgradeType)
        {
            case "SpeedBoost":
                playerAttributes.ApplyModifier("walkSpeed", modifier, upgradeType);
                playerAttributes.ApplyModifier("runSpeed", modifier, upgradeType);
                break;

            case "DamageBoost":
                playerAttributes.ApplyModifier("baseDamage", modifier, upgradeType);
                break;

            case "JumpBoost":
                playerAttributes.ApplyModifier("jumpHeight", modifier, upgradeType);
                break;

            default:
                Debug.LogWarning($"Unknown upgrade type: {upgradeType}");
                break;
        }

        Debug.Log($"Applied {upgradeType} with {modifier}x modifier");
    }
    #endregion
}

/*
 * QUICK REFERENCE - How to Use PlayerAttributesService
 * 
 * 1. GET REFERENCE:
 *    IPlayerAttributes attrs = GetComponent<IPlayerAttributes>();
 * 
 * 2. APPLY MODIFIER (multiplies base value):
 *    attrs.ApplyModifier("walkSpeed", 1.5f, "myUpgrade_001");
 *    // Now walkSpeed = 4 * 1.5 = 6
 * 
 * 3. READ CURRENT VALUE (includes all modifiers):
 *    float currentSpeed = attrs.WalkSpeed;
 * 
 * 4. READ BASE VALUE (without modifiers):
 *    float baseSpeed = attrs.GetBaseValue("walkSpeed");
 * 
 * 5. GET TOTAL MULTIPLIER:
 *    float totalMod = attrs.GetTotalModifier("walkSpeed");
 * 
 * 6. REMOVE MODIFIER:
 *    attrs.RemoveModifier("walkSpeed", "myUpgrade_001");
 * 
 * 7. RESET ALL:
 *    attrs.ResetToDefaults();
 * 
 * ATTRIBUTE NAMES (use these strings exactly):
 * - walkSpeed
 * - runSpeed
 * - crouchSpeedMultiplier
 * - jumpHeight
 * - gravity
 * - crouchHeight
 * - crouchTransitionTime
 * - baseDamage
 * - damageMultiplier
 * - maxHealth
 * - maxOxygen
 */
