using System;
using System.Collections.Generic;
using VContainer.Unity;


namespace StateMachine
{
    public interface IState
    {
        void Enter();
        void Exit();
    }
    public interface ICondition
    {
        bool Check();
    }
    public interface IFsm
    {
        void RegisterState(string name, IState state);
        void RegisterTransition(string from, string to, ICondition condition);
        void LaunchState(string name);
    }
    
    /// <summary>
    /// Написано в спешке, проверок на корректность ввода - почти нет.
    /// тестов тоже нет.
    /// </summary>
    public class SimpleFsm : IFsm, ITickable, IFixedTickable
    {
        private readonly Dictionary<string, IState> _states = new Dictionary<string, IState>();
        private readonly Dictionary<IState, List<ICondition>> _fromState = new Dictionary<IState, List<ICondition>>();
        private readonly Dictionary<ICondition, IState> _toState = new Dictionary<ICondition, IState>();

        private IState _currentState;
        private readonly EmptyState _emptyState;

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

        public void RegisterState(string name, IState state)
        {
            _states.Add(name, state);
        }

        public void RegisterTransition(string from, string to, ICondition condition)
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
                _fromState.Add(fromSate, new List<ICondition>());
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

            ChangeState(_states[name]);
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
                    if (!condition.Check())
                    {
                        continue;
                    }

                    var nextState = _toState[condition];
                    ChangeState(nextState);
                }
            }
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