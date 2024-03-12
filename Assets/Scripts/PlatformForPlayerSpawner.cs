using System.Collections.Generic;
using Core;
using UnityEngine;
using VContainer.Unity;


public interface IFollowTarget
{
    void SetFollowTarget(Transform target);
}
public class PlatformForPlayerSpawner : ITickable, IFollowTarget, IResettable
{
    private readonly IPlatformPool _platformPool;
    private readonly int _platformLenght;
    private readonly int _safeZone;

    private Transform _followTarget;
    private float Position => _followTarget.position.x;
    
    private readonly Queue<IPlatform> _activePlatforms = new Queue<IPlatform>();
    private int _platformNumber;
    
    public PlatformForPlayerSpawner(IPlatformPool platformPool, PlatformsConfig config)
    {
        _platformPool = platformPool;
        _platformLenght = config.lenght;
        _safeZone = config.safeZone;
    }
    
    public void SetFollowTarget(Transform target)
    {
        _followTarget = target;
    }
    
    public void Tick()
    {
        if (Position + _platformLenght / 2f > _platformNumber * _platformLenght + _platformLenght / 2f - _safeZone)
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
        var platform = _platformPool.GetPlatform(GetRandomPlatform());
        platform.Position = new Vector3(_platformNumber * _platformLenght, 0, 0);
        _activePlatforms.Enqueue(platform);
    }

    private PlatformType GetRandomPlatform()
    {
        var value = Random.Range(1, 5);
        return value switch
        {
            1 => PlatformType.Custom,
            2 => PlatformType.Frog,
            3 => PlatformType.FrogSlime,
            4 => PlatformType.Enemy,
            _ => PlatformType.Custom
        };
    }
    
    private void DeletePrevPlatform()
    {
        _platformPool.ReleasePlatform(_activePlatforms.Dequeue());
    }

    public void Reset()
    {
        _platformNumber = 0;
        foreach (var platform in _activePlatforms)
        {
            _platformPool.ReleasePlatform(platform);
        }
        _activePlatforms.Clear();
    }


}