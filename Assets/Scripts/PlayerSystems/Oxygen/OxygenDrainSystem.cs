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

        // Try to auto-find IOxygenService if not on same GameObject
        if (oxygenService == null)
        {
            oxygenService = GetComponentInChildren<IOxygenService>(true);
            if (oxygenService == null)
                oxygenService = GetComponentInParent<IOxygenService>(true);
        }

        if (oxygenService == null)
        {
            Debug.LogError(
                "OxygenDrainSystem requires an IOxygenService on the same GameObject (or in children/parents).",
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

    [Header("Debug")]
    [SerializeField] private bool debugLogging = true;
    [SerializeField] private float debugLogInterval = 1f;
    private float debugTimer = 0f;
    private void Start()
    {
        Debug.Log("OxygenDrainSystem Started", this);
    }

    private void Update()
    {
       // optional periodic debug logging
        if (debugLogging)
        {
            debugTimer -= Time.deltaTime;
            if (debugTimer <= 0f)
            {
                Debug.Log($"[OxygenDrainSystem] Oxygen: {oxygenService?.CurrentOxygen} | Health: {healthService?.CurrentHealth} | Drain Enabled: {isDrainEnabled}", this);
                debugTimer = Mathf.Max(0.01f, debugLogInterval);
            }
        }
        if (oxygenService == null || healthService == null)
            return;

        if (healthService.IsDead)
            return;

        if (!isDrainEnabled)
            return;

        if (!oxygenService.IsEmpty)
        {
            float amount = oxygenDrainPerSecond * Time.deltaTime;
            oxygenService.ConsumeOxygen(amount);

            if (debugLogging)
            {
                // immediate log when consuming (supplemental)
                Debug.Log($"[OxygenDrainSystem] Consumed {amount:F4} oxygen -> {oxygenService.CurrentOxygen:F4}", this);
            }

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