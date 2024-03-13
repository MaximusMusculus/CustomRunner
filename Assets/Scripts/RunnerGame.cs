using System.Collections.Generic;
using System.Threading;
using Core;
using Cysharp.Threading.Tasks;
using Game.Components;
using Game.Properties;
using UnityEngine;
using VContainer.Unity;

//--PLayer делает своего персонажа
public class TestCharacter : IStartable, ITickable, IFixedTickable
{
    private readonly ICharacterFactory _characterFactory;
    private readonly CharacterConfig _characterConfig;
    private readonly IReadOnlyList<IFollowTarget> _followTarget;
    
    private ComponentsContainer _characterContainer;
    
    public TestCharacter(ICharacterFactory characterFactory, CharacterConfig characterConfig, IReadOnlyList<IFollowTarget> followTarget)
    {
        _characterFactory = characterFactory;
        _characterConfig = characterConfig;
        _followTarget = followTarget;
    }

    public void Start()
    {
        var character = _characterFactory.CreatePlayer(_characterConfig);
        character.TryGetComponent<IBehaviourComponent>(out var behaviourComponent);
        behaviourComponent.SetDefaultBehaviour(_characterFactory.CreatePlayerBehaviour(character));
        _characterContainer = character;

        if (!character.TryGetComponent<IMoveComponent>(out var moveComponent))
        {
            return;
        }
        
        foreach (var target in _followTarget)
        {
            target.SetFollowTarget(moveComponent.Transform);
        }
    }

    public void Tick()
    {
        _characterContainer.Update(Time.deltaTime);
    }

    public void FixedTick()
    {
        _characterContainer.FixedUpdate();
    }
}


/// <summary>
/// Собственно сама игра, управление окружением на сцене, счетом, персонажем и т.д.
/// </summary>
public class RunnerGame : IAsyncStartable, ITickable, IFixedTickable
{
    private const string Restart = "Restart";
    private const string Play = "Play";
    private const string Lose = "Lose";

    private readonly ICharacterFactory _characterFactory;
    private readonly CharacterConfig _characterConfig;
    private readonly GameContext _gameContext;
    private readonly SimpleStatesBehaviour _gameStatesBehaviour;

    public RunnerGame(IPlayerSpawnPoint spawnPoint, ICharacterFactory characterFactory, CharacterConfig characterConfig,
        IReadOnlyList<IFollowTarget> followTarget, IReadOnlyList<IResettable> resettables)
    {
        _characterFactory = characterFactory;
        _characterConfig = characterConfig;


        /*_gameContext = new GameContext();
        var stateStart = new BehaviourReStart(_gameContext, spawnPoint, resettables, followTarget);
        var statePlay = new BehaviourPlay(_gameContext);
        var stateLose = new BehaviourLose(_gameContext);

        _gameStatesBehaviour = new SimpleStatesBehaviour();
        _gameStatesBehaviour.AddState(Restart, stateStart, true);
        _gameStatesBehaviour.AddState(Play, statePlay);
        _gameStatesBehaviour.AddState(Lose, stateLose);
        
        _gameStatesBehaviour.AddTransition(Restart, Play, () => true);
        _gameStatesBehaviour.AddTransition(Play, Lose, IsPlayerDie);
        _gameStatesBehaviour.AddTransition(Lose, Restart, stateLose.IsStateFinish);*/
    }

    private bool IsPlayerDie()
    {
        //Тут надо сделать событие с шиной событий и добавить в общую систему. Но не сейчас. 
        return false;
        //_gameContext.CharacterBehaviour.CheckIsInState(CharacterFactory.StateDead);
    }


    public async UniTask StartAsync(CancellationToken cancellation)
    {
        var character = _characterFactory.CreatePlayer(_characterConfig);
        character.TryGetComponent<IBehaviourComponent>(out var behaviourComponent);
        behaviourComponent.SetDefaultBehaviour(_characterFactory.CreatePlayerBehaviour(character));

        _gameContext.CharacterContainer = character;
        _gameStatesBehaviour?.Activate();
    }

    public void Tick()
    {
        _gameStatesBehaviour?.Update(Time.deltaTime);
        _gameContext?.CharacterContainer?.Update(Time.deltaTime);
    }

    public void FixedTick()
    {
        _gameStatesBehaviour?.FixedUpdate();
        _gameContext?.CharacterContainer?.FixedUpdate();
    }


    public interface IGameContext
    {
        ComponentsContainer CharacterContainer { get; }
        SimpleStatesBehaviour CharacterBehaviour { get; }
    }

    public class GameContext : IGameContext
    {
        public ComponentsContainer CharacterContainer { get; set; }
        public SimpleStatesBehaviour CharacterBehaviour { get; set; }
    }


    public class BaseGameState
    {
        protected ComponentsContainer Character => Context.CharacterContainer;
        protected SimpleStatesBehaviour CharacterBehaviour => Context.CharacterBehaviour;
        protected IGameContext Context { get; private set; }

        protected BaseGameState(IGameContext context)
        {
            Context = context;
        }
    }

    public class BehaviourReStart : BaseGameState, IBehaviour
    {
        private readonly IPlayerSpawnPoint _playerSpawnPoint;
        private readonly IReadOnlyList<IResettable> _resettables;
        private readonly IReadOnlyList<IFollowTarget> _followTargets;

        public BehaviourReStart(IGameContext context, IPlayerSpawnPoint spawnPoint, IReadOnlyList<IResettable> resettables,
            IReadOnlyList<IFollowTarget> followTargets) : base(context)
        {
            _playerSpawnPoint = spawnPoint;
            _resettables = resettables;
            _followTargets = followTargets;
        }

        public void Activate()
        {
            Character.TryGetComponent<IMoveComponent>(out var moveComponent);
            
            moveComponent.Position = _playerSpawnPoint.GetPlayerSpawnPoint();
            CharacterBehaviour.Activate();
            foreach (var followTarget in _followTargets)
            {
                followTarget.SetFollowTarget(moveComponent.Transform);
            }

            foreach (var resettable in _resettables)
            {
                resettable.Reset();
            }
        }

        public void Deactivate()
        {
            CharacterBehaviour.Deactivate();
        }

        public void Update(float deltaTime)
        {
        }
    }

    public class BehaviourPlay : BaseGameState, IBehaviour, IUpdate, IFixedUpdate
    {
        public BehaviourPlay(IGameContext context) : base(context)
        {
        }

        public void Activate()
        {
        }

        public void Deactivate()
        {
        }

        public void Update(float deltaTime)
        {
            //Character.Update

            //персонаж бежит, платформы переставляются, токены спавнятся, эффекты применяются, очки начисляются
            CharacterBehaviour.Update(Time.deltaTime);
            Context.CharacterContainer.Update(Time.deltaTime);
        }

        public void FixedUpdate()
        {
            CharacterBehaviour.FixedUpdate();
            Context.CharacterContainer.FixedUpdate();
        }
    }

    public class BehaviourLose : BaseGameState, IBehaviour
    {
        //персонаж умер
        //грустную музыку
        //показ окна с текущим счетом (чего либо)
        //кнопка рестарт -> StateReStart
        //кнопка выход -> StateMenu
        private float _waitTimer;

        public BehaviourLose(IGameContext context) : base(context)
        {
        }

        public void Activate()
        {
            _waitTimer = 1;
        }

        public void Deactivate()
        {
        }

        public void Update(float deltaTime)
        {
            _waitTimer -= Time.deltaTime;
        }

        public bool IsStateFinish()
        {
            return _waitTimer <= 0;
        }
    }
}