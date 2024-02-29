using Cinemachine;
using UnityEngine;

public interface ICamera
{
    void SetFollowTarget(Transform target);
}

public class SimpleCamera : ICamera
{
    private readonly CinemachineVirtualCamera _virtualCamera;
    public SimpleCamera(CinemachineVirtualCamera virtualCamera)
    {
        _virtualCamera = virtualCamera;
    }

    public void SetFollowTarget(Transform target)
    {
        _virtualCamera.Follow = target;
    }
}