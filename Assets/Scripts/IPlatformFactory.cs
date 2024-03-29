using System;
using System.Linq;
using Custom;
using UnityEngine;

public interface IPlatform : IResettable
{
    PlatformType Type { get; }
    Vector3 Position { get; set; }
}

public enum PlatformType
{
    None = 0,
    Custom = 1,
    Frog =2,
    FrogSlime = 3,
    Enemy = 4,
}

public class GamePlatform : IPlatform
{
    public PlatformType Type { get; }
    private readonly GameObject _gameObject;

    public Vector3 Position
    {
        get => _gameObject.transform.position;
        set => _gameObject.transform.position = value;
    }

    public GamePlatform(PlatformType type, GameObject gameObject)
    {
        Type = type;
        _gameObject = gameObject;
    }

    public void Reset()
    {
        //возвращение
    }
}

public interface IPlatformFactory
{
    IPlatform CreatePlatform(PlatformType platformType);
}


/// <summary>
/// Создает платформы по запросу
/// </summary>
public class PlatformFactory : IPlatformFactory
{
    private readonly PlatformsConfig _platformsConfig;

    public PlatformFactory(PlatformsConfig platformsConfig)
    {
        _platformsConfig = platformsConfig;
    }

    public IPlatform CreatePlatform(PlatformType platformType)
    {
        var prefabInfo = _platformsConfig.Platforms.FirstOrDefault(s => s.type == platformType);
        if (prefabInfo == null)
        {
            throw new ArgumentException($"Platform prefab not found for type {platformType}");
        }

        var platform = GameObject.Instantiate(prefabInfo.gameObject, Vector3.down * 100, Quaternion.identity);
        return new GamePlatform(platformType, platform);
    }
}