using System;

public static class OxygenEvents
{
    public static event Action<IOxygenService, float> OxygenChanged;
    public static event Action<IOxygenService> OxygenDepleted;
    public static event Action<IOxygenService, float> OxygenAdded;
    public static event Action<IOxygenService, float> OxygenSpent;

    public static void RaiseOxygenChanged(
        IOxygenService source,
        float currentOxygen)
    {
        OxygenChanged?.Invoke(source, currentOxygen);
    }

    public static void RaiseOxygenDepleted(
        IOxygenService source)
    {
        OxygenDepleted?.Invoke(source);
    }

    public static void RaiseOxygenAdded(
        IOxygenService source,
        float amount)
    {
        OxygenAdded?.Invoke(source, amount);
    }

    public static void RaiseOxygenSpent(
        IOxygenService source,
        float amount)
    {
        OxygenSpent?.Invoke(source, amount);
    }
}