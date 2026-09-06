using System;

public static class HealthEvents
{
    public static event Action<float, float> HealthChanged;
    public static event Action<float> DamageTaken;
    public static event Action<float> Healed;
    public static event Action Died;

    public static void RaiseHealthChanged(float currentHealth, float maxHealth)
    {
        HealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public static void RaiseDamageTaken(float amount)
    {
        DamageTaken?.Invoke(amount);
    }

    public static void RaiseHealed(float amount)
    {
        Healed?.Invoke(amount);
    }

    public static void RaiseDied()
    {
        Died?.Invoke();
    }
}