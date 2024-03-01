using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;


public interface IFollowTarget
{
    void SetFollowTarget(Transform target);
}
public class PlatformForPlayerSpawner : ITickable, IFollowTarget
{
    private readonly IPlatformPool _platformPool;
    private readonly int _platformLenght;
    private readonly int _safeZone;
    private Transform _target;
    private float _position => _target.position.x;
    
    private Queue<IPlatform> _activePlatforms = new Queue<IPlatform>();
    private int _platformNumber;
    
    public PlatformForPlayerSpawner(IPlatformPool platformPool, PlatformsConfig config)
    {
        _platformPool = platformPool;
        _platformLenght = config.lenght;
        _safeZone = config.safeZone;
    }
    
    public void SetFollowTarget(Transform target)
    {
        _target = target;
    }
    
    public void Tick()
    {
        if (_position + _platformLenght / 2f > _platformNumber * _platformLenght + _platformLenght / 2f - _safeZone)
        {
            SpawnNextPlatform();
            if (_activePlatforms.Count > 3)
            {
                DeletePrevPlatform();
            }
        }
    }

    private void SpawnNextPlatform()
    {
        _platformNumber++;
        var platform = _platformPool.GetPlatform(PlatformType.Custom);
        platform.Position = new Vector3(_platformNumber * _platformLenght, 0, 0);
        _activePlatforms.Enqueue(platform);
    }
    private void DeletePrevPlatform()
    {
        _platformPool.ReleasePlatform(_activePlatforms.Dequeue());
    }
}