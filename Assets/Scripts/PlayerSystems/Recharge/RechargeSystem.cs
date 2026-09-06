using UnityEngine;


public class RechargeSystem : MonoBehaviour, IRechargeService
{
    [Header("Recharge Settings")]
    [SerializeField]
    [Min(0f)]
    private float rechargeRatePerSecond = 2f;

    private IOxygenService oxygenService;

    // TODO: Uncomment when the related interfaces are available.
    // private IMovementControl movementControl;
    // private ICombatControl combatControl;
    // private IRevealService revealService;

    private bool isRecharging;

    public bool IsRecharging => isRecharging;

    private void Awake()
    {
        oxygenService = GetComponent<IOxygenService>();

        if (oxygenService == null)
        {
            Debug.LogError(
                "RechargeSystem requires an IOxygenService.",
                this
            );
        }

        // TODO: Connect these dependencies when their interfaces are ready.
        //
        // movementControl = GetComponent<IMovementControl>();
        // combatControl = GetComponent<ICombatControl>();
        // revealService = GetComponent<IRevealService>();
    }

    private void Update()
    {
        if (!isRecharging || oxygenService == null)
            return;

        oxygenService.AddOxygen(
            rechargeRatePerSecond * Time.deltaTime
        );
    }

    public void StartRecharge()
    {
        if (isRecharging || oxygenService == null)
            return;

        isRecharging = true;

        // TODO: Recharge must disable player movement.
        // Waiting for IMovementControl implementation.
        // movementControl.SetMovementEnabled(false);

        // TODO: Recharge must disable shooting/combat.
        // Waiting for ICombatControl implementation.
        // combatControl.SetCombatEnabled(false);

        // TODO: Player must be revealed while recharging.
        // Waiting for IRevealService implementation.
        // revealService.SetRevealed(true);

        RechargeEvents.RaiseRechargeStarted(this);
    }

    public void CancelRecharge()
    {
        if (!isRecharging)
            return;

        isRecharging = false;

        // TODO: Restore movement when recharge is cancelled.
        // movementControl.SetMovementEnabled(true);

        // TODO: Restore combat when recharge is cancelled.
        // combatControl.SetCombatEnabled(true);

        // TODO: Stop revealing the player when recharge is cancelled.
        // revealService.SetRevealed(false);

        RechargeEvents.RaiseRechargeCancelled(this);
    }
}