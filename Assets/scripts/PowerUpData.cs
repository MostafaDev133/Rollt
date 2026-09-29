using UnityEngine;

[CreateAssetMenu(fileName = "NewPowerUp", menuName = "PowerUps/PowerUpData")]
public class PowerUpData : ScriptableObject
{
    public string powerUpName = "Power-Up";
    public PowerUpCategory category;
    public Sprite icon;

    // Abstract or virtual execution method
    public virtual void Activate(GameObject user)
    {
        Debug.Log($"{user.name} activated {powerUpName} ({category})!");
    }
}