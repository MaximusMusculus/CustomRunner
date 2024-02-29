using System.Collections.Generic;
using Custom;
using VContainer.Unity;

public interface IPlatformPool
{
    IPlatform GetPlatform(PlatformType platformType);
    void ReleasePlatform(IPlatform platform);
}

public class PlatformPool : IInitializable, IPlatformPool
{
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
        _pools[platform.Type].Release(platform);
    }
}