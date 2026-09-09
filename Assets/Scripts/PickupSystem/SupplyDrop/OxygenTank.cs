using UnityEngine;

public class OxygenTank : PickupSystem
{
    [Header("Oxygen Settings")]
    [SerializeField]
    [Min(0f)]
    private float oxygenAmount = 60f;

    public float OxygenAmount => oxygenAmount;

    protected override bool CanApplyEffect(GameObject collector)
    {
        // Oxygen has no maximum cap, so the tank is always collectable
        // as long as the collector owns an oxygen service.
        return collector.GetComponentInParent<IOxygenService>() != null;
    }

    protected override void ApplyEffect(GameObject collector)
    {
        IOxygenService oxygenService =
            collector.GetComponentInParent<IOxygenService>();

        if (oxygenService == null)
            return;

        oxygenService.AddOxygen(oxygenAmount);
    }
}
