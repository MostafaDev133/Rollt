using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerSetup : MonoBehaviour
{
    int index; PlayerInput playerInput;
    [SerializeField] private MeshRenderer ballRenderer;
    private static readonly Color[] PlayerColors = new Color[]
    {
        Color.red, Color.blue, Color.green, Color.yellow,
        new Color(1f, 0.5f, 0f),    // Orange
        new Color(0.6f, 0f, 1f),    // Purple
        Color.cyan, Color.magenta
    };
    public PlayerHUD AssignedHUD { get; private set; }

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }
    private void Start()
    {
        index = playerInput.playerIndex;
        // 1. Pick player color
        Color assignedColor = (index >= 0 && index < PlayerColors.Length) ? PlayerColors[index] : Color.white;
        if (ballRenderer != null)
        {
            ballRenderer.material.color = assignedColor;
        }
        // 3. Spawn and bind the HUD for this specific player
        if (HUDManager.Instance != null)
        {
            AssignedHUD = HUDManager.Instance.CreatePlayerHUD(index, assignedColor);
        }
    }
}