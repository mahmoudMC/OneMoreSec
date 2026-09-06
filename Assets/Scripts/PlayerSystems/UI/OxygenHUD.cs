using TMPro;
using UnityEngine;

public class OxygenHUD : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text oxygenText;

    [Header("Local Test")]
    [SerializeField] private OxygenSystem oxygenSystem;

    private IOxygenService oxygenService;

    private void OnEnable()
    {
        OxygenEvents.OxygenChanged += OnOxygenChanged;
    }

    private void Start()
    {
        if (oxygenSystem != null)
        {
            Bind(oxygenSystem);
        }
    }

    private void OnDisable()
    {
        OxygenEvents.OxygenChanged -= OnOxygenChanged;
    }

    public void Bind(IOxygenService source)
    {
        oxygenService = source;
        Refresh();
    }

    private void OnOxygenChanged(
        IOxygenService source,
        float currentOxygen)
    {
        if (source != oxygenService)
            return;

        UpdateUI(currentOxygen);
    }

    private void Refresh()
    {
        if (oxygenService == null)
            return;

        UpdateUI(oxygenService.CurrentOxygen);
    }

    private void UpdateUI(float currentOxygen)
    {
        if (oxygenText == null)
            return;

        oxygenText.text =
            $"{Mathf.CeilToInt(currentOxygen)}s";
    }
}