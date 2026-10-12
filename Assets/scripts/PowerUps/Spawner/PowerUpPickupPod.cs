using UnityEngine;

public class PowerUpPickupPod : MonoBehaviour
{
    private PowerUpCategory assignedCategory;
    private PowerUpDatabase database;
    private PowerUpSpawner spawner;

    public void Initialize(PowerUpCategory category, PowerUpDatabase db, PowerUpSpawner ownerSpawner)
    {
        assignedCategory = category;
        database = db;
        spawner = ownerSpawner;

        // Change pod color or visual indicator based on 'assignedCategory' here
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if a player ball collided with the pod
        if (other.TryGetComponent<PlayerInventory>(out PlayerInventory inventory))
        {
            // Resolve exact power-up from the pre-decided category
            Debug.Log("Touched Pod");
            PowerUpData awardedPower = database.GetRandomPowerUpOfCategory(assignedCategory);

            if (awardedPower != null)
            {
                // Use your existing AddPowerUp method

                Debug.Log("Successfully the pod got a power");
                bool success = inventory.AddPowerUp(awardedPower);

                if (success)
                {
                    Debug.Log("added a power");
                    // Notify spawner and destroy pod only if successfully added
                    if (spawner != null)
                    {
                        Debug.Log("got a spawner");
                        spawner.OnPodCollected();
                    }

                    Destroy(gameObject);
                }
            }
        }
    }
}