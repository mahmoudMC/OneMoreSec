using UnityEngine;


public class LocalScanTargetProvider : MonoBehaviour, IScanTargetProvider
{
    public bool TryGetNearestTarget(
        Transform requester,
        out Transform target)
    {
        target = null;

        GameObject[] players =
            GameObject.FindGameObjectsWithTag("Player");

        float closestDistanceSqr = float.MaxValue;

        foreach (GameObject player in players)
        {
            Transform candidate = player.transform;

            // Don't scan yourself.
            if (candidate == requester)
                continue;

            float distanceSqr =
                (candidate.position - requester.position).sqrMagnitude;

            if (distanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqr;
                target = candidate;
            }
        }

        return target != null;
    }
}