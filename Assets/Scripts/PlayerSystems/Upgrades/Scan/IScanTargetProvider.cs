using UnityEngine;

public interface IScanTargetProvider
{
    bool TryGetNearestTarget(
        Transform requester,
        out Transform target
    );
}