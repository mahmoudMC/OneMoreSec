using UnityEngine;

public class PickupSpawnPoint : MonoBehaviour
{
    [Header("Spawn Point Settings")]
    [SerializeField]
    private bool isEnabled = true;

    [Header("Debug")]
    [SerializeField]
    private float gizmoRadius = 0.4f;

    private PickupSystem currentPickup;

    public bool IsEnabled => isEnabled;
    public bool IsOccupied => currentPickup != null;
    public bool IsAvailable => isEnabled && !IsOccupied;
    public PickupSystem CurrentPickup => currentPickup;

    public Vector3 SpawnPosition => transform.position;
    public Quaternion SpawnRotation => transform.rotation;

    public void SetOccupant(PickupSystem pickup)
    {
        currentPickup = pickup;
    }

    public void ReleaseOccupant()
    {
        currentPickup = null;
    }

    public void ClearOccupant()
    {
        if (currentPickup != null)
        {
            // TODO: Multiplayer - despawn the networked object on the server
            // instead of destroying it locally.

            Destroy(currentPickup.gameObject);
        }

        currentPickup = null;
    }

    public void SetEnabled(bool enabled)
    {
        isEnabled = enabled;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isEnabled
            ? (IsOccupied ? Color.green : Color.cyan)
            : Color.gray;

        Gizmos.DrawWireSphere(transform.position, gizmoRadius);
        Gizmos.DrawLine(
            transform.position,
            transform.position + Vector3.up * gizmoRadius * 2f
        );
    }
}
