using System.Collections.Generic;
using Cinemachine;
using StateMachine;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class LevelLifeScope : LifetimeScope
{
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    [SerializeField] private CharacterContainer _player;
    
    [SerializeField] private PlatformsConfig _platformsConfig;
    

    protected override void Configure(IContainerBuilder builder)
    {
        base.Configure(builder);
        
        builder.RegisterInstance(_virtualCamera).AsSelf();
        builder.RegisterInstance(_platformsConfig).AsSelf();
        
        builder.Register<SimpleCamera>(Lifetime.Singleton).As<ICamera>();
        builder.Register<InputJump>(Lifetime.Singleton).As<IInputJump, ITickable>();
        builder.Register<InputAxisFotTest>(Lifetime.Singleton).As<IInputAxis>();
        builder.Register<PlatformFactory>(Lifetime.Singleton).As<IPlatformFactory>();
        builder.Register<PlatformPool>(Lifetime.Singleton).As<IPlatformPool, IInitializable>();
        builder.Register<PlatformForPlayerSpawner>(Lifetime.Singleton).As<ITickable>();

        //builder.Register<InputRunnerAxis>(Lifetime.Singleton).AsImplementedInterfaces();

        builder.RegisterInstance<ICharacterContainer>(_player);
        builder.RegisterEntryPoint<TestCharacterController>();
    }
}


public class RunnerGame : IStartable, ITickable, IFixedTickable
{
    private IFsm _fsm;

    public RunnerGame()
    {
        _fsm = new SimpleFsm(); //get from
        
    }

    public void Start()
    {
        //create states fsm
        //config transitions
        //launch
    }

    public void Tick()
    {
        //update fsm
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


    public void FixedTick()
    {
        
    }
}