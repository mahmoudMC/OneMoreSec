using System.Collections.Generic;
using UnityEngine;

public class MapPickupSpawner : MonoBehaviour
{
    [Header("Pickup Prefab")]
    [SerializeField]
    private SmallMedKit medKitPrefab;

    [Header("Round Settings")]
    [SerializeField]
    [Min(0)]
    private int pickupsPerRound = 6;

    [Tooltip(
        "Enable only while testing without the Match System. " +
        "The Match System should call SpawnRound() instead."
    )]
    [SerializeField]
    private bool spawnOnStart = true;

    [Header("Spawn Points")]
    [Tooltip("Leave empty to collect every PickupSpawnPoint under this object.")]
    [SerializeField]
    private PickupSpawnPoint[] spawnPoints;

    private readonly List<PickupSystem> spawnedPickups = new List<PickupSystem>();
    private readonly List<PickupSpawnPoint> availablePoints =
        new List<PickupSpawnPoint>();

    public int SpawnedCount => spawnedPickups.Count;

    private void Awake()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            spawnPoints = GetComponentsInChildren<PickupSpawnPoint>(true);
        }

        if (spawnPoints.Length == 0)
        {
            Debug.LogError(
                "MapPickupSpawner requires at least one PickupSpawnPoint.",
                this
            );
        }

        if (medKitPrefab == null)
        {
            Debug.LogError(
                "MapPickupSpawner requires a Small Med Kit prefab.",
                this
            );
        }
    }

    private void OnEnable()
    {
        PickupEvents.PickupCollected += OnPickupCollected;
    }

    private void OnDisable()
    {
        PickupEvents.PickupCollected -= OnPickupCollected;
    }

    private void Start()
    {
        if (spawnOnStart)
        {
            SpawnRound();
        }
    }

    public void SpawnRound()
    {
        // TODO: Multiplayer - only the server may spawn pickups, and every
        // spawned object must be networked so all clients see the same set.
        //
        // if (!IsServer)
        //     return;

        ClearRound();

        if (medKitPrefab == null)
            return;

        BuildAvailablePoints();

        int spawnCount = Mathf.Min(pickupsPerRound, availablePoints.Count);

        for (int i = 0; i < spawnCount; i++)
        {
            // Each point is removed from the list once used, so two pickups
            // can never share the same spawn point.
            int index = Random.Range(0, availablePoints.Count);

            PickupSpawnPoint point = availablePoints[index];
            availablePoints.RemoveAt(index);

            SpawnAt(point);
        }
    }

    public void ClearRound()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (spawnPoints[i] != null)
            {
                spawnPoints[i].ClearOccupant();
            }
        }

        spawnedPickups.Clear();
        availablePoints.Clear();
    }

    private void SpawnAt(PickupSpawnPoint point)
    {
        SmallMedKit medKit = Instantiate(
            medKitPrefab,
            point.SpawnPosition,
            point.SpawnRotation
        );

        // TODO: Multiplayer - NetworkObject.Spawn() the med kit here.

        point.SetOccupant(medKit);
        spawnedPickups.Add(medKit);
    }

    private void BuildAvailablePoints()
    {
        availablePoints.Clear();

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            PickupSpawnPoint point = spawnPoints[i];

            if (point != null && point.IsAvailable)
            {
                availablePoints.Add(point);
            }
        }
    }

    private void OnPickupCollected(IPickup pickup, GameObject collector)
    {
        PickupSystem collected = pickup as PickupSystem;

        if (collected == null || !spawnedPickups.Remove(collected))
            return;

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            PickupSpawnPoint point = spawnPoints[i];

            if (point != null && point.CurrentPickup == collected)
            {
                point.ReleaseOccupant();
                return;
            }
        }
    }
}
