using Cinemachine;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class LevelLifeScope : LifetimeScope
{
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    [SerializeField] private CharacterContainer _player;
    
    
    protected override void Configure(IContainerBuilder builder)
    {
        base.Configure(builder);
        builder.RegisterInstance(_virtualCamera).AsSelf();
        builder.Register<SimpleCamera>(Lifetime.Singleton).AsImplementedInterfaces();
        builder.Register<InputJump>(Lifetime.Singleton).AsImplementedInterfaces();
        builder.Register<InputAxisFotTest>(Lifetime.Singleton).AsImplementedInterfaces();

        builder.RegisterInstance<ICharacterContainer>(_player);
        builder.RegisterEntryPoint<TestCharacterController>();
    }
}