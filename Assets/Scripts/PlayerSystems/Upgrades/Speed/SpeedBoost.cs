using System.Collections;
using UnityEngine;

public class SpeedBoost : MonoBehaviour, IUpgrade
{
    [Header("Upgrade Settings")]
    [SerializeField]
    private string upgradeName = "Speed Boost";

    [SerializeField]
    [Min(0f)]
    private float cost = 15f;

    [SerializeField]
    [Min(0f)]
    private float duration = 10f;

    [SerializeField]
    [Min(0f)]
    private float speedMultiplier = 1.3f;

    private IOxygenService oxygenService;

    // TODO:
    // private IMovementModifier movementModifier;

    private bool isActive;
    private Coroutine activeCoroutine;

    public string UpgradeName => upgradeName;
    public float Cost => cost;

    public bool IsActive => isActive;
    public float Duration => duration;
    public float SpeedMultiplier => speedMultiplier;

    private void Awake()
    {
        oxygenService = GetComponent<IOxygenService>();

        if (oxygenService == null)
        {
            Debug.LogError(
                "SpeedBoost requires an IOxygenService.",
                this
            );
        }

        // TODO:
        // movementModifier = GetComponent<IMovementModifier>();
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
        // Apply the speed multiplier through the Movement System.
        //
        // movementModifier.AddSpeedMultiplier(speedMultiplier);

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
        // Remove the speed multiplier through the Movement System.
        //
        // movementModifier.RemoveSpeedMultiplier(speedMultiplier);

        activeCoroutine = null;
    }
}