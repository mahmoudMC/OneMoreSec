using UnityEngine;


public class OxygenDrainSystem : MonoBehaviour
{
    [Header("Drain Settings")]
    [SerializeField]
    [Min(0f)]
    private float oxygenDrainPerSecond = 1f;

    [Header("Zero Oxygen Damage")]
    [SerializeField]
    [Min(0f)]
    private float healthDrainPerSecond = 5f;

    private IOxygenService oxygenService;
    private IHealthService healthService;

    private bool isDrainEnabled = true;

    public bool IsDrainEnabled => isDrainEnabled;

    private void Awake()
    {
        oxygenService = GetComponent<IOxygenService>();
        healthService = GetComponent<IHealthService>();

        if (oxygenService == null)
        {
            Debug.LogError(
                "OxygenDrainSystem requires an IOxygenService on the same GameObject.",
                this
            );
        }

        if (healthService == null)
        {
            Debug.LogError(
                "OxygenDrainSystem requires an IHealthService on the same GameObject.",
                this
            );
        }
    }
    private void Start()
    {
        Debug.Log("OxygenDrainSystem Started", this);
    }

    private void Update()
    {
       // Debug.Log(
       //    $"Oxygen: {oxygenService?.CurrentOxygen} | " +
       //    $"Health: {healthService?.CurrentHealth} | " +
       //    $"Drain Enabled: {isDrainEnabled}"
       //);
        if (oxygenService == null || healthService == null)
            return;

        if (healthService.IsDead)
            return;

        if (!isDrainEnabled)
            return;

        if (!oxygenService.IsEmpty)
        {
            oxygenService.ConsumeOxygen(
                oxygenDrainPerSecond * Time.deltaTime
            );

            return;
        }

        healthService.TakeDamage(
            healthDrainPerSecond * Time.deltaTime
        );
    }

    public void SetDrainEnabled(bool enabled)
    {
        isDrainEnabled = enabled;
    }
}