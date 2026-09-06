using System;

public static class HealthEvents
{
    public static event Action<IHealthService, float, float> HealthChanged;
    public static event Action<IHealthService, float> DamageTaken;
    public static event Action<IHealthService, float> Healed;
    public static event Action<IHealthService> Died;

    public static void RaiseHealthChanged(
        IHealthService source,
        float currentHealth,
        float maxHealth)
    {
        HealthChanged?.Invoke(source, currentHealth, maxHealth);
    }

    public static void RaiseDamageTaken(
        IHealthService source,
        float amount)
    {
        DamageTaken?.Invoke(source, amount);
    }

    public static void RaiseHealed(
        IHealthService source,
        float amount)
    {
        Healed?.Invoke(source, amount);
    }

    public static void RaiseDied(IHealthService source)
    {
        Died?.Invoke(source);
    }
}