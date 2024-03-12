using System.Collections.Generic;
using System.Threading;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer.Unity;


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
    private readonly SimpleFsm _gameFsm;
    
    public RunnerGame(IPlayerSpawnPoint spawnPoint, ICharacterFactory characterFactory, CharacterConfig characterConfig, IReadOnlyList<IFollowTarget> followTarget, IReadOnlyList<IResettable> resettables)
    {
        _characterFactory = characterFactory;
        _characterConfig = characterConfig;
        
        
        _gameContext = new GameContext();
        var stateStart = new StateReStart(_gameContext, spawnPoint, resettables, followTarget);
        var statePlay = new StatePlay(_gameContext);
        var stateLose = new StateLose(_gameContext);

        _gameFsm = new SimpleFsm();
        _gameFsm.AddState(Restart, stateStart);
        _gameFsm.AddState(Play, statePlay);
        _gameFsm.AddState(Lose, stateLose);
        
        _gameFsm.AddTransition(Restart, Play, () => true);
        _gameFsm.AddTransition(Play, Lose, IsPlayerDie);
        _gameFsm.AddTransition(Lose, Restart, stateLose.IsStateFinish);
    }

    private bool IsPlayerDie()
    {
        //Тут надо сделать событие с шиной событий и добавить в общую систему. Но не сейчас. 
        return _gameContext.CharacterBehaviour.CheckIsInState(CharacterFactory.StateDead);
    }


    public async UniTask StartAsync(CancellationToken cancellation)
    {
        _gameContext.CharacterContainer = await _characterFactory.CreatePlayer(_characterConfig);
        _gameContext.CharacterBehaviour = _characterFactory.CreatePlayerBehaviour(_gameContext.CharacterContainer);
        _gameFsm.LaunchState(Restart);
    }

    public void Tick()
    {
        _gameFsm.Tick();
        _gameContext.CharacterContainer.Tick();
    }

    public void FixedTick()
    {
        _gameContext?.CharacterContainer?.FixedTick();
        _gameFsm.FixedTick();
    }


    public interface IGameContext
    {
        ICharacterContainer CharacterContainer { get; }
        SimpleFsm CharacterBehaviour { get; }
    }
    
    public class GameContext : IGameContext
    {
        public ICharacterContainer CharacterContainer { get;  set; }
        public SimpleFsm CharacterBehaviour { get; set; }
    }
    

    public class BaseGameState 
    {
        protected ICharacterContainer Character => Context.CharacterContainer;
        protected SimpleFsm CharacterBehaviour => Context.CharacterBehaviour;
        protected IGameContext Context { get; private set; }

        protected BaseGameState(IGameContext context)
        {
            Context = context;
        }
    }

    public class StateReStart : BaseGameState, IState
    {
        private readonly IPlayerSpawnPoint _playerSpawnPoint;
        private readonly IReadOnlyList<IResettable> _resettables;
        private readonly IReadOnlyList<IFollowTarget> _followTargets;

        public StateReStart(IGameContext context, IPlayerSpawnPoint spawnPoint, IReadOnlyList<IResettable> resettables, IReadOnlyList<IFollowTarget> followTargets) : base(context)
        {
            _playerSpawnPoint = spawnPoint;
            _resettables = resettables;
            _followTargets = followTargets;
        }
        
        public void Enter()
        {
            Character.MoveComponent.Position = _playerSpawnPoint.GetPlayerSpawnPoint();
            CharacterBehaviour.Reset();

            foreach (var followTarget in _followTargets)
            {
                followTarget.SetFollowTarget(Character.MoveComponent.Transform);
            }
            
            foreach (var resettable in _resettables)
            {
                resettable.Reset();
            }
        }

        public void Exit()
        {
        }
    }

    public class StatePlay : BaseGameState, IState, ITickable, IFixedTickable
    {
        public StatePlay(IGameContext context) : base(context)
        {
        }
        
        public void Enter() { }
        public void Exit() { }

        public void Tick()
        {
            //персонаж бежит, платформы переставляются, токены спавнятся, эффекты применяются, очки начисляются
            CharacterBehaviour.Tick();
        }
        public void FixedTick()
        {
            CharacterBehaviour.FixedTick();
        }

    }

    public class StateLose : BaseGameState, IState, ITickable
    {
        //персонаж умер
        //грустную музыку
        //показ окна с текущим счетом (чего либо)
        //кнопка рестарт -> StateReStart
        //кнопка выход -> StateMenu
        private float _waitTimer;
        
        public StateLose(IGameContext context) : base(context)
        {
        }

        public void Enter()
        {
            _waitTimer = 1;
        }

        public void Exit()
        {
        }
        
        public void Tick()
        {
            _waitTimer -= Time.deltaTime;
        }
        
        public bool IsStateFinish()
        {
            return _waitTimer <= 0;
        }
    }



}