using UnityEngine;

// [CreateAssetMenu(fileName = "RepulsionWavePowerUp", menuName = "PowerUps/Hybrid/RepulsionWave")]
public class RepulsionWavePowerUp : PowerUpData
{
    [Header("Wave Settings")]
    [Tooltip("Total number of outward pulses")]
    public int totalWaves = 6;

    [Tooltip("Time between pulses (0.5s = 2 waves per second)")]
    public float pulseInterval = 0.5f;

    [Tooltip("Effective radius around the ball")]
    public float waveRadius = 6.0f;

    [Header("Force Tuning")]
    [Tooltip("Base outward impulse strength")]
    public float baseForce = 25f;

    [Tooltip("Upward pop force to break ground friction")]
    public float upwardLift = 5f;

    [Header("Visual Effects")]
    public GameObject waveVfxPrefab;

    public override void Activate(GameObject user) // understand this function
    {
        base.Activate(user); 

        // Attach an emitter to the user ball to execute the wave loop
        var emitter = user.AddComponent<RepulsionWaveEmitter>();
        emitter.Initialize(this);
    }
}