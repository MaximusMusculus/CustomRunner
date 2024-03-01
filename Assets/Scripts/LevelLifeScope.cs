using Cinemachine;
using Custom;
using UnityEngine;
using VContainer;
using VContainer.Unity;


/// <summary>
/// Корень композиции для уровня
/// Для расширения - убрать AutoRun и активировать контекс вручную попутно устанавливая настроечные конфиги. 
/// </summary>
public class LevelLifeScope : LifetimeScope
{
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    [SerializeField] private LayersConfig layersConfig;
    
    //Конфиги можно брать откуда угодно. Для теста - указываю прям тут
    [SerializeField] private CharacterConfig _playerCharacterConfig;
    [SerializeField] private PlatformsConfig _platformsConfig;
    [SerializeField] private Transform _spawnPoint;
    
    protected override void Configure(IContainerBuilder builder)
    {
        base.Configure(builder);
        
        builder.RegisterInstance(layersConfig);
        builder.RegisterInstance(_virtualCamera);
        builder.RegisterInstance(_platformsConfig);
        builder.RegisterInstance(_playerCharacterConfig);
        builder.RegisterInstance(new SimplePlayerSpawnPoint(_spawnPoint)).As<IPlayerSpawnPoint>();
        
        builder.Register<SimpleCamera>(Lifetime.Singleton).As<IFollowTarget>();
        builder.Register<InputJump>(Lifetime.Singleton).As<IInputJump, ITickable>();
        builder.Register<InputRunnerAxis>(Lifetime.Singleton).As<IInputAxis>(); //InputAxisFotTest
        builder.Register<PlatformFactory>(Lifetime.Singleton).As<IPlatformFactory>();
        builder.Register<CharacterFactory>(Lifetime.Singleton).As<ICharacterFactory>();
        builder.Register<PlatformPool>(Lifetime.Singleton).As<IPlatformPool, IInitializable>();
        builder.Register<PlatformForPlayerSpawner>(Lifetime.Singleton).As<ITickable, IFollowTarget, IResettable>();
        
        builder.RegisterEntryPoint<RunnerGame>();
    }
}