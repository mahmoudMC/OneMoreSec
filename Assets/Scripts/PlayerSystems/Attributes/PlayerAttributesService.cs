using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Service that manages all player attributes and modifiers.
/// Upgrades and other systems modify attributes through this service.
/// </summary>
public class PlayerAttributesService : MonoBehaviour, IPlayerAttributes
{
    [Header("Movement Speeds")]
    [SerializeField] private float baseWalkSpeed = 4f;
    [SerializeField] private float baseRunSpeed = 7f;
    [SerializeField] [Range(0f, 1f)] private float baseCrouchSpeedMultiplier = 0.5f;

    [Header("Jump & Gravity")]
    [SerializeField] private float baseJumpHeight = 1.2f;
    [SerializeField] private float baseGravity = -20f;

    [Header("Crouch")]
    [SerializeField] private float baseCrouchHeight = 1f;
    [SerializeField] private float baseCrouchTransitionTime = 0.12f;

    [Header("Combat")]
    [SerializeField] private float baseBaseDamage = 10f;
    [SerializeField] private float baseDamageMultiplier = 1f;

    [Header("General")]
    [SerializeField] private float baseMaxHealth = 100f;
    [SerializeField] private float baseMaxOxygen = 100f;

    // Store modifiers per attribute (string -> list of modifiers with keys)
    private Dictionary<string, List<(string key, float modifier)>> modifiers = new();

    private void Awake()
    {
        InitializeModifierDictionary();
    }

    private void InitializeModifierDictionary()
    {
        modifiers.Clear();
        modifiers["walkSpeed"] = new();
        modifiers["runSpeed"] = new();
        modifiers["crouchSpeedMultiplier"] = new();
        modifiers["jumpHeight"] = new();
        modifiers["gravity"] = new();
        modifiers["crouchHeight"] = new();
        modifiers["crouchTransitionTime"] = new();
        modifiers["baseDamage"] = new();
        modifiers["damageMultiplier"] = new();
        modifiers["maxHealth"] = new();
        modifiers["maxOxygen"] = new();
    }

    #region Properties - Return current values (base * all modifiers)

    public float WalkSpeed => baseWalkSpeed * GetTotalModifier("walkSpeed");
    public float RunSpeed => baseRunSpeed * GetTotalModifier("runSpeed");
    public float CrouchSpeedMultiplier => Mathf.Clamp01(baseCrouchSpeedMultiplier * GetTotalModifier("crouchSpeedMultiplier"));
    
    public float JumpHeight => baseJumpHeight * GetTotalModifier("jumpHeight");
    public float Gravity => baseGravity * GetTotalModifier("gravity");
    
    public float CrouchHeight => baseCrouchHeight * GetTotalModifier("crouchHeight");
    public float CrouchTransitionTime => baseCrouchTransitionTime * GetTotalModifier("crouchTransitionTime");
    
    public float BaseDamage => baseBaseDamage * GetTotalModifier("baseDamage");
    public float DamageMultiplier => baseDamageMultiplier * GetTotalModifier("damageMultiplier");
    
    public float MaxHealth => baseMaxHealth * GetTotalModifier("maxHealth");
    public float MaxOxygen => baseMaxOxygen * GetTotalModifier("maxOxygen");

    #endregion

    #region Modifier Methods

    /// <summary>
    /// Apply a modifier to an attribute. 
    /// Pass a unique key so you can remove it later (e.g., "damageBoost_upgrade_001")
    /// </summary>
    public void ApplyModifier(string attributeName, float modifier, string modifierKey = "")
    {
        if (!modifiers.ContainsKey(attributeName))
        {
            Debug.LogWarning($"PlayerAttributesService: Unknown attribute '{attributeName}'");
            return;
        }

        // Use timestamp as key if not provided
        if (string.IsNullOrEmpty(modifierKey))
            modifierKey = $"{attributeName}_{Time.time}";

        modifiers[attributeName].Add((modifierKey, modifier));
        Debug.Log($"Applied {modifier}x modifier to {attributeName} (key: {modifierKey})");
    }

    /// <summary>
    /// Remove a specific modifier by key
    /// </summary>
    public bool RemoveModifier(string attributeName, string modifierKey)
    {
        if (!modifiers.ContainsKey(attributeName))
        {
            Debug.LogWarning($"PlayerAttributesService: Unknown attribute '{attributeName}'");
            return false;
        }

        bool removed = modifiers[attributeName].RemoveAll(m => m.key == modifierKey) > 0;
        if (removed)
            Debug.Log($"Removed modifier '{modifierKey}' from {attributeName}");
        else
            Debug.LogWarning($"Modifier key '{modifierKey}' not found in {attributeName}");
        
        return removed;
    }

    /// <summary>
    /// Get the total multiplier for an attribute (product of all active modifiers)
    /// </summary>
    public float GetTotalModifier(string attributeName)
    {
        if (!modifiers.ContainsKey(attributeName))
            return 1f;

        float total = 1f;
        foreach (var (key, modifier) in modifiers[attributeName])
        {
            total *= modifier;
        }
        return total;
    }

    /// <summary>
    /// Get base value before any modifiers
    /// </summary>
    public float GetBaseValue(string attributeName)
    {
        return attributeName switch
        {
            "walkSpeed" => baseWalkSpeed,
            "runSpeed" => baseRunSpeed,
            "crouchSpeedMultiplier" => baseCrouchSpeedMultiplier,
            "jumpHeight" => baseJumpHeight,
            "gravity" => baseGravity,
            "crouchHeight" => baseCrouchHeight,
            "crouchTransitionTime" => baseCrouchTransitionTime,
            "baseDamage" => baseBaseDamage,
            "damageMultiplier" => baseDamageMultiplier,
            "maxHealth" => baseMaxHealth,
            "maxOxygen" => baseMaxOxygen,
            _ => 0f
        };
    }

    /// <summary>
    /// Reset all attributes to base values and clear modifiers
    /// </summary>
    public void ResetToDefaults()
    {
        InitializeModifierDictionary();
        Debug.Log("PlayerAttributesService: Reset all attributes to defaults");
    }

    #endregion

    #region Debug
    /// <summary>
    /// Print all active modifiers (useful for debugging)
    /// </summary>
    public void PrintModifiers()
    {
        Debug.Log("=== PLAYER ATTRIBUTE MODIFIERS ===");
        foreach (var kvp in modifiers)
        {
            if (kvp.Value.Count > 0)
            {
                Debug.Log($"{kvp.Key}: {kvp.Value.Count} modifiers, Total: {GetTotalModifier(kvp.Key)}x");
                foreach (var (key, mod) in kvp.Value)
                    Debug.Log($"  - {key}: {mod}x");
            }
        }
    }
    #endregion
}
