using UnityEngine;
using System.Collections.Generic;

public class MapSpawnManager : MonoBehaviour
{
    [Header("Designer-Placed Safe Spots")]
    [Tooltip("Drag all valid safe spawn transforms for this specific map here.")]
    [SerializeField] private List<Transform> safeSpawnPoints = new List<Transform>();

    [SerializeField] private float dropHeightOffset = 1.5f;

    public static MapSpawnManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Finds the closest safe spawn point to the ball's fall position.
    /// </summary>
    public Vector3 GetClosestSpawnPosition(Vector3 fallPosition)
    {
        if (safeSpawnPoints == null || safeSpawnPoints.Count == 0)
        {
            // Fallback in case no points were added to the list
            return new Vector3(0f, 2f, 0f);
        }

        Transform closestPoint = safeSpawnPoints[0];
        float minDistanceSqr = Mathf.Infinity;

        // Iterate through all valid map spawn points
        for (int i = 0; i < safeSpawnPoints.Count; i++)
        {
            Transform point = safeSpawnPoints[i];
            if (point == null) continue;

            // Compare distance on the horizontal (X/Z) plane
            Vector3 diff = point.position - fallPosition;
            diff.y = 0f; // Ignore vertical fall depth
            float distSqr = diff.sqrMagnitude;

            if (distSqr < minDistanceSqr)
            {
                minDistanceSqr = distSqr;
                closestPoint = point;
            }
        }

        return closestPoint.position + (Vector3.up * dropHeightOffset);
    }

    // Visual aid in editor to see all safe points clearly
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        foreach (var point in safeSpawnPoints)
        {
            if (point != null)
            {
                Gizmos.DrawWireSphere(point.position + Vector3.up * dropHeightOffset, 0.6f);
            }
        }
    }
}