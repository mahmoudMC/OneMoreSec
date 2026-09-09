using UnityEngine;

public abstract class PickupSystem : MonoBehaviour, IPickup, IInteractable
{
    [Header("Pickup Settings")]
    [SerializeField]
    private string pickupName = "Pickup";

    [SerializeField]
    private string interactionPrompt = "Press F to pick up";

    [SerializeField]
    private bool destroyOnCollect = true;

    private bool isCollected;

    public string PickupName => pickupName;
    public bool IsCollected => isCollected;

    public string InteractionPrompt => interactionPrompt;

    protected virtual void Start()
    {
        PickupEvents.RaisePickupSpawned(this);
    }

    public bool CanInteract(GameObject interactor)
    {
        return CanCollect(interactor);
    }

    public void Interact(GameObject interactor)
    {
        TryCollect(interactor);
    }

    public bool CanCollect(GameObject collector)
    {
        if (isCollected || collector == null)
            return false;

        if (IsCollectorDead(collector))
            return false;

        return CanApplyEffect(collector);
    }

    public bool TryCollect(GameObject collector)
    {
        if (!CanCollect(collector))
        {
            PickupEvents.RaisePickupCollectionFailed(this, collector);

            return false;
        }

        // TODO: Multiplayer - collection must be server-authoritative so two
        // players cannot collect the same pickup on the same frame.
        // The server applies the effect and then despawns the object for
        // every client.

        isCollected = true;

        ApplyEffect(collector);

        PickupEvents.RaisePickupCollected(this, collector);

        Remove();

        return true;
    }

    protected abstract bool CanApplyEffect(GameObject collector);

    protected abstract void ApplyEffect(GameObject collector);

    protected virtual void Remove()
    {
        PickupEvents.RaisePickupRemoved(this);

        // TODO: Multiplayer - replace with NetworkObject.Despawn() on the
        // server once pickups become networked objects.

        if (destroyOnCollect)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public static bool IsCollectorDead(GameObject collector)
    {
        IHealthService healthService =
            collector.GetComponentInParent<IHealthService>();

        return healthService != null && healthService.IsDead;
    }
}
