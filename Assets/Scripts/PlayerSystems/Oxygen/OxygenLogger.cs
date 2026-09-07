using UnityEngine;

/// <summary>
/// Simple logger that prints oxygen events for a bound IOxygenService to the Unity console.
/// Attach to the player and optionally assign the OxygenSystem reference in the inspector.
/// </summary>
public class OxygenLogger : MonoBehaviour
{
    [Header("Local Test")]
    [SerializeField] private OxygenSystem oxygenSystem;
    [Header("Runtime Logging")]
    [Tooltip("When true, log current oxygen repeatedly at the interval below.")]
    [SerializeField] private bool logEveryFrame = true;
    [Tooltip("Seconds between repeated logs when logEveryFrame is enabled.")]
    [SerializeField] private float logInterval = 0.5f;

    private IOxygenService oxygenService;
    private float logTimer = 0f;

    private void OnEnable()
    {
        OxygenEvents.OxygenChanged += OnOxygenChanged;
        OxygenEvents.OxygenAdded += OnOxygenAdded;
        OxygenEvents.OxygenSpent += OnOxygenSpent;
        OxygenEvents.OxygenDepleted += OnOxygenDepleted;
    }

    private void OnDisable()
    {
        OxygenEvents.OxygenChanged -= OnOxygenChanged;
        OxygenEvents.OxygenAdded -= OnOxygenAdded;
        OxygenEvents.OxygenSpent -= OnOxygenSpent;
        OxygenEvents.OxygenDepleted -= OnOxygenDepleted;
    }

    private void Start()
    {
        if (oxygenSystem != null)
        {
            Bind(oxygenSystem);
            return;
        }

        // Try to auto-bind any IOxygenService on this GameObject
        oxygenService = GetComponent<IOxygenService>();
        if (oxygenService != null)
        {
            Debug.Log("[OxygenLogger] Auto-bound to IOxygenService on same GameObject.", this);
            return;
        }

        oxygenService = GetComponentInChildren<IOxygenService>(true);
        if (oxygenService != null)
        {
            Debug.Log("[OxygenLogger] Auto-bound to IOxygenService in children.", this);
            return;
        }

        oxygenService = GetComponentInParent<IOxygenService>(true);
        if (oxygenService != null)
        {
            Debug.Log("[OxygenLogger] Auto-bound to IOxygenService in parent.", this);
            return;
        }

        Debug.LogWarning("[OxygenLogger] No IOxygenService found to bind. Assign OxygenSystem in the inspector.", this);
    }

    private void Update()
    {
        if (!logEveryFrame || oxygenService == null)
            return;

        logTimer -= Time.deltaTime;
        if (logTimer <= 0f)
        {
            Debug.Log($"[Oxygen][Realtime] {oxygenService.CurrentOxygen}", this);
            logTimer = Mathf.Max(0.01f, logInterval);
        }
    }

    public void Bind(IOxygenService source)
    {
        oxygenService = source;
        Debug.Log($"[OxygenLogger] Bound to oxygen source. Current: {oxygenService.CurrentOxygen}", this);
    }

    private void OnOxygenChanged(IOxygenService source, float currentOxygen)
    {
        if (source != oxygenService) return;
        Debug.Log($"[Oxygen] Changed -> {currentOxygen}", this);
    }

    private void OnOxygenAdded(IOxygenService source, float amount)
    {
        if (source != oxygenService) return;
        Debug.Log($"[Oxygen] Added +{amount} -> {source.CurrentOxygen}", this);
    }

    private void OnOxygenSpent(IOxygenService source, float amount)
    {
        if (source != oxygenService) return;
        Debug.Log($"[Oxygen] Spent -{amount} -> {source.CurrentOxygen}", this);
    }

    private void OnOxygenDepleted(IOxygenService source)
    {
        if (source != oxygenService) return;
        Debug.Log("[Oxygen] Depleted", this);
    }
}
