using UnityEngine;

public class OxygenSystem : MonoBehaviour, IOxygenService
{
    [Header("Oxygen Settings")]
    [SerializeField]
    [Min(0f)]
    private float startingOxygen = 120f;

    private float currentOxygen;

    public float CurrentOxygen => currentOxygen;
    public float StartingOxygen => startingOxygen;
    public bool IsEmpty => currentOxygen <= 0f;

    private void Awake()
    {
        currentOxygen = Mathf.Max(0f, startingOxygen);
    }

    public bool CanSpend(float amount)
    {
        return amount > 0f && currentOxygen >= amount;
    }

    public bool TrySpend(float amount)
    {
        if (!CanSpend(amount))
            return false;

        float previousOxygen = currentOxygen;

        currentOxygen -= amount;
        currentOxygen = Mathf.Max(currentOxygen, 0f);

        float actualSpent = previousOxygen - currentOxygen;

        OxygenEvents.RaiseOxygenSpent(this, actualSpent);
        OxygenEvents.RaiseOxygenChanged(this, currentOxygen);

        CheckForDepletion(previousOxygen);

        return true;
    }

    public void ConsumeOxygen(float amount)
    {
        if (amount <= 0f || IsEmpty)
            return;

        float previousOxygen = currentOxygen;

        currentOxygen -= amount;
        currentOxygen = Mathf.Max(currentOxygen, 0f);

        OxygenEvents.RaiseOxygenChanged(this, currentOxygen);

        CheckForDepletion(previousOxygen);
    }

    public void AddOxygen(float amount)
    {
        if (amount <= 0f)
            return;

        currentOxygen += amount;

        OxygenEvents.RaiseOxygenAdded(this, amount);
        OxygenEvents.RaiseOxygenChanged(this, currentOxygen);
    }

    public void ResetOxygen()
    {
        float previousOxygen = currentOxygen;

        currentOxygen = Mathf.Max(0f, startingOxygen);

        if (!Mathf.Approximately(previousOxygen, currentOxygen))
        {
            OxygenEvents.RaiseOxygenChanged(this, currentOxygen);
        }
    }

    private void CheckForDepletion(float previousOxygen)
    {
        if (previousOxygen > 0f && IsEmpty)
        {
            OxygenEvents.RaiseOxygenDepleted(this);
        }
    }
}