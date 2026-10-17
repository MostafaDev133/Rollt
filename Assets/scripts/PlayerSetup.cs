using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerSetup : MonoBehaviour
{
    int index; PlayerInput playerInput;
    [SerializeField] private MeshRenderer ballRenderer;
    private float leaveHoldTimer = 0f;

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
        Color assignedColor = (index >= 0 && index < PlayerColors.Length) ? PlayerColors[index] : Color.white;
        if (ballRenderer != null) ballRenderer.material.color = assignedColor; // assign the color only if the if the player have a Mesh Renderer
    }
    private void Update()
    {
        // Check if THIS specific player's device is holding B (East button) or Backspace
        bool isHoldingCancel = false;

        if (playerInput.currentControlScheme == "Gamepad" && playerInput.GetDevice<Gamepad>() is Gamepad pad)
        {
            isHoldingCancel = pad.buttonEast.isPressed;
        }
        else if (Keyboard.current != null)
        {
            isHoldingCancel = Keyboard.current.escapeKey.isPressed;
        }

        if (isHoldingCancel)
        {
            leaveHoldTimer += Time.deltaTime;
            if (leaveHoldTimer >= 1.0f)
            {
                Destroy(gameObject); // Despawns ball and frees device in PlayerInputManager
            }
        }
        else
        {
            leaveHoldTimer = 0f;
        }
    }
}