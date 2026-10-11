using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerRadialUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject radialCanvasObject;
    [SerializeField] private Image[] slotIcons;

    [Header("Position Offset")]
    [SerializeField] private Vector3 worldOffset = new Vector3(-1.2f, 1.5f, 0f);

    private Camera mainCamera;
    private PlayerInventory playerInventory;
    private PlayerInput playerInput;
    private InputAction radialAction;
    private Sprite[] emptySlotPlaceholder = new Sprite[5];

    private void Awake()
    {
        mainCamera = Camera.main;
        playerInventory = GetComponent<PlayerInventory>();
        playerInput = GetComponent<PlayerInput>();

        if (radialCanvasObject != null)
        {
            radialCanvasObject.transform.SetParent(null);
            radialCanvasObject.SetActive(false);
        }
        for (int i = 0; i < slotIcons.Length; i++)
        {
            emptySlotPlaceholder[i] = slotIcons[i].sprite;
        }
    }

    private void OnEnable()
    {
        if (playerInput != null && playerInput.actions != null)
        {
            radialAction = playerInput.actions.FindAction("ToggleRadial");
            if (radialAction != null)
            {
                radialAction.performed += OnRadialPerformed;
                radialAction.canceled += OnRadialCanceled;
            }
        }
    }

    private void OnDisable()
    {
        if (radialAction != null)
        {
            radialAction.performed -= OnRadialPerformed;
            radialAction.canceled -= OnRadialCanceled;
        }
    }

    private void Update()
    {
        if (radialCanvasObject != null && radialCanvasObject.activeSelf)
        {
            UpdateUIPosition();
            RefreshRadialIcons();
        }
    }

    private void OnRadialPerformed(InputAction.CallbackContext context)
    {
        if (radialCanvasObject != null)
        {
            radialCanvasObject.SetActive(true);
            RefreshRadialIcons();
        }
    }

    // Fired instantly when LT is released
    private void OnRadialCanceled(InputAction.CallbackContext context)
    {
        if (radialCanvasObject != null)
        {
            radialCanvasObject.SetActive(false);
        }
    }

    private void UpdateUIPosition()
    {
        if (mainCamera == null) mainCamera = Camera.main;

        radialCanvasObject.transform.position = transform.position + worldOffset;
        radialCanvasObject.transform.rotation = mainCamera.transform.rotation;
    }

    
    public void RefreshRadialIcons()
    {
        if (playerInventory == null) return;

        PowerUpCategory[] categories = {
        PowerUpCategory.Offensive,
        PowerUpCategory.Defensive,
        PowerUpCategory.Hybrid,
        PowerUpCategory.Event,
        PowerUpCategory.Passive
        };

        for (int i = 0; i < slotIcons.Length && i < categories.Length; i++)
        {
            if (slotIcons[i] == null) continue;

            // Keep ALL slots active so the pinwheel framework is always fully visible
            slotIcons[i].gameObject.SetActive(true);

            // Check if player currently holds a power-up in this category
            if (playerInventory.TryGetPowerUpIcon(categories[i], out Sprite icon))
            {
                slotIcons[i].sprite = icon;
                slotIcons[i].color = Color.white; // Full color for active power-up
            }
            else
            {
                // Slot is empty: Revert to default placeholder or clear it out
                slotIcons[i].sprite = emptySlotPlaceholder[i];

                // If you have a placeholder, keep it full color; otherwise make it faint/transparent
                slotIcons[i].color = emptySlotPlaceholder[i] != null ? Color.white : new Color(1f, 1f, 1f, 0.25f);
            }
        }
    }
}