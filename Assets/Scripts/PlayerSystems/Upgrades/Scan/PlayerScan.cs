using System.Collections;
using UnityEngine;

public class PlayerScan : MonoBehaviour, IUpgrade
{
    [Header("Upgrade Settings")]
    [SerializeField]
    private string upgradeName = "Nearest Player Scan";

    [SerializeField]
    [Min(0f)]
    private float cost = 15f;

    [SerializeField]
    [Min(0f)]
    private float duration = 5f;

    private IOxygenService oxygenService;
    private IScanTargetProvider scanTargetProvider;

    private bool isActive;
    private Transform activeTarget;
    private Coroutine activeCoroutine;

    public string UpgradeName => upgradeName;
    public float Cost => cost;

    public bool IsActive => isActive;
    public float Duration => duration;

    private void Awake()
    {
        oxygenService = GetComponent<IOxygenService>();
        scanTargetProvider = GetComponent<IScanTargetProvider>();

        if (oxygenService == null)
        {
            Debug.LogError(
                "PlayerScan requires an IOxygenService.",
                this
            );
        }

        if (scanTargetProvider == null)
        {
            Debug.LogError(
                "PlayerScan requires an IScanTargetProvider.",
                this
            );
        }
    }

    public bool CanActivate()
    {
        if (isActive)
            return false;

        if (oxygenService == null || scanTargetProvider == null)
            return false;

        if (!oxygenService.CanSpend(cost))
            return false;

        return scanTargetProvider.TryGetNearestTarget(
            transform,
            out _
        );
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

        if (oxygenService == null || scanTargetProvider == null)
        {
            UpgradeEvents.RaiseUpgradeActivationFailed(
                this,
                UpgradeFailureReason.CannotActivate
            );

            return false;
        }

        if (!scanTargetProvider.TryGetNearestTarget(
                transform,
                out Transform target))
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

        Activate(target);

        UpgradeEvents.RaiseUpgradeActivated(this);

        return true;
    }

    private void Activate(Transform target)
    {
        isActive = true;
        activeTarget = target;

        // TODO:
        // Reveal the selected target through the Reveal System.
        //
        // Example concept:
        //
        // IRevealService revealService =
        //     activeTarget.GetComponent<IRevealService>();
        //
        // revealService?.SetRevealed(true);

        activeCoroutine = StartCoroutine(ScanDuration());
    }

    private IEnumerator ScanDuration()
    {
        yield return new WaitForSeconds(duration);

        Deactivate();
    }

    private void Deactivate()
    {
        if (!isActive)
            return;

        // TODO:
        // Stop revealing the selected target.
        //
        // if (activeTarget != null)
        // {
        //     IRevealService revealService =
        //         activeTarget.GetComponent<IRevealService>();
        //
        //     revealService?.SetRevealed(false);
        // }

        isActive = false;
        activeTarget = null;
        activeCoroutine = null;
    }
}