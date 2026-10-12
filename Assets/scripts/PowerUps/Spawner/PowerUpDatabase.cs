using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PowerUpDatabase", menuName = "PowerUps/Database")]
public class PowerUpDatabase : ScriptableObject
{
    [System.Serializable]
    public struct CategorizedPowerUp
    {
        public PowerUpCategory category;
        public PowerUpData powerUpData;
    }

    public List<CategorizedPowerUp> allPowerUps = new List<CategorizedPowerUp>();

    public bool HasPowerUpsInCategory(PowerUpCategory category)
    {
        foreach (var item in allPowerUps)
        {
            if (item.category == category && item.powerUpData != null)
            {
                return true;
            }
        }
        return false;
    }
    // Helper method to grab a random power-up of a specific category upon pickup
    public PowerUpData GetRandomPowerUpOfCategory(PowerUpCategory category)
    {
        List<PowerUpData> matchingPool = new List<PowerUpData>();

        foreach (var item in allPowerUps)
        {
            if (item.category == category && item.powerUpData != null)
            {
                matchingPool.Add(item.powerUpData);
            }
        }

        if (matchingPool.Count == 0) return null;

        int randomIndex = Random.Range(0, matchingPool.Count);
        return matchingPool[randomIndex];
    }
}