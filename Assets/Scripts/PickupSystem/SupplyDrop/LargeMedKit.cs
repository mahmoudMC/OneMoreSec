using UnityEngine;

public class LargeMedKit : PickupSystem
{
    [Header("Heal Settings")]
    [SerializeField]
    [Min(0f)]
    private float healAmount = 60f;

    public float HealAmount => healAmount;

    protected override bool CanApplyEffect(GameObject collector)
    {
        IHealthService healthService =
            collector.GetComponentInParent<IHealthService>();

        if (healthService == null)
            return false;

        return healthService.CurrentHealth < healthService.MaxHealth;
    }

    protected override void ApplyEffect(GameObject collector)
    {
        IHealthService healthService =
            collector.GetComponentInParent<IHealthService>();

        if (healthService == null)
            return;

        healthService.Heal(healAmount);
    }
}
