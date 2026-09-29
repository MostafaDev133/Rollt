using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [Header("Player Details")]
    [SerializeField] private Image playerBadgeImage;
    [SerializeField] private TextMeshProUGUI playerNameText;

    [Header("4-Category Slot Icons")]
    [SerializeField] private Image offensiveIcon;
    [SerializeField] private Image defensiveIcon;
    [SerializeField] private Image hybridIcon;
    [SerializeField] private Image eventIcon;

    [Header("Slot Selection Outlines / Highlights")]
    [SerializeField] private GameObject offensiveSelectBorder;
    [SerializeField] private GameObject defensiveSelectBorder;
    [SerializeField] private GameObject hybridSelectBorder;
    [SerializeField] private GameObject eventSelectBorder;

    [Header("Defaults")]
    [SerializeField] private Sprite emptySlotSprite;

    public void Initialize(int playerIndex, Color playerColor)
    {
        if (playerNameText != null)
            playerNameText.text = $"P{playerIndex + 1}";

        if (playerBadgeImage != null)
            playerBadgeImage.color = playerColor;

        ClearAllSlots();
        SetSelectedSlot(null);
    }

    public void UpdateSlot(PowerUpCategory category, Sprite icon)
    {
        Image targetImage = GetSlotImage(category);
        if (targetImage == null) return;

        if (icon != null)
        {
            targetImage.sprite = icon;
            targetImage.color = Color.white;
        }
        else
        {
            targetImage.sprite = emptySlotSprite;
            targetImage.color = new Color(1f, 1f, 1f, 0.2f);
        }
    }

    public void SetSelectedSlot(PowerUpCategory? selectedCategory)
    {
        if (offensiveSelectBorder != null) offensiveSelectBorder.SetActive(selectedCategory == PowerUpCategory.Offensive);
        if (defensiveSelectBorder != null) defensiveSelectBorder.SetActive(selectedCategory == PowerUpCategory.Defensive);
        if (hybridSelectBorder != null) hybridSelectBorder.SetActive(selectedCategory == PowerUpCategory.Hybrid);
        if (eventSelectBorder != null) eventSelectBorder.SetActive(selectedCategory == PowerUpCategory.Event);
    }

    public void ClearAllSlots()
    {
        UpdateSlot(PowerUpCategory.Offensive, null);
        UpdateSlot(PowerUpCategory.Defensive, null);
        UpdateSlot(PowerUpCategory.Hybrid, null);
        UpdateSlot(PowerUpCategory.Event, null);
    }

    private Image GetSlotImage(PowerUpCategory category)
    {
        switch (category)
        {
            case PowerUpCategory.Offensive: return offensiveIcon;
            case PowerUpCategory.Defensive: return defensiveIcon;
            case PowerUpCategory.Hybrid: return hybridIcon;
            case PowerUpCategory.Event: return eventIcon;
            default: return null;
        }
    }
}