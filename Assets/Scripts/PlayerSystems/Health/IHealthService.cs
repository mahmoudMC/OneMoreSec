public interface IHealthService
{
    float CurrentHealth { get; }
    float MaxHealth { get; }
    bool IsDead { get; }

    void TakeDamage(float amount);
    void Heal(float amount);
    void ResetHealth();
}