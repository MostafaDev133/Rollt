using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PowerUpSpawner : MonoBehaviour
{
    [Header("Database & Prefabs")]
    [SerializeField] private PowerUpDatabase powerUpDatabase;
    [SerializeField] private GameObject pickupPodPrefab; // Visual representation spawned in the arena

    [Header("Spawn Settings")]
    [Tooltip("Time bounds between spawns (10 to 20 seconds)")]
    [SerializeField] private float minSpawnTime = 10f;
    [SerializeField] private float maxSpawnTime = 20f;

    [Header("Category Probabilities (Weights)")]
    [Tooltip("Higher weight = higher chance to spawn")]
    [SerializeField] private float offensiveWeight = 40f;
    [SerializeField] private float defensiveWeight = 30f;
    [SerializeField] private float hybridWeight = 20f;
    [SerializeField] private float eventWeight = 10f; // Rare!

    [Header("Spawn Area")]
    [SerializeField] private MeshCollider spawnAreaMesh; // Your custom mesh spawn zone

    private GameObject currentSpawnedPod;
    private bool isWaitingToSpawn = false;

    private void Start()
    {
        if (spawnAreaMesh == null)
        {
            spawnAreaMesh = GetComponent<MeshCollider>();
        }

        StartCoroutine(SpawnTimerRoutine());
    }

    private IEnumerator SpawnTimerRoutine()
    {
        while (true)
        {
            if (/*currentSpawnedPod == null &&*/ !isWaitingToSpawn)
            {
                isWaitingToSpawn = true;
                float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
                yield return new WaitForSeconds(waitTime);

                SpawnPowerUpPod();
                isWaitingToSpawn = false;
            }

            yield return null;
        }
    }

    private void SpawnPowerUpPod()
    {
        if (powerUpDatabase == null || pickupPodPrefab == null) return;

        // 1. Determine Category dynamically based on what's actually available
        PowerUpCategory? chosenCategory = RollWeightedCategory();
        if (!chosenCategory.HasValue) return; // Skip spawning if no valid categories exist

        Vector3 spawnPosition = GetRandomPointInMeshCollider(spawnAreaMesh);

        // 2. Instantiate the pod in the arena
        currentSpawnedPod = Instantiate(pickupPodPrefab, spawnPosition, Quaternion.identity);

        var podScript = currentSpawnedPod.GetComponent<PowerUpPickupPod>();
        if (podScript != null)
        {
            podScript.Initialize(chosenCategory.Value, powerUpDatabase, this);
        }
    }

    private PowerUpCategory? RollWeightedCategory()
    {
        // Lists to hold only categories that actually have power-ups in the database
        List<PowerUpCategory> validCategories = new List<PowerUpCategory>();
        List<float> weights = new List<float>();

        if (powerUpDatabase.HasPowerUpsInCategory(PowerUpCategory.Offensive))
        {
            validCategories.Add(PowerUpCategory.Offensive);
            weights.Add(offensiveWeight);
        }
        if (powerUpDatabase.HasPowerUpsInCategory(PowerUpCategory.Defensive))
        {
            validCategories.Add(PowerUpCategory.Defensive);
            weights.Add(defensiveWeight);
        }
        if (powerUpDatabase.HasPowerUpsInCategory(PowerUpCategory.Hybrid))
        {
            validCategories.Add(PowerUpCategory.Hybrid);
            weights.Add(hybridWeight);
        }
        if (powerUpDatabase.HasPowerUpsInCategory(PowerUpCategory.Event))
        {
            validCategories.Add(PowerUpCategory.Event);
            weights.Add(eventWeight);
        }
        // If you add Passive or custom categories later, they are automatically supported here!

        // Safety check: If all categories are empty, don't try to roll
        if (validCategories.Count == 0) return null;

        float totalWeight = 0f;
        foreach (float w in weights)
        {
            totalWeight += w;
        }

        float roll = Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;

        for (int i = 0; i < validCategories.Count; i++)
        {
            cumulativeWeight += weights[i];
            if (roll < cumulativeWeight)
            {
                return validCategories[i];
            }
        }

        return validCategories[0]; // Fallback
    }

    private Vector3 GetRandomPointInMeshCollider(MeshCollider meshCollider)
    {
        if (meshCollider == null) return transform.position;

        Bounds bounds = meshCollider.bounds;
        for (int i = 0; i < 30; i++) // Try up to 30 times to find a valid point inside
        {
            Vector3 randomPoint = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                bounds.max.y + 1f, // Drop from slightly above
                Random.Range(bounds.min.z, bounds.max.z)
            );

            if (Physics.Raycast(randomPoint, Vector3.down, out RaycastHit hit, 10f))
            {
                if (hit.collider == meshCollider)
                {
                    return hit.point + Vector3.up * 0.2f; // Slight offset above floor
                }
            }
        }

        return transform.position; // Fallback to spawner center
    }

    public void OnPodCollected()
    {
        currentSpawnedPod = null;
    }
}