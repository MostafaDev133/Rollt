using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class PlayerScript : MonoBehaviour
{
    public float rollTorque = 15f;
    public float directForce = 10f;
    public float maxAngularVelocity = 25f;
    private Rigidbody rb;
    private Vector2 inputDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = maxAngularVelocity;
    }

    private void Update()
    {
        // Simple input check for rapid testing (WASD / Left Stick)
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        inputDirection = new Vector2(h, v).normalized;
    }

    private void FixedUpdate()
    {
        if (inputDirection.sqrMagnitude < 0.01f) return;

        // Standard isometric / top-down fixed perspective (camera pointing along +Z, tilted down)
        Vector3 moveDirection = new Vector3(inputDirection.x, 0f, inputDirection.y);

        // 1. Direct Force: gives snappy steering response
        rb.AddForce(moveDirection * directForce, ForceMode.Force);

        // 2. Torque: applies genuine physical rolling spin perpendicular to move direction
        Vector3 torqueAxis = Vector3.Cross(Vector3.up, moveDirection);
        rb.AddTorque(torqueAxis * rollTorque, ForceMode.Force);
    }
}