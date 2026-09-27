using UnityEngine;
using Cinemachine;

public class AutoTargetGroupMember : MonoBehaviour
{
    public float targetWeight = 1f;
    public float targetRadius = 1.2f;
    private CinemachineTargetGroup targetGroup;

    private void Start()
    {
        targetGroup = FindFirstObjectByType<CinemachineTargetGroup>();

        if (targetGroup != null)
        {
            targetGroup.AddMember(transform, targetWeight, targetRadius);
        }
    }

    private void OnDestroy()
    {
        if (targetGroup != null)
        {
            targetGroup.RemoveMember(transform);
        }
    }
}