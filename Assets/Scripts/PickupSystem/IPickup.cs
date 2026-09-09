using UnityEngine;

public interface IPickup
{
    string PickupName { get; }
    bool IsCollected { get; }

    bool CanCollect(GameObject collector);
    bool TryCollect(GameObject collector);
}
