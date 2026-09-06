using UnityEngine;

public class UpgradeInput : MonoBehaviour
{
    [Header("Upgrade References")]
    [SerializeField] private SpeedBoost speedBoost;
    [SerializeField] private DamageBoost damageBoost;
    [SerializeField] private PlayerScan playerScan;

    [Header("Recharge")]
    [SerializeField] private RechargeSystem rechargeSystem;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (speedBoost != null)
                speedBoost.TryActivate();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (damageBoost != null)
                damageBoost.TryActivate();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (playerScan != null)
                playerScan.TryActivate();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (rechargeSystem == null)
                return;

            if (rechargeSystem.IsRecharging)
                rechargeSystem.CancelRecharge();
            else
                rechargeSystem.StartRecharge();
        }
    }
}