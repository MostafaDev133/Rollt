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
            // Cancel active power emitters
            if (rb.TryGetComponent<RepulsionWaveEmitter>(out var emitter)) emitter.CancelWave();
            // Clear any inventory locks
            if (rb.TryGetComponent<PlayerInventory>(out var inventory)) inventory.ResetActivePower();
            Respawn(rb); // Teleport and grant spawn protection
        }
    }

    private void Respawn(Rigidbody rb)
    {
        // Cancel any lingering protection/constraints before moving
        if (rb.TryGetComponent<BallMovement>(out var movement)) movement.EndSpawnProtection();
        // Clear lingering velocities
        rb.linearVelocity = Vector3.zero; rb.angularVelocity = Vector3.zero;
        // Calculate target position
        Vector3 targetPoint = Vector3.up * 2f;
        if (spawnAreaCollider != null) targetPoint = spawnAreaCollider.ClosestPoint(rb.position);
        targetPoint.y += verticalOffset;
        // Force both Transform and Rigidbody to the new coordinates
        rb.transform.position = targetPoint; rb.position = targetPoint;
        // Tell PhysX to sync immediately so the new coordinates are locked in
        Physics.SyncTransforms();
        // Now apply protection constraints at the new location
        if (movement != null) movement.ActivateSpawnProtection(3.0f);
    }
}