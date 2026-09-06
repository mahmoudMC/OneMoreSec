using UnityEngine;

public class HealthSystem : MonoBehaviour, IHealthService
{
    [SerializeField]
    [Min(0f)]
    private float maxHealth = 100f;

    private float currentHealth;
    private bool isDead;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => isDead;

    private void Awake()
    {
        currentHealth = maxHealth;
        isDead = false;
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0f || isDead)
            return;

        float previousHealth = currentHealth;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        float actualDamage = previousHealth - currentHealth;

        if (actualDamage <= 0f)
            return;

        HealthEvents.RaiseDamageTaken(this, actualDamage);
        HealthEvents.RaiseHealthChanged(
            this,
            currentHealth,
            maxHealth
        );

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

        HealthEvents.RaiseHealed(this, actualHeal);
        HealthEvents.RaiseHealthChanged(
            this,
            currentHealth,
            maxHealth
        );
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        isDead = false;

        HealthEvents.RaiseHealthChanged(
            this,
            currentHealth,
            maxHealth
        );
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        HealthEvents.RaiseDied(this);
    }
}