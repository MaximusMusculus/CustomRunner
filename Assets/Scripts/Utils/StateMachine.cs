using System;
using System.Collections.Generic;
using Custom;
using VContainer.Unity;


namespace StateMachine
{
    public interface IState
    {
        void Enter();
        void Exit();
    }
    
    
    public interface IFsm
    {
        void AddState(string name, IState state);
        void AddTransition(string from, string to, Condition condition);
        void LaunchState(string name);
    }
    public delegate bool Condition();  
        
    /// <summary>
    /// Написано в спешке, проверок на корректность ввода - почти нет.
    /// тестов тоже нет.
    /// </summary>
    public class SimpleFsm : IFsm, ITickable, IFixedTickable, IResettable
    {
        private readonly Dictionary<string, IState> _states = new Dictionary<string, IState>();
        private readonly Dictionary<IState, List<Condition>> _fromState = new Dictionary<IState, List<Condition>>();
        private readonly Dictionary<Condition, IState> _toState = new Dictionary<Condition, IState>();

        private IState _currentState;
        private readonly EmptyState _emptyState;
        private IState _launchState;

        private ITickable _currentTickable;
        private IFixedTickable _currentFixedTickable;
        
        private class EmptyState : IState, ITickable, IFixedTickable
        {
            public void Enter() { }
            public void Exit() { }
            public void Tick() { }
            public void FixedTick() { }
        }
        
        public SimpleFsm()
        {
            _emptyState = new EmptyState();
            _currentState = _emptyState;
            _currentTickable = _emptyState;
            _currentFixedTickable = _emptyState;
        }

        public void AddState(string name, IState state)
        {
            _states.Add(name, state);
        }

        public void AddTransition(string from, string to, Condition condition)
        {
            if (!_states.TryGetValue(from, out var fromSate))
            {
                throw new ArgumentException($"State {from} not registered");
            }

            if (!_states.TryGetValue(to, out var toState))
            {
                throw new ArgumentException($"State {to} not registered");
            }

            if (_toState.ContainsKey(condition))
            {
                throw new ArgumentException($"Condition {from}->{to} already registered");
            }

            if (!_fromState.ContainsKey(fromSate))
            {
                _fromState.Add(fromSate, new List<Condition>());
            }

            _fromState[fromSate].Add(condition);
            _toState.Add(condition, toState);
        }

        public void LaunchState(string name)
        {
            if (_currentState != _emptyState)
            {
                throw new ArgumentException("Fsm is launched");
            }

            _launchState = _states[name];
            ChangeState(_launchState);
        }

        private void ChangeState(IState nextState)
        {
            _currentState.Exit();
            _currentState = nextState;
            _currentTickable = _currentState as ITickable ?? _emptyState;
            _currentFixedTickable = _currentState as IFixedTickable ?? _emptyState;
            _currentState.Enter();
        }
        
        private void UpdateTransition()
        {
            if (_fromState.TryGetValue(_currentState, out var conditions))
            {
                foreach (var condition in conditions)
                {
                    if (!condition())
                    {
                        continue;
                    }
                    var nextState = _toState[condition];
                    ChangeState(nextState);
                }
            }
        }

        //хак
        public bool CheckIsInState(string state)
        {
            return _currentState == _states[state];
        }

        public void Reset()
        {
            ChangeState(_launchState);
        }

        public void Tick()
        {
            UpdateTransition();
            _currentTickable.Tick();
        }

        public void FixedTick()
        {
            _currentFixedTickable.FixedTick();
        }

    
    }
}