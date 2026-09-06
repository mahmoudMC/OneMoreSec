using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthHUD : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image healthFill;
    [SerializeField] private TMP_Text healthText;

    [Header("Local Test")]
    [SerializeField] private HealthSystem healthSystem;

    private IHealthService healthService;

    private void OnEnable()
    {
        HealthEvents.HealthChanged += OnHealthChanged;
    }

    private void Start()
    {
        if (healthSystem != null)
            Bind(healthSystem);
    }

    private void OnDisable()
    {
        HealthEvents.HealthChanged -= OnHealthChanged;
    }

    public void Bind(IHealthService source)
    {
        healthService = source;
        Refresh();
    }

    private void OnHealthChanged(
        IHealthService source,
        float currentHealth,
        float maxHealth)
    {
        if (source != healthService)
            return;

        UpdateUI(currentHealth, maxHealth);
    }

    private void Refresh()
    {
        if (healthService == null)
            return;

        UpdateUI(
            healthService.CurrentHealth,
            healthService.MaxHealth
        );
    }

    private void UpdateUI(float currentHealth, float maxHealth)
    {
        if (healthFill != null)
        {
            healthFill.fillAmount =
                maxHealth > 0f
                    ? currentHealth / maxHealth
                    : 0f;
        }

        if (healthText != null)
        {
            healthText.text =
                $"{Mathf.CeilToInt(currentHealth)} / {Mathf.CeilToInt(maxHealth)}";
        }
    }
}