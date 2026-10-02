using UnityEngine;

public class BillboardToCamera : MonoBehaviour
{
    private Camera targetCamera;

    private void Awake()
    {
        targetCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        if (targetCamera != null)
        {
            // Face the camera lens position directly rather than flat forward
            Vector3 dirToCam = targetCamera.transform.position - transform.position;
            if (dirToCam.sqrMagnitude > 0.001f)
            {
                // Unity Quads face backward along local -Z by default
                transform.rotation = Quaternion.LookRotation(-dirToCam.normalized, targetCamera.transform.up);
            }
        }
    }
}