using UnityEngine;

[CreateAssetMenu(fileName = "SO_PowerShield", menuName = "PowerUps/Defensive/PowerShield")]
public class PowerShieldPowerUp : PowerUpData
{
    [Header("Shield Settings")]
    public float duration = 5.0f;

    [Header("Disruption Pulse Settings")]
    public float waveRadius = 7.0f;
    public float pushForce = 12.0f;
    public float upwardLift = 3.0f;

    [Header("Visual Effects")]
    public GameObject shieldVisualPrefab; // Your hollow Blender sphere mesh
    public GameObject pulseVfxPrefab;      // Expanding wave particle effect

    public override void Activate(GameObject user)
    {
        base.Activate(user);

        var emitter = user.AddComponent<PowerShieldEmitter>();
        emitter.Initialize(this);
    }
}