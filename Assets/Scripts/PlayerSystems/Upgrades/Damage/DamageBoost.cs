using System.Collections;
using UnityEngine;


public class DamageBoost : MonoBehaviour, IUpgrade
{
    [Header("Upgrade Settings")]
    [SerializeField]
    private string upgradeName = "Damage Boost";

    [SerializeField]
    [Min(0f)]
    private float cost = 20f;

    [SerializeField]
    [Min(0f)]
    private float duration = 10f;

    [SerializeField]
    [Min(0f)]
    private float damageMultiplier = 1.25f;

    private IOxygenService oxygenService;

    // TODO:
    // private IDamageModifier damageModifier;

    private bool isActive;
    private Coroutine activeCoroutine;

    public string UpgradeName => upgradeName;
    public float Cost => cost;

    public bool IsActive => isActive;
    public float Duration => duration;
    public float DamageMultiplier => damageMultiplier;

    private void Awake()
    {
        oxygenService = GetComponent<IOxygenService>();

        if (oxygenService == null)
        {
            Debug.LogError(
                "DamageBoost requires an IOxygenService.",
                this
            );
        }

        // TODO:
        // damageModifier = GetComponent<IDamageModifier>();
    }

    public bool CanActivate()
    {
        if (isActive)
            return false;

        if (oxygenService == null)
            return false;

        return oxygenService.CanSpend(cost);
    }

    public bool TryActivate()
    {
        if (isActive)
        {
            UpgradeEvents.RaiseUpgradeActivationFailed(
                this,
                UpgradeFailureReason.AlreadyActive
            );

            return false;
        }

        if (oxygenService == null)
        {
            UpgradeEvents.RaiseUpgradeActivationFailed(
                this,
                UpgradeFailureReason.CannotActivate
            );

            return false;
        }

        if (!oxygenService.TrySpend(cost))
        {
            UpgradeEvents.RaiseUpgradeActivationFailed(
                this,
                UpgradeFailureReason.NotEnoughOxygen
            );

            return false;
        }

        Activate();

        UpgradeEvents.RaiseUpgradeActivated(this);

        return true;
    }

    private void Activate()
    {
        isActive = true;

        // TODO:
        // Apply the damage multiplier through the Combat System.
        //
        // damageModifier.AddDamageMultiplier(damageMultiplier);

        activeCoroutine = StartCoroutine(BoostDuration());
    }

    private IEnumerator BoostDuration()
    {
        yield return new WaitForSeconds(duration);

        Deactivate();
    }

    private void Deactivate()
    {
        if (!isActive)
            return;

        isActive = false;

        // TODO:
        // Remove the damage multiplier through the Combat System.
        //
        // damageModifier.RemoveDamageMultiplier(damageMultiplier);

        activeCoroutine = null;
    }
}