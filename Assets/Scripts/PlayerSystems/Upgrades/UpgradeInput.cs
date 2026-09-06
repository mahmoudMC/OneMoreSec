using UnityEngine;

/// <summary>
/// Handles player input for activating upgrades.
///
/// Responsibilities:
/// - Listens for upgrade input keys.
/// - Requests activation from the correct upgrade.
///
/// This class should NOT:
/// - Contain upgrade logic.
/// - Modify oxygen.
/// - Apply movement or damage effects.
/// - Handle UI.
/// </summary>
public class UpgradeInput : MonoBehaviour
{
    private SpeedBoost speedBoost;
    private DamageBoost damageBoost;
    private PlayerScan playerScan;

    private void Awake()
    {
        speedBoost = GetComponent<SpeedBoost>();
        damageBoost = GetComponent<DamageBoost>();
        playerScan = GetComponent<PlayerScan>();

        if (speedBoost == null)
        {
            Debug.LogError(
                "UpgradeInput requires a SpeedBoost component.",
                this
            );
        }

        if (damageBoost == null)
        {
            Debug.LogError(
                "UpgradeInput requires a DamageBoost component.",
                this
            );
        }

        if (playerScan == null)
        {
            Debug.LogError(
                "UpgradeInput requires a PlayerScan component.",
                this
            );
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            speedBoost?.TryActivate();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            damageBoost?.TryActivate();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            playerScan?.TryActivate();
        }
    }
}