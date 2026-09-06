public interface IOxygenService
{
    float CurrentOxygen { get; }
    float StartingOxygen { get; }
    bool IsEmpty { get; }

    bool CanSpend(float amount);
    bool TrySpend(float amount);

    void AddOxygen(float amount);
    void ConsumeOxygen(float amount);

    void ResetOxygen();
}