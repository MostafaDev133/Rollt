using UnityEngine;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    [SerializeField] private Transform hudContainer;
    [SerializeField] private GameObject playerHudPrefab;

    private void Awake()
    {
        Instance = this;
    }

    public PlayerHUD CreatePlayerHUD(int playerIndex, Color playerColor)
    {
        if (playerHudPrefab == null || hudContainer == null) return null;

        GameObject hudObj = Instantiate(playerHudPrefab, hudContainer);
        PlayerHUD hud = hudObj.GetComponent<PlayerHUD>();
        hud.Initialize(playerIndex, playerColor);
        return hud;
    }
}