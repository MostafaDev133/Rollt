using UnityEngine;

public class KillZone : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private Collider spawnAreaCollider;
    [SerializeField] private float verticalOffset = 1.0f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            // 1. Cancel active power emitters
            if (rb.TryGetComponent<RepulsionWaveEmitter>(out var emitter)) emitter.CancelWave();
            if (rb.TryGetComponent<MagneticRailgunEmitter>(out var railEmitter)) railEmitter.CancelPower();
            //if (rb.TryGetComponent<PowerShieldEmitter>(out var ShieldEmitter)) ShieldEmitter.BreakShield();
            if (rb.TryGetComponent<PlayerInventory>(out var inventory)) inventory.ResetActivePower();

            // 2. Process Knockout Attribution (Check who touched this ball within 2 seconds)
            if (rb.TryGetComponent<BallTouchTracker>(out var touchTracker))
            {
                var attackers = touchTracker.GetValidAttackers();
                foreach (var attackerRb in attackers)
                {
                    if (attackerRb.TryGetComponent<PlayerStats>(out var attackerStats))
                    {
                        attackerStats.RegisterPushKill();
                    }
                }
            }

            // 3. Register fall on the victim
            if (rb.TryGetComponent<PlayerStats>(out var victimStats))
            {
                victimStats.RegisterFall();
            }

            // 4. Respawn
            Respawn(rb, victimStats);
        }
    }

    private void Respawn(Rigidbody rb, PlayerStats victimStats)
    {
        if (rb.TryGetComponent<BallMovement>(out var movement)) movement.EndSpawnProtection();

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 targetPoint = Vector3.up * 2f;
        if (spawnAreaCollider != null) targetPoint = spawnAreaCollider.ClosestPoint(rb.position);
        targetPoint.y += verticalOffset;

        rb.transform.position = targetPoint;
        rb.position = targetPoint;
        Physics.SyncTransforms();

        // Check if victim earned their defensive comeback shield reward upon respawn
        if (victimStats != null)
        {
            victimStats.CheckAndAwardComebackShield();
        }

        if (movement != null) movement.ActivateSpawnProtection(3.0f);
    }
}