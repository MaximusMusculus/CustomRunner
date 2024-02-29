using System.Collections.Generic;
using Custom;
using UnityEngine;
using VContainer.Unity;

public interface IPlatformPool
{
    IPlatform GetPlatform(PlatformType platformType);
    void ReleasePlatform(IPlatform platform);
}

public class PlatformPool : IInitializable, IPlatformPool
{
    private Vector3 _platformHidePosition = new Vector3(-1000, -1000, -1000);
    private readonly IPlatformFactory _platformFactory;
    private readonly Dictionary<PlatformType, IPool<IPlatform>> _pools;

    public PlatformPool(IPlatformFactory platformFactory)
    {
        _platformFactory = platformFactory;
        _pools = new Dictionary<PlatformType, IPool<IPlatform>>();
    }

    public void Initialize()
    {
        _pools[PlatformType.Custom] = new StackPool<IPlatform>(() => _platformFactory.CreatePlatform(PlatformType.Custom));
    }

    public IPlatform GetPlatform(PlatformType platformType)
    {
        return _pools[platformType].Get();
    }

    public void ReleasePlatform(IPlatform platform)
    {
        platform.Position = _platformHidePosition;
        _pools[platform.Type].Release(platform);
    }
}