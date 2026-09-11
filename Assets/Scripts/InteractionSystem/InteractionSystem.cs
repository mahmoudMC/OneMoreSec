using System;
using UnityEngine;

public class InteractionSystem : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField]
    private Transform interactionOrigin;

    [SerializeField]
    [Min(0f)]
    private float interactionRange = 3f;

    [SerializeField]
    [Min(0f)]
    private float interactionRadius = 0.25f;

    [SerializeField]
    private LayerMask interactableMask = ~0;

    [Header("Input")]
    [SerializeField]
    private KeyCode interactKey = KeyCode.F;

    [Header("Debug")]
    [SerializeField]
    private bool drawDebugRay = true;

    private IHealthService healthService;

    private IInteractable currentTarget;
    private bool isInteractionEnabled = true;

    public event Action<IInteractable> TargetChanged;
    public event Action<IInteractable> InteractionPerformed;
    public event Action InteractionFailed;

    public IInteractable CurrentTarget => currentTarget;
    public bool HasTarget => currentTarget != null;
    public bool IsInteractionEnabled => isInteractionEnabled;

    public string CurrentPrompt =>
        currentTarget != null ? currentTarget.InteractionPrompt : string.Empty;

    private void Awake()
    {
        healthService = GetComponent<IHealthService>();

        if (interactionOrigin == null && Camera.main != null)
        {
            interactionOrigin = Camera.main.transform;
        }

        if (interactionOrigin == null)
        {
            Debug.LogError(
                "InteractionSystem requires an interaction origin (usually the player camera).",
                this
            );
        }
    }

    private void Update()
    {
        // TODO: Multiplayer - only the local player may drive interaction.
        // Uncomment when the player prefab becomes a NetworkBehaviour.
        //
        // if (!IsOwner)
        //     return;

        if (!isInteractionEnabled || interactionOrigin == null)
        {
            SetTarget(null);
            return;
        }

        if (healthService != null && healthService.IsDead)
        {
            SetTarget(null);
            return;
        }

        SetTarget(DetectInteractable());

        if (Input.GetKeyDown(interactKey))
        {
            TryInteract();
        }
    }

    private void OnDisable()
    {
        SetTarget(null);
    }

    public bool TryInteract()
    {
        if (!isInteractionEnabled || currentTarget == null)
        {
            InteractionFailed?.Invoke();
            return false;
        }

        if (!currentTarget.CanInteract(gameObject))
        {
            InteractionFailed?.Invoke();
            return false;
        }

        IInteractable target = currentTarget;

        // TODO: Multiplayer - interaction must be validated on the server.
        // The owner should request the interaction and the server should
        // run the effect, so two players cannot claim the same Pickup or
        // Supply Box on the same frame.
        //
        // RequestInteractServerRpc(targetNetworkObjectId);

        target.Interact(gameObject);

        InteractionPerformed?.Invoke(target);

        return true;
    }

    public void SetInteractionEnabled(bool enabled)
    {
        isInteractionEnabled = enabled;

        if (!isInteractionEnabled)
        {
            SetTarget(null);
        }
    }

    private IInteractable DetectInteractable()
    {
        Vector3 origin = interactionOrigin.position;
        Vector3 direction = interactionOrigin.forward;

        RaycastHit hit;
        bool hasHit;

        if (interactionRadius > 0f)
        {
            hasHit = Physics.SphereCast(
                origin,
                interactionRadius,
                direction,
                out hit,
                interactionRange,
                interactableMask,
                QueryTriggerInteraction.Collide
            );
        }
        else
        {
            hasHit = Physics.Raycast(
                origin,
                direction,
                out hit,
                interactionRange,
                interactableMask,
                QueryTriggerInteraction.Collide
            );
        }

        if (!hasHit)
            return null;

        IInteractable interactable =
            hit.collider.GetComponentInParent<IInteractable>();

        if (interactable == null)
            return null;

        if (!interactable.CanInteract(gameObject))
            return null;

        return interactable;
    }

    private void SetTarget(IInteractable target)
    {
        if (ReferenceEquals(currentTarget, target))
            return;

        currentTarget = target;

        TargetChanged?.Invoke(currentTarget);
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawDebugRay || interactionOrigin == null)
            return;

        Gizmos.color = HasTarget ? Color.green : Color.red;

        Vector3 origin = interactionOrigin.position;
        Vector3 end = origin + interactionOrigin.forward * interactionRange;

        Gizmos.DrawLine(origin, end);

        if (interactionRadius > 0f)
        {
            Gizmos.DrawWireSphere(end, interactionRadius);
        }
    }
}
