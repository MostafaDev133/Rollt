using UnityEngine;
using System.Collections.Generic;

public class BallTouchTracker : MonoBehaviour
{
    private readonly Dictionary<Rigidbody, float> recentTouches = new Dictionary<Rigidbody, float>();
    private const float TouchWindow = 2.2f;
    private Rigidbody selfRb;

    private void Awake()
    {
        selfRb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody otherRb = collision.collider.attachedRigidbody;
        if (otherRb != null && otherRb != selfRb)
        {
            // Verify it's another player ball
            if (otherRb.GetComponent<PlayerInventory>() != null)
            {
                recentTouches[otherRb] = Time.time;
            }
        }
    }

    public List<Rigidbody> GetValidAttackers()
    {
        List<Rigidbody> validAttackers = new List<Rigidbody>();
        float currentTime = Time.time;

        List<Rigidbody> keys = new List<Rigidbody>(recentTouches.Keys);
        foreach (var rb in keys)
        {
            if (rb == null)
            {
                recentTouches.Remove(rb);
                continue;
            }

            if (currentTime - recentTouches[rb] <= TouchWindow)
            {
                validAttackers.Add(rb);
            }
            else
            {
                recentTouches.Remove(rb);
            }
        }
        return validAttackers;
    }
}