using System.Collections;
using UnityEngine;

public class RepulsionWaveEmitter : MonoBehaviour
{
    private RepulsionWavePowerUp config;
    private Rigidbody selfRb;

    public void Initialize(RepulsionWavePowerUp powerUpConfig)
    {
        config = powerUpConfig;
        selfRb = GetComponent<Rigidbody>();
        StartCoroutine(WaveRoutine());
    }

    private IEnumerator WaveRoutine()
    {
        for (int i = 0; i < config.totalWaves; i++)
        {
            EmitSinglePulse();
            yield return new WaitForSeconds(config.pulseInterval);
        }
        if (TryGetComponent<PlayerInventory>(out var inventory))
        {
            inventory.SetPowerBusy(false);
        }
        Destroy(this);
    }

    private void EmitSinglePulse()
    {
        Vector3 origin = transform.position;

        // Spawn the expanding ring at the ball's position (placed slightly above the floor)
        if (config.waveVfxPrefab != null)
        {
            Vector3 vfxPosition = new Vector3(origin.x, origin.y - 0.4f, origin.z);
            Instantiate(config.waveVfxPrefab, vfxPosition, Quaternion.identity);
        }

        // Find all colliders in range
        Collider[] hits = Physics.OverlapSphere(origin, config.waveRadius);

        foreach (Collider hit in hits)
        {
            // Skip self
            if (hit.attachedRigidbody == null || hit.attachedRigidbody == selfRb)
                continue;

            if (hit.attachedRigidbody.TryGetComponent<BallMovement>(out var movement) && movement.IsProtected)
                continue;

            Rigidbody targetRb = hit.attachedRigidbody;

            // Calculate directional vector away from the user
            Vector3 direction = targetRb.position - origin;
            float distance = direction.magnitude;

            if (distance < 0.001f) continue;

            direction.Normalize();

            // Calculate force: stronger when closer
            float distanceFactor = 1f - Mathf.Clamp01(distance / config.waveRadius);
            Vector3 pushForce = (direction * config.baseForce * distanceFactor);
            pushForce.y += config.upwardLift;

            // Apply immediate velocity change for crisp knockback
            targetRb.AddForce(pushForce, ForceMode.Impulse);
        }

        Debug.Log($"Pulse fired from {name}!");
    }

    // Visual aid in Scene View to see the radius
    private void OnDrawGizmosSelected()
    {
        if (config != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, config.waveRadius);
        }
    }
    public void CancelWave()
    {
        StopAllCoroutines();
        Destroy(this);
    }
}
