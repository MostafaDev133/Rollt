using UnityEngine;
public class BillboardToCamera : MonoBehaviour
{
    private Camera targetCamera;
    private void Awake() { targetCamera = Camera.main; }
    private void LateUpdate()
    {
        if (targetCamera != null)
        {
            Vector3 dirToCam = targetCamera.transform.position - transform.position;
            if (dirToCam.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(-dirToCam.normalized, targetCamera.transform.up);
            }
        } 
        else { targetCamera = Camera.main; }
    }
}