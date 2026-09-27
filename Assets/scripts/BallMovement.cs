using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class BallMovement : MonoBehaviour
{
    public float rollTorque = 15f, directForce = 10f, maxAngularVelocity = 25f;
    private Rigidbody rb;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = maxAngularVelocity;
    }
    // Called automatically by the PlayerInput component when the stick moves
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void FixedUpdate()
    {
        if (moveInput.sqrMagnitude < 0.01f) return;
        // Camera-relative direction (camera pointing forward along +Z)
        Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);

        // 1. Direct Force (responsive steering)
        rb.AddForce(moveDirection * directForce, ForceMode.Force);

        // 2. Rolling Torque (physical spin)
        Vector3 torqueAxis = Vector3.Cross(Vector3.up, moveDirection);
        rb.AddTorque(torqueAxis * rollTorque, ForceMode.Force);
    }
}