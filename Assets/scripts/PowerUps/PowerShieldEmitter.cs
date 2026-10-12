using System.Collections;
using UnityEngine;

public class PowerShieldEmitter : MonoBehaviour
{
    private PowerShieldPowerUp config;
    private BallMovement movement;
    private PlayerInventory inventory;
    private GameObject spawnedShieldVisual;

    public void Initialize(PowerShieldPowerUp powerUpConfig)
    {
        config = powerUpConfig;
        movement = GetComponent<BallMovement>();
        inventory = GetComponent<PlayerInventory>();

        StartCoroutine(ShieldRoutine());
    }

    private IEnumerator ShieldRoutine()
    {
        // 1. Fire the initial Disruption Pulse
        EmitDisruptionPulse();

        // 2. Enable active shield state
        SetShieldState(true);

        // 3. Instantiate hollow Blender sphere as a child object
        if (config.shieldVisualPrefab != null)
        {
            spawnedShieldVisual = Instantiate(config.shieldVisualPrefab, transform.position, Quaternion.identity, transform);
        }

        yield return new WaitForSeconds(config.duration);

        CleanupAndReset();
    }

    private void EmitDisruptionPulse()
    {
        Vector3 origin = transform.position;

        // Spawn expanding wave VFX
        if (config.pulseVfxPrefab != null)
        {
            Instantiate(config.pulseVfxPrefab, origin, Quaternion.identity);
        }

        Collider[] hits = Physics.OverlapSphere(origin, config.waveRadius);

        foreach (Collider hit in hits)
        {
            if (hit.attachedRigidbody == null || hit.attachedRigidbody.gameObject == gameObject)
                continue;

            // Ignore target if they ALREADY have a shield or spawn protection active
            if (hit.attachedRigidbody.TryGetComponent<BallMovement>(out var targetMovement) && targetMovement.IsImmuneToPowers)
                continue;

            // Break target out of active power-ups (e.g., interrupt Railgun charging)
            if (hit.attachedRigidbody.TryGetComponent<MagneticRailgunEmitter>(out var railgun))
            {
                railgun.CancelPower();
            }
            if (hit.attachedRigidbody.TryGetComponent<RepulsionWaveEmitter>(out var wave))
            {
                wave.CancelWave();
            }

            // Apply mild knockback vector
            Vector3 direction = (hit.attachedRigidbody.position - origin).normalized;
            direction.y = 0f;

            Vector3 push = (direction * config.pushForce);
            push.y += config.upwardLift;

            hit.attachedRigidbody.AddForce(push, ForceMode.Impulse);
        }
    }

    private void SetShieldState(bool state)
    {
        if (movement != null)
        {
            // Toggle shield property on movement script
            var field = typeof(BallMovement).GetProperty("IsShielded");
            if (field != null) field.SetValue(movement, state);
        }
    }

    public void BreakShield()
    {
        StopAllCoroutines();
        CleanupAndReset();
    }

    private void CleanupAndReset()
    {
        SetShieldState(false);

        if (spawnedShieldVisual != null)
        {
            Destroy(spawnedShieldVisual);
        }

        if (inventory != null)
        {
            inventory.SetPowerBusy(false);
        }

        Destroy(this);
    }
}