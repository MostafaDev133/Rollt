using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private PlayerInventory inventory;

    [Header("Power-Up Database")]
    [SerializeField] private PowerUpDatabase powerUpDatabase; // Drag your global database asset here

    [Header("Streak Counters (Read-Only Preview)")]
    [SerializeField] private int fallStreak = 0;
    [SerializeField] private int pushStreak = 0;

    private void Awake()
    {
        inventory = GetComponent<PlayerInventory>();
    }

    public void RegisterFall()
    {
        fallStreak++;
        Debug.Log($"{name} fell! Fall streak is now {fallStreak}/5");
    }

    public void RegisterPushKill()
    {
        pushStreak++;
        Debug.Log($"{name} pushed an opponent! Push streak is now {pushStreak}/5");

        if (pushStreak >= 5)
        {
            TryAwardOffensivePower();
        }
    }

    // Called by KillZone upon respawn if fallStreak >= 5
    public void CheckAndAwardComebackShield()
    {
        if (fallStreak >= 5)
        {
            if (inventory != null && powerUpDatabase != null)
            {
                // Pull a random power-up from the Defensive category
                PowerUpData randomDefensive = powerUpDatabase.GetRandomPowerUpOfCategory(PowerUpCategory.Defensive);

                if (randomDefensive != null)
                {
                    bool added = inventory.AddPowerUp(randomDefensive);
                    if (added)
                    {
                        fallStreak = 0; // Successfully rewarded, reset counter
                        Debug.Log($"{name} received a random Defensive Comeback reward: {randomDefensive.powerUpName}!");
                        return;
                    }
                }
            }

            // If slot is occupied or no defensive power exists yet, hold counter at 4
            fallStreak = 4;
            Debug.Log($"{name} defensive slot occupied or category empty. Holding fall streak at 4.");
        }
    }

    private void TryAwardOffensivePower()
    {
        if (inventory != null && powerUpDatabase != null)
        {
            // Pull a random power-up from the Offensive category
            PowerUpData randomOffensive = powerUpDatabase.GetRandomPowerUpOfCategory(PowerUpCategory.Offensive);

            if (randomOffensive != null)
            {
                bool added = inventory.AddPowerUp(randomOffensive);
                if (added)
                {
                    pushStreak = 0; // Successfully rewarded, reset counter
                    Debug.Log($"{name} received a random Offensive push reward: {randomOffensive.powerUpName}!");
                    return;
                }
            }
        }

        // If slot is occupied or category empty, hold counter at 4
        pushStreak = 4;
        Debug.Log($"{name} offensive slot occupied or category empty. Holding push streak at 4.");
    }
}