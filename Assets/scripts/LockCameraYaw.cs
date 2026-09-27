using UnityEngine;
using Cinemachine;

[ExecuteAlways]
[SaveDuringPlay]
[AddComponentMenu("")] // Keeps it organized in Cinemachine extensions
public class LockCameraYaw : CinemachineExtension
{
    [SerializeField] private float lockedYawAngle = 0f;

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        // Intercept after the Aim stage finishes calculating rotation
        if (stage == CinemachineCore.Stage.Aim)
        {
            Vector3 euler = state.RawOrientation.eulerAngles;
            // Keep calculated X (vertical tilt), force Y (yaw) to fixed angle, lock Z (roll)
            euler.y = lockedYawAngle;
            euler.z = 0f;
            state.RawOrientation = Quaternion.Euler(euler);
        }
    }
}