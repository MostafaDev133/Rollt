using UnityEngine;

public class KillZone : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("Drag your SpawnArea object here")]
    [SerializeField] private Collider spawnAreaCollider;

    [Tooltip("Small lift so the ball doesn't spawn stuck in the floor")]
    [SerializeField] private float verticalOffset = 1.0f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            Respawn(rb);
        }
    }

    private void Respawn(Rigidbody rb)
    {
        // 1. Zero out linear and angular speed
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // 2. Fallback check if the collider wasn't assigned in the Inspector
        if (spawnAreaCollider == null)
        {
            rb.position = new Vector3(0f, 2f, 0f);
            return;
        }

        // 3. Unity finds the point inside SpawnArea closest to where the ball dropped
        Vector3 fallPoint = rb.position;
        Vector3 closestSpawnPoint = spawnAreaCollider.ClosestPoint(fallPoint);

        // 4. Add slight upward offset to ensure a clean drop
        closestSpawnPoint.y += verticalOffset;

        // 5. Teleport ball
        rb.position = closestSpawnPoint;
    }
}