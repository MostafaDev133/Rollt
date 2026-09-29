using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    private readonly Dictionary<PowerUpCategory, PowerUpData> heldPowerUps =
        new Dictionary<PowerUpCategory, PowerUpData>();

    private PowerUpCategory? currentlySelectedCategory = null;
    private PlayerHUD playerHUD;

    [SerializeField] private PowerUpData debugPowerUp;

    public bool IsFiringPower { get; private set; }

    public void SetPowerBusy(bool busy)
    {
        IsFiringPower = busy;
    }

    public void ResetActivePower()
    {
        IsFiringPower = false;
    }
    private void Start()
    {
        if (TryGetComponent<PlayerSetup>(out var setup))
        {
            playerHUD = setup.AssignedHUD;
        }
    }

    private void Update()
    {
        // Temporary test key: Press 'T' to grant the power-up to this ball
        if ((Input.GetKeyDown(KeyCode.T) || (Input.GetKeyDown(KeyCode.JoystickButton4))) && debugPowerUp != null )
        {
            AddPowerUp(debugPowerUp);
            Debug.Log("Granted test power-up!");
        }
    }

    public bool AddPowerUp(PowerUpData powerUp)
    {
        if (powerUp == null) return false;

        // If slot is occupied, ignore or swap
        if (heldPowerUps.ContainsKey(powerUp.category))
        {
            return false;
        }

        heldPowerUps[powerUp.category] = powerUp;

        // Auto-select this category if nothing is selected yet
        if (!currentlySelectedCategory.HasValue)
        {
            SelectSlot(powerUp.category);
        }

        if (playerHUD != null)
        {
            playerHUD.UpdateSlot(powerUp.category, powerUp.icon);
        }

        return true;
    }

    private void SelectSlot(PowerUpCategory category)
    {
        currentlySelectedCategory = category;

        if (playerHUD != null)
        {
            playerHUD.SetSelectedSlot(currentlySelectedCategory);
        }
    }

    // Fired by RB / RT / Spacebar
    public void OnFire(InputValue value)
    {
        Debug.Log($"--- OnFire received! Currently selected slot: {(currentlySelectedCategory.HasValue ? currentlySelectedCategory.Value.ToString() : "None")} ---");
        if (!value.isPressed) return;
        // Block firing if another power is already running
        if (IsFiringPower)
        {
            Debug.Log("Cannot fire: another power-up is active!");
            return;
        }

        if (currentlySelectedCategory.HasValue && heldPowerUps.TryGetValue(currentlySelectedCategory.Value, out PowerUpData powerUp))
        {
            Debug.Log($"Firing power-up: {powerUp.powerUpName}!");
            IsFiringPower = true;
            powerUp.Activate(gameObject);
            heldPowerUps.Remove(currentlySelectedCategory.Value);

            if (playerHUD != null)
            {
                playerHUD.UpdateSlot(currentlySelectedCategory.Value, null);
            }

            // Auto-shift selection to another held item if available
            AutoSelectNextAvailable();
        }
        else
        {
            Debug.LogWarning("Fire was pressed, but either no slot is selected or the selected slot is empty!");
        }
    }

    private void AutoSelectNextAvailable()
    {
        foreach (var key in heldPowerUps.Keys)
        {
            SelectSlot(key);
            return;
        }

        currentlySelectedCategory = null;
        if (playerHUD != null)
        {
            playerHUD.SetSelectedSlot(null);
        }
    }

    // Face buttons now select the active slot
    public void OnUseOffensive(InputValue value)
    {
        Debug.Log("--- OnUseOffencsive received! Selecting Hybrid slot ---");
        if (value.isPressed) SelectSlot(PowerUpCategory.Offensive);
    }

    public void OnUseDefensive(InputValue value)
    {
        if (value.isPressed) SelectSlot(PowerUpCategory.Defensive);
    }

    public void OnUseHybrid(InputValue value)
    {
        Debug.Log("--- OnUseHybrid received! Selecting Hybrid slot ---");
        if (value.isPressed) SelectSlot(PowerUpCategory.Hybrid);
    }

    public void OnUseEvent(InputValue value)
    {
        if (value.isPressed) SelectSlot(PowerUpCategory.Event);
    }
}