using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAim : MonoBehaviour
{
    [SerializeField] private LineRenderer aimLine;
    [SerializeField] private float lineLength = 2.5f;
    [SerializeField] private LayerMask groundLayer = ~0; // Detects all layers by default

    private Vector2 rightStickInput;
    private Vector2 leftStickInput;
    private Vector3 currentAimDirection = Vector3.forward;
    private Camera mainCamera;

    public Vector3 AimDirection => currentAimDirection;
    public bool IsAimingWithRightStick => rightStickInput.sqrMagnitude > 0.05f;

    private void Awake()
    {
        mainCamera = Camera.main;

        if (aimLine != null)
        {
            aimLine.useWorldSpace = true;
            aimLine.enabled = false; // Hidden by default
        }
    }

    public void OnLook(InputValue value)
    {
        rightStickInput = value.Get<Vector2>();
    }

    public void OnMove(InputValue value)
    {
        leftStickInput = value.Get<Vector2>();
    }

    private void Update()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        CalculateAimDirection();
        UpdateLineVisual();
    }

    private void CalculateAimDirection()
    {
        Vector2 activeInput = Vector2.zero;

        // 1. Right stick takes primary aim priority
        if (rightStickInput.sqrMagnitude > 0.05f)
        {
            activeInput = rightStickInput;
        }
        // 2. Fallback: Left stick (movement)
        else if (leftStickInput.sqrMagnitude > 0.05f)
        {
            activeInput = leftStickInput;
        }

        // Convert stick input to flat camera-relative world direction
        if (activeInput.sqrMagnitude > 0.05f && mainCamera != null)
        {
            Vector3 camForward = mainCamera.transform.forward;
            Vector3 camRight = mainCamera.transform.right;

            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            currentAimDirection = (camRight * activeInput.x + camForward * activeInput.y).normalized;
        }
    }

    private void UpdateLineVisual()
    {
        if (aimLine == null) return;

        // The line ONLY appears when actively pushing the Right Stick
        if (!IsAimingWithRightStick)
        {
            if (aimLine.enabled) aimLine.enabled = false;
            return;
        }

        if (!aimLine.enabled) aimLine.enabled = true;

        // Find the floor directly below the ball to avoid clipping through the platform
        Vector3 groundOrigin = transform.position;
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 4f, groundLayer))
        {
            groundOrigin = hit.point + Vector3.up * 0.03f; // Snaps cleanly right on top of the platform
        }
        else
        {
            groundOrigin.y -= 0.6f; // Fallback if airborne
        }

        Vector3 targetPoint = groundOrigin + currentAimDirection * lineLength;

        aimLine.SetPosition(0, groundOrigin);
        aimLine.SetPosition(1, targetPoint);
    }
}