using System;
using UnityEngine;

public static class PickupEvents
{
    public static event Action<IPickup> PickupSpawned;
    public static event Action<IPickup, GameObject> PickupCollected;
    public static event Action<IPickup, GameObject> PickupCollectionFailed;
    public static event Action<IPickup> PickupRemoved;
    public static event Action<SupplyBox, GameObject, IPickup> SupplyBoxOpened;

    public static void RaisePickupSpawned(IPickup source)
    {
        PickupSpawned?.Invoke(source);
    }

    public static void RaisePickupCollected(
        IPickup source,
        GameObject collector)
    {
        PickupCollected?.Invoke(source, collector);
    }

    public static void RaisePickupCollectionFailed(
        IPickup source,
        GameObject collector)
    {
        PickupCollectionFailed?.Invoke(source, collector);
    }

    public static void RaisePickupRemoved(IPickup source)
    {
        PickupRemoved?.Invoke(source);
    }

    public static void RaiseSupplyBoxOpened(
        SupplyBox source,
        GameObject opener,
        IPickup reward)
    {
        SupplyBoxOpened?.Invoke(source, opener, reward);
    }
}
