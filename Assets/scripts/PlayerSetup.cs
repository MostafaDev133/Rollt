using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(PlayerInput))]
public class PlayerSetup : MonoBehaviour
{
    int index; PlayerInput playerInput;
    public PlayerHUD AssignedHUD { get; private set; }
    [SerializeField] private MeshRenderer ballRenderer;
    private static readonly Color[] PlayerColors = new Color[]
    {
        Color.red, Color.blue, Color.green, Color.yellow,
        new Color(1f, 0.5f, 0f),    // Orange
        new Color(0.6f, 0f, 1f),    // Purple
        Color.cyan, Color.magenta
    };
    private void Awake() // assign components
    {
        playerInput = GetComponent<PlayerInput>();
    }
    private void Start() // assign color to MeshRenderer & HUDManager (will edit this later)
    {
        index = playerInput.playerIndex;
        // assignedColor = PlayerColorss[index] (if index >= 0 and index < 8), else set it to white
        Color assignedColor = (index >= 0 && index < PlayerColors.Length) ? PlayerColors[index] : Color.white;
        if (ballRenderer != null) ballRenderer.material.color = assignedColor; // assign the color only if the if the player have a Mesh Renderer
        // Spawn and bind the HUD for this specific player
        if (HUDManager.Instance != null) AssignedHUD = HUDManager.Instance.CreatePlayerHUD(index, assignedColor);
    }
}