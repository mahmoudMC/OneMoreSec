using UnityEngine;

public class HealthSystem : MonoBehaviour, IHealthService
{
    [SerializeField] private float maxHealth = 100f;

    private float currentHealth;
    private bool isDead;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => isDead;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0f || isDead)
            return;

        float previousHealth = currentHealth;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        float actualDamage = previousHealth - currentHealth;

        HealthEvents.RaiseDamageTaken(actualDamage);
        HealthEvents.RaiseHealthChanged(currentHealth, maxHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (amount <= 0f || isDead)
            return;

        float previousHealth = currentHealth;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        float actualHeal = currentHealth - previousHealth;

        if (actualHeal <= 0f)
            return;

        HealthEvents.RaiseHealed(actualHeal);
        HealthEvents.RaiseHealthChanged(currentHealth, maxHealth);
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        HealthEvents.RaiseDied();
    }
}