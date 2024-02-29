using Cinemachine;
using UnityEngine;

public interface ICamera
{
    void SetFollowTarget(Transform target);
}

public class SimpleCamera : ICamera
{
    private CinemachineVirtualCamera _virtualCamera;
    public void SetFollowTarget(Transform target)
    {
        _virtualCamera.Follow = target;
    }
}