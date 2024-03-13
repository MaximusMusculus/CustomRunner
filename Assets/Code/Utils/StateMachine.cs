using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public interface IBehaviour : IUpdate
    {
        void Activate();
        void Deactivate();
    }
    
    public class EmptyBehaviour : IBehaviour, IFixedUpdate
    {
        public void Activate() { }
        public void Deactivate() { }
        public void Update(float deltaTime) { }
        public void FixedUpdate() { }
    }


    public interface IStatesBehaviour : IBehaviour
    {
        void AddState(string name, IBehaviour behaviour, bool isLaunchState = false);
        void AddTransition(string from, string to, Condition condition);
    }
    
    public delegate bool Condition();   
        
    /// <summary>
    /// Написано в спешке, проверок на корректность ввода - почти нет.
    /// тестов тоже нет.
    /// FSM это дерево поведений с условиями переходов
    /// </summary>
    public class SimpleStatesBehaviour : IStatesBehaviour, IFixedUpdate, IResettable
    {
        private readonly Dictionary<string, IBehaviour> _states = new Dictionary<string, IBehaviour>();
        private readonly Dictionary<IBehaviour, List<Condition>> _fromState = new Dictionary<IBehaviour, List<Condition>>();
        private readonly Dictionary<Condition, IBehaviour> _toState = new Dictionary<Condition, IBehaviour>();

        private readonly EmptyBehaviour _emptyBehaviour;
        private IBehaviour _currentBehaviour;
        private IBehaviour _launchBehaviour;
        private IFixedUpdate _currentFixedUpdatable;
        private bool _isRunning;
        
        
        public SimpleStatesBehaviour()
        {
            _emptyBehaviour = new EmptyBehaviour();
            _currentBehaviour = _emptyBehaviour;
            _currentFixedUpdatable = _emptyBehaviour;
        }

        public void AddState(string name, IBehaviour behaviour, bool isLaunchState = false)
        {
            if (isLaunchState && _launchBehaviour != null)
            {
                throw new ArgumentException("Launch state already registered");
            }

            if (isLaunchState)
            {
                _launchBehaviour = behaviour;
            }
            _states.Add(name, behaviour);
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


        public void Activate()
        {
            if (_currentBehaviour != _emptyBehaviour)
            {
                throw new ArgumentException("Fsm is launched");
            }

            if (_launchBehaviour == null)
            {
                throw new ArgumentException("Launch state not registered");
            }
            ChangeState(_launchBehaviour);
            Debug.Log("Fsm is Activate");
        }

        public void Deactivate()
        {
            Debug.Log("Fsm is deactivated");
            ChangeState(_emptyBehaviour);
        }
        
        private void ChangeState(IBehaviour nextBehaviour)
        {
            _currentBehaviour.Deactivate();
            _currentBehaviour = nextBehaviour;
            _currentFixedUpdatable = _currentBehaviour as IFixedUpdate ?? _emptyBehaviour;
            _currentBehaviour.Activate();
        }
        
        private void UpdateTransition()
        {
            if (_fromState.TryGetValue(_currentBehaviour, out var conditions))
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

        public void Reset()
        {
            ChangeState(_launchBehaviour);
        }

        public void Update(float deltaTime)
        {
            if (_currentBehaviour == _emptyBehaviour)
            {
                return;
            }
            UpdateTransition();
            _currentBehaviour.Update(deltaTime);
        }
        public void FixedUpdate()
        {
            if(_currentFixedUpdatable == _emptyBehaviour)
            {
                return;
            }
            _currentFixedUpdatable.FixedUpdate();
        }
        
    }
}