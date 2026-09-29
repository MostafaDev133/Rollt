using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class BallMovement : MonoBehaviour
{
    public float rollTorque = 15f, directForce = 10f, maxAngularVelocity = 25f;
    private Rigidbody rb;
    private Vector2 moveInput;

    public bool IsProtected { get; private set; }
    private Coroutine protectionCoroutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = maxAngularVelocity;
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();

        // If player touches the stick/WASD during protection, immediately break it
        if (IsProtected && moveInput.sqrMagnitude > 0.05f)
        {
            EndSpawnProtection();
        }
    }

    public void ActivateSpawnProtection(float duration = 3.0f)
    {
        if (protectionCoroutine != null)
            StopCoroutine(protectionCoroutine);

        protectionCoroutine = StartCoroutine(SpawnProtectionRoutine(duration));
    }

    private IEnumerator SpawnProtectionRoutine(float duration)
    {
        IsProtected = true;

        // Freeze X and Z movement so opponents cannot push the ball, but keep Y free for gravity landing
        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;

        yield return new WaitForSeconds(duration);

        EndSpawnProtection();
    }

    public void EndSpawnProtection()
    {
        if (!IsProtected) return;

        IsProtected = false;

        // Restore completely free movement
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.None;
        }

        if (protectionCoroutine != null)
        {
            StopCoroutine(protectionCoroutine);
            protectionCoroutine = null;
        }

        Debug.Log($"{name} spawn protection ended.");
    }

    private void FixedUpdate()
    {
        // Don't apply manual movement force while locked in spawn freeze
        if (IsProtected) return;

        if (moveInput.sqrMagnitude < 0.01f) return;

        // Camera-relative direction (camera pointing forward along +Z)
        Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);

        // 1. Direct Force (responsive steering)
        rb.AddForce(moveDirection * directForce, ForceMode.Force);

        // 2. Rolling Torque (physical spin)
        Vector3 torqueAxis = Vector3.Cross(Vector3.up, moveDirection);
        rb.AddTorque(torqueAxis * rollTorque, ForceMode.Force);
    }

    private void OnDisable()
    {
        if (rb != null)
            rb.constraints = RigidbodyConstraints.None;
    }
}