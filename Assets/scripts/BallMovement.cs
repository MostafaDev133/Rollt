using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class BallMovement : MonoBehaviour
{
    private Rigidbody rb;
    private Vector2 moveInput;

    [Header("Physics Tuning")]
    public float rollTorque = 15f;
    public float directForce = 10f, maxAngularVelocity = 25f, extraGravity = 25f;
    public bool IsProtected { get; private set; }
    private Coroutine protectionCoroutine;

    [Header("Visual Effects")]
    [Tooltip("Child GameObject showing the glowing purple protection ring")]
    [SerializeField] private GameObject protectionVisual;
    [SerializeField] private float ringSpinSpeed = 180f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = maxAngularVelocity;

        if (protectionVisual != null)
            protectionVisual.SetActive(false);
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        if (IsProtected && moveInput.sqrMagnitude > 0.05f) EndSpawnProtection();
    }

    public void ActivateSpawnProtection(float duration = 3.0f)
    {
        if (protectionCoroutine != null) StopCoroutine(protectionCoroutine);
        protectionCoroutine = StartCoroutine(SpawnProtectionRoutine(duration));
    }

    private IEnumerator SpawnProtectionRoutine(float duration)
    {
        IsProtected = true;
        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;

        if (protectionVisual != null)
            protectionVisual.SetActive(true);

        yield return new WaitForSeconds(duration);

        EndSpawnProtection();
    }

    public void EndSpawnProtection()
    {
        if (!IsProtected) return;
        IsProtected = false;

        if (rb != null) rb.constraints = RigidbodyConstraints.None;

        if (protectionVisual != null)
            protectionVisual.SetActive(false);

        if (protectionCoroutine != null)
        {
            StopCoroutine(protectionCoroutine);
            protectionCoroutine = null;
        }

        Debug.Log($"{name} spawn protection ended.");
    }

    private void Update()
    {
        // Slowly spin the ring around the ball while protected
        if (IsProtected && protectionVisual != null)
        {
            protectionVisual.transform.Rotate(Vector3.up, ringSpinSpeed * Time.deltaTime, Space.World);
        }
    }

    private void FixedUpdate()
    {
        if (IsProtected) return;
        if (moveInput.sqrMagnitude < 0.01f) return;

        Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);
        rb.AddForce(moveDirection * directForce, ForceMode.Force);

        Vector3 torqueAxis = Vector3.Cross(Vector3.up, moveDirection);
        rb.AddTorque(torqueAxis * rollTorque, ForceMode.Force);

        rb.AddForce(Vector3.down * extraGravity, ForceMode.Acceleration);
    }

    private void OnDisable()
    {
        if (rb != null) rb.constraints = RigidbodyConstraints.None;
        if (protectionVisual != null) protectionVisual.SetActive(false);
    }
}