using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MainMenuManager : MonoBehaviour
{
    [Header("Menu & Lobby UI")]
    [SerializeField] private GameObject sidePanel;
    [SerializeField] private GameObject firstSelectedButton;
    [SerializeField] private GameObject lobbyPromptPanel;

    [Header("Managers & Post-Processing")]
    [SerializeField] private PlayerInputManager playerInputManager;
    [SerializeField] private Volume globalVolume;

    private DepthOfField dofComponent;
    private FilmGrain filmGrainComponent;
    private bool isInLocalLobby = false;
    private float bHoldTimer = 0f;
    private const float HoldDuration = 1.0f;

    private void Start()
    {
        if (globalVolume != null && globalVolume.profile != null)
        {
            globalVolume.profile.TryGet(out dofComponent);
            //globalVolume.profile.TryGet(out filmGrainComponent);
        }

        ShowMainMenu();
    }

    private void Update()
    {
        // When in Local Lobby with 0 players, holding B (Gamepad East) or Escape returns to Main Menu
        if (isInLocalLobby && playerInputManager != null && playerInputManager.playerCount == 0)
        {
            bool holdingB = (Gamepad.current != null && Gamepad.current.buttonEast.isPressed) ||
                            (Keyboard.current != null && Keyboard.current.escapeKey.isPressed);

            if (holdingB)
            {
                bHoldTimer += Time.deltaTime;
                if (bHoldTimer >= HoldDuration)
                {
                    bHoldTimer = 0f;
                    ShowMainMenu();
                }
            }
            else
            {
                bHoldTimer = 0f;
            }
        }
    }

    public void ShowMainMenu()
    {
        isInLocalLobby = false;

        if (playerInputManager != null) playerInputManager.DisableJoining();

        if (sidePanel != null) sidePanel.SetActive(true);
        if (lobbyPromptPanel != null) lobbyPromptPanel.SetActive(false);

        // Turn on background blur & grain
        if (dofComponent != null) dofComponent.active = true;
        //if (filmGrainComponent != null) filmGrainComponent.active = true;

        // Restore D-Pad focus to LocalPlay_Button
        if (EventSystem.current != null && firstSelectedButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        }
    }

    // Connect to LocalPlay_Button -> OnClick()
    public void OnClickLocalPlay()
    {
        isInLocalLobby = true;

        if (sidePanel != null) sidePanel.SetActive(false);
        if (lobbyPromptPanel != null) lobbyPromptPanel.SetActive(true);

        // Turn off blur so the platform is sharp
        if (dofComponent != null) dofComponent.active = false;
        //if (filmGrainComponent != null) filmGrainComponent.active = false;

        if (playerInputManager != null) playerInputManager.EnableJoining();
    }

    // Connect to Quit_Button -> OnClick()
    public void OnClickQuit()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}