using UnityEngine;

[CreateAssetMenu(fileName = "SO_MagneticRailgun", menuName = "PowerUps/Offensive/MagneticRailgun")]
public class MagneticRailgunPowerUp : PowerUpData
{
    [Header("Timings")]
    public float totalChargeDuration = 1.0f;
    public float slowdownRampDuration = 0.2f;

    [Header("Charge Weight & Mass")]
    [Tooltip("Multiplier for Rigidbody mass during charge (e.g. 8x to 10x)")]
    public float massMultiplier = 8.0f;

    [Header("Aim Steer Limit")]
    [Tooltip("Maximum degrees the player can steer left/right from the initial lock")]
    public float maxSteerAngle = 10.0f;

    [Header("Range & Cone Dimensions")]
    public float maxRange = 12.0f;
    public float startHalfAngle = 35.0f; // Wide cone at 0s
    public float endHalfAngle = 8.0f;    // Narrow focused beam at 1s

    [Header("Magnetic Pull")]
    public float centerPullForce = 25.0f;
    public float inwardPullForce = 8.0f;

    [Header("Discharge Forces")]
    public float blastForce = 90.0f;
    public float blastUpwardLift = 12.0f;
    public float recoilForce = 35.0f;
    public float recoilUpwardLift = 8.0f;

    [Header("Visual Effects")]
    [Tooltip("Floor cone indicator prefab")]
    public GameObject coneVisualPrefab;
    [Tooltip("3D blast particle spawned upon firing")]
    public GameObject blastVfxPrefab;

    public override void Activate(GameObject user)
    {
        base.Activate(user);

        var emitter = user.AddComponent<MagneticRailgunEmitter>();
        emitter.Initialize(this);
    }
}