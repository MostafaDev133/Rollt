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
            // 1. Cancel active power emitters
            if (rb.TryGetComponent<RepulsionWaveEmitter>(out var emitter))
            {
                emitter.CancelWave();
            }

            // 2. Clear any inventory locks
            if (rb.TryGetComponent<PlayerInventory>(out var inventory))
            {
                inventory.ResetActivePower();
            }

            // 3. Teleport and grant spawn protection
            Respawn(rb);
        }
    }

    private void Respawn(Rigidbody rb)
    {
        // 1. Cancel any lingering protection/constraints before moving
        if (rb.TryGetComponent<BallMovement>(out var movement))
        {
            movement.EndSpawnProtection();
        }

        // 2. Clear lingering velocities
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // 3. Calculate target position
        Vector3 targetPoint = Vector3.up * 2f;
        if (spawnAreaCollider != null)
        {
            targetPoint = spawnAreaCollider.ClosestPoint(rb.position);
        }
        targetPoint.y += verticalOffset;

        // 4. Force both Transform and Rigidbody to the new coordinates
        rb.transform.position = targetPoint;
        rb.position = targetPoint;

        // 5. Tell PhysX to sync immediately so the new coordinates are locked in
        Physics.SyncTransforms();

        // 6. Now apply protection constraints at the new location
        if (movement != null)
        {
            movement.ActivateSpawnProtection(3.0f);
        }
    }
}