using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;

public class PlatformForPlayerSpawner : ITickable
{
    private readonly IPlatformPool _platformPool;
    private readonly ICharacterContainer _character;
    private readonly int _platformLenght;
    private readonly int _safeZone;
    private float _position => _character.Rigidbody.position.x;
    
    private Queue<IPlatform> _activePlatforms = new Queue<IPlatform>();
    private int _platformNumber;
    

    public PlatformForPlayerSpawner(IPlatformPool platformPool, PlatformsConfig config, ICharacterContainer character)
    {
        _platformPool = platformPool;
        _platformLenght = config.lenght;
        _safeZone = config.safeZone;
        _character = character;
    }
    

    public void Tick()
    {
        if (_position + _platformLenght / 2f > _platformNumber * _platformLenght + _platformLenght / 2f - _safeZone)
        {
            SpawnNextPlatform();
            if (_activePlatforms.Count > 2)
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