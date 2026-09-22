using UnityEngine;

public class KillZone : MonoBehaviour
{
    [SerializeField] private Transform defaultSpawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the falling object has a Rigidbody (our ball)
        if (other.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            RespawnBall(rb);
        }
    }

    private void RespawnBall(Rigidbody rb)
    {
        // 1. Zero out any falling or rolling momentum
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // 2. Relocate to spawn position
        Vector3 targetPos = defaultSpawnPoint != null
            ? defaultSpawnPoint.position
            : new Vector3(0f, 2f, 0f);

        rb.position = targetPos;
    }
}