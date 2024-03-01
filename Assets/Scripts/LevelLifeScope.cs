using System.Collections.Generic;
using System.Threading;
using Cinemachine;
using Cysharp.Threading.Tasks;
using StateMachine;
using UnityEngine;
using VContainer;
using VContainer.Unity;


[System.Serializable]
public class LayersConfig
{
    public LayerMask ground;
    public LayerMask enemy;

}
public class LevelLifeScope : LifetimeScope
{
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    [SerializeField] private CharacterConfig _playerCharacterConfig;
    [SerializeField] private PlatformsConfig _platformsConfig;
    [SerializeField] private LayersConfig layersConfig;
    
    protected override void Configure(IContainerBuilder builder)
    {
        base.Configure(builder);

        builder.RegisterInstance(layersConfig);
        builder.RegisterInstance(_virtualCamera);
        builder.RegisterInstance(_platformsConfig);
        builder.RegisterInstance(_playerCharacterConfig);
        
        builder.Register<SimpleCamera>(Lifetime.Singleton).As<IFollowTarget>();
        builder.Register<InputJump>(Lifetime.Singleton).As<IInputJump, ITickable>();
        builder.Register<InputAxisFotTest>(Lifetime.Singleton).As<IInputAxis>(); //InputRunnerAxis
        builder.Register<PlatformFactory>(Lifetime.Singleton).As<IPlatformFactory>();
        builder.Register<CharacterFactory>(Lifetime.Singleton).As<ICharacterFactory>();
        builder.Register<PlatformPool>(Lifetime.Singleton).As<IPlatformPool, IInitializable>();
        builder.Register<PlatformForPlayerSpawner>(Lifetime.Singleton).As<ITickable, IFollowTarget>();

        builder.RegisterEntryPoint<RunnerGame>();
    }
}


public class RunnerGame : IAsyncStartable, ITickable, IFixedTickable
{
    private readonly ICharacterFactory _characterFactory;
    private readonly CharacterConfig _characterConfig;

    private SimpleFsm _characterUpdate;
    private readonly IReadOnlyList<IFollowTarget> _followTarget; //??

    public RunnerGame(ICharacterFactory characterFactory, CharacterConfig characterConfig, IReadOnlyList<IFollowTarget> followTarget)
    {
        _characterFactory = characterFactory;
        _characterConfig = characterConfig;
        _followTarget = followTarget;
    }
    
    public async UniTask StartAsync(CancellationToken cancellation)
    {
        var playerCharacter = await _characterFactory.CreatePlayer(_characterConfig);
        _characterUpdate = _characterFactory.CreatePlayerBehaviour(playerCharacter);

        foreach (var target in _followTarget)
        {
            target.SetFollowTarget(playerCharacter.Rigidbody.transform);
        }
        
        //create states fsm
        //config transitions
        //launch
    }

    public void Tick()
    {
        //update fsm
        _characterUpdate?.Tick();
    }

    
    public void FixedTick()
    {
        _characterUpdate?.FixedTick();
    }

    public class GameContext 
    {
        
    }
    
    
    public class StateReStart : IState
    {
        public void Enter()
        {
            Debug.Log("StateReStart enter");
            //персонажа на место
            //счет обнулить
            //эффекты снять
            //камеру на персонажа
            //платформы на место
        }

        public void Exit()
        {
            Debug.Log("StateReStart exit");
        }
    }

    public class StatePlay
    {
        //персонаж бежит, платформы переставляются, токены спавнятся 
        //ждем события о смерти персонажа
    }

    public class StateDie
    {
        //персонаж умер
        //грустную музыку
        //показ окна с текущим счетом (чего либо)
        //кнопка рестарт -> StateReStart
    }



}