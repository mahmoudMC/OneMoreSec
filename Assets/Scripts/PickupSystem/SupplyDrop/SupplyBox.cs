using UnityEngine;

public class SupplyBox : MonoBehaviour, IInteractable
{
    [Header("Supply Box Settings")]
    [SerializeField]
    private string interactionPrompt = "Press F to open Supply Box";

    [SerializeField]
    private Transform rewardSpawnPoint;

    [Header("High-Value Loot Pool")]
    [Tooltip("Only one reward drops per Supply Box.")]
    [SerializeField]
    private PickupSystem[] rewardPrefabs;

    [Header("After Opening")]
    [SerializeField]
    private bool destroyAfterOpening = true;

    private bool isOpened;

    public string InteractionPrompt => interactionPrompt;
    public bool IsOpened => isOpened;

    private void Awake()
    {
        if (rewardSpawnPoint == null)
        {
            rewardSpawnPoint = transform;
        }

        if (rewardPrefabs == null || rewardPrefabs.Length == 0)
        {
            Debug.LogError(
                "SupplyBox requires at least one reward prefab.",
                this
            );
        }
    }

    public bool CanInteract(GameObject interactor)
    {
        if (isOpened || interactor == null)
            return false;

        if (PickupSystem.IsCollectorDead(interactor))
            return false;

        return HasReward();
    }

    public void Interact(GameObject interactor)
    {
        if (!CanInteract(interactor))
            return;

        // TODO: Multiplayer - opening must run on the server so only one
        // player can claim the box, and the spawned reward must be a
        // networked object spawned for every client.

        isOpened = true;

        IPickup reward = SpawnReward();

        PickupEvents.RaiseSupplyBoxOpened(this, interactor, reward);

        Close();
    }

    private bool HasReward()
    {
        if (rewardPrefabs == null)
            return false;

        for (int i = 0; i < rewardPrefabs.Length; i++)
        {
            if (rewardPrefabs[i] != null)
                return true;
        }

        return false;
    }

    private IPickup SpawnReward()
    {
        PickupSystem prefab = PickRandomReward();

        if (prefab == null)
            return null;

        PickupSystem reward = Instantiate(
            prefab,
            rewardSpawnPoint.position,
            rewardSpawnPoint.rotation
        );

        return reward;
    }

    private PickupSystem PickRandomReward()
    {
        int validCount = 0;

        for (int i = 0; i < rewardPrefabs.Length; i++)
        {
            if (rewardPrefabs[i] != null)
                validCount++;
        }

        if (validCount == 0)
            return null;

        int pickedIndex = Random.Range(0, validCount);

        for (int i = 0; i < rewardPrefabs.Length; i++)
        {
            if (rewardPrefabs[i] == null)
                continue;

            if (pickedIndex == 0)
                return rewardPrefabs[i];

            pickedIndex--;
        }

        return null;
    }

    private void Close()
    {
        // TODO: Play the opening VFX / SFX before the box disappears.

        if (destroyAfterOpening)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
