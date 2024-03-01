using Cinemachine;
using UnityEngine;


public class SimpleCamera : IFollowTarget
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