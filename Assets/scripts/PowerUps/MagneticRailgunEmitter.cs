using System.Collections;
using UnityEngine;

public class MagneticRailgunEmitter : MonoBehaviour
{
    private MagneticRailgunPowerUp config;
    private Rigidbody selfRb;
    private PlayerAim playerAim;
    private PlayerInventory inventory;

    private float originalMass;
    private Vector3 lockedAimDirection;
    private Vector3 currentAimDirection;
    private GameObject spawnedConeVisual;
    private Material coneMaterialInstance;

    public void Initialize(MagneticRailgunPowerUp powerUpConfig)
    {
        config = powerUpConfig;
        selfRb = GetComponent<Rigidbody>();
        playerAim = GetComponent<PlayerAim>();
        inventory = GetComponent<PlayerInventory>();

        originalMass = selfRb.mass;

        // Capture initial aim direction
        lockedAimDirection = playerAim != null ? playerAim.AimDirection : transform.forward;
        currentAimDirection = lockedAimDirection;

        // Instantiate range decal
        if (config.coneVisualPrefab != null)
        {
            spawnedConeVisual = Instantiate(config.coneVisualPrefab, transform.position, Quaternion.identity);
            var renderer = spawnedConeVisual.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                coneMaterialInstance = renderer.material; // Unique instance for color shifting
            }
        }

        StartCoroutine(ChargeAndFireRoutine());
    }

    private IEnumerator ChargeAndFireRoutine()
    {
        float timer = 0f;

        while (timer < config.totalChargeDuration)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / config.totalChargeDuration);

            // 1. First 0.2s: Smoothly ramp mass and damp existing momentum
            if (timer <= config.slowdownRampDuration)
            {
                float rampProgress = timer / config.slowdownRampDuration;
                selfRb.mass = Mathf.Lerp(originalMass, originalMass * config.massMultiplier, rampProgress);
                selfRb.linearVelocity = Vector3.Lerp(selfRb.linearVelocity, selfRb.linearVelocity * 0.1f, Time.deltaTime * 15f);
                selfRb.angularVelocity = Vector3.Lerp(selfRb.angularVelocity, Vector3.zero, Time.deltaTime * 15f);
            }
            else
            {
                selfRb.mass = originalMass * config.massMultiplier;
            }

            // 2. Aim Clamping (+-10 degrees)
            UpdateAimDirection();

            // 3. Update Cone Visual (Narrowing, Stretching, and Color Shift)
            UpdateConeVisual(progress);

            // 4. Funnel Magnetic Pull (Physics)
            ApplyFunnelPull(progress);

            yield return null;
        }

        // Detonate
        FireDischarge();
    }

    private void UpdateAimDirection()
    {
        if (playerAim != null)
        {
            Vector3 rawStickDir = playerAim.AimDirection;
            float angle = Vector3.SignedAngle(lockedAimDirection, rawStickDir, Vector3.up);
            float clampedAngle = Mathf.Clamp(angle, -config.maxSteerAngle, config.maxSteerAngle);
            currentAimDirection = Quaternion.Euler(0f, clampedAngle, 0f) * lockedAimDirection;
        }
    }

    private void UpdateConeVisual(float progress)
    {
        if (spawnedConeVisual == null) return;

        // Snap cone position right under the ball
        Vector3 groundPos = transform.position;
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 3f))
        {
            groundPos = hit.point + Vector3.up * 0.02f;
        }
        spawnedConeVisual.transform.position = groundPos;

        // Rotate cone along clamped aim vector
        spawnedConeVisual.transform.rotation = Quaternion.LookRotation(currentAimDirection, Vector3.up);

        // Scale: starts wide/shorter, ends narrow/longer
        float currentWidth = Mathf.Lerp(1.2f, 0.25f, progress);
        float currentLength = Mathf.Lerp(0.8f, 1.4f, progress);
        spawnedConeVisual.transform.localScale = new Vector3(currentWidth, 1f, currentLength);

        // Shift color from Orange to Red/Hot Yellow
        if (coneMaterialInstance != null)
        {
            Color orange = new Color(1f, 0.5f, 0f, 0.7f);
            Color intenseRed = new Color(1f, 0.1f, 0.05f, 0.95f);
            coneMaterialInstance.color = Color.Lerp(orange, intenseRed, progress);
        }
    }

    private void ApplyFunnelPull(float progress)
    {
        float currentHalfAngle = Mathf.Lerp(config.startHalfAngle, config.endHalfAngle, progress);
        Collider[] colliders = Physics.OverlapSphere(transform.position, config.maxRange);

        foreach (Collider col in colliders)
        {
            if (col.attachedRigidbody == null || col.attachedRigidbody == selfRb) continue;

            Rigidbody enemyRb = col.attachedRigidbody;

            // Ignore spawn-protected balls
            if (enemyRb.TryGetComponent<BallMovement>(out var movement) && movement.IsImmuneToPowers) continue;

            Vector3 toEnemy = enemyRb.position - transform.position;
            toEnemy.y = 0f;
            float distance = toEnemy.magnitude;

            if (distance < 0.1f || distance > config.maxRange) continue;

            // Angle check: Is enemy inside the active cone?
            float angle = Vector3.Angle(currentAimDirection, toEnemy);
            if (angle > currentHalfAngle) continue;

            // 1. Centripetal Pull towards central aim line
            Vector3 centerLinePoint = transform.position + Vector3.Project(toEnemy, currentAimDirection);
            Vector3 toCenterLine = (centerLinePoint - enemyRb.position);
            toCenterLine.y = 0f;

            enemyRb.AddForce(toCenterLine.normalized * config.centerPullForce, ForceMode.Force);

            // 2. Gentle pull towards user ball
            enemyRb.AddForce(-toEnemy.normalized * config.inwardPullForce, ForceMode.Force);
        }
    }

    private void FireDischarge()
    {
        Vector3 origin = transform.position;

        // Spawn 3D blast VFX directly in front of the ball
        if (config.blastVfxPrefab != null)
        {
            Vector3 spawnPos = origin + (currentAimDirection * 1.2f);
            Instantiate(config.blastVfxPrefab, spawnPos, Quaternion.LookRotation(currentAimDirection));
        }

        // Apply impulse to all enemies trapped in the final narrow beam
        Collider[] targets = Physics.OverlapSphere(origin, config.maxRange);
        foreach (Collider col in targets)
        {
            if (col.attachedRigidbody == null || col.attachedRigidbody == selfRb) continue;

            Rigidbody enemyRb = col.attachedRigidbody;
            if (enemyRb.TryGetComponent<BallMovement>(out var movement) && movement.IsImmuneToPowers) continue;

            Vector3 toEnemy = enemyRb.position - origin;
            toEnemy.y = 0f;
            float distance = toEnemy.magnitude;

            if (distance < 0.1f || distance > config.maxRange) continue;

            float angle = Vector3.Angle(currentAimDirection, toEnemy);
            if (angle <= config.endHalfAngle)
            {
                // Closer to user = stronger push force
                float proximityMultiplier = 1f - Mathf.Clamp01(distance / config.maxRange);
                float finalForce = config.blastForce * Mathf.Lerp(0.4f, 1.0f, proximityMultiplier);

                Vector3 blastVector = currentAimDirection * finalForce;
                blastVector.y += config.blastUpwardLift;

                enemyRb.AddForce(blastVector, ForceMode.Impulse);
            }
        }

        // Absolute recoil on user (Backwards + Upwards)
        Vector3 recoilVector = (-currentAimDirection * config.recoilForce);
        recoilVector.y += config.recoilUpwardLift;
        selfRb.AddForce(recoilVector, ForceMode.Impulse);

        CleanupAndReset();
    }

    public void CancelPower()
    {
        StopAllCoroutines();
        CleanupAndReset();
    }

    private void CleanupAndReset()
    {
        // Restore default mass
        if (selfRb != null)
        {
            selfRb.mass = originalMass;
        }

        // Release inventory firing lock
        if (inventory != null)
        {
            inventory.SetPowerBusy(false);
        }

        if (spawnedConeVisual != null)
        {
            Destroy(spawnedConeVisual);
        }

        Destroy(this);
    }
}