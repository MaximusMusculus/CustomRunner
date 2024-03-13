using System.Collections.Generic;
using Core;
using Game.Components;


namespace Game.Properties
{
    public interface IBehaviourComponent : IComponent
    {
        void SetDefaultBehaviour(IUpdate behaviour);
        void AddBehaviour(IUpdate behaviour, bool isOverriding);
        void Remove(IUpdate behaviour);
    }

    public class BehaviourComponent : IBehaviourComponent, IUpdate
    {
        private IUpdate _mainBehaviour = new EmptyBehaviour();
        private readonly List<IUpdate> _overridingBehaviours = new List<IUpdate>();
        private readonly List<IUpdate> _additionalBehaviours = new List<IUpdate>();

        public void SetDefaultBehaviour(IUpdate behaviour)
        {
            _mainBehaviour = behaviour;
        }

        public void AddBehaviour(IUpdate behaviour, bool isOverriding) //+mode? поведение не замещает, а дополняет
        {
            if (isOverriding)
            {
                _overridingBehaviours.Add(behaviour);
            }
            else
            {
                _additionalBehaviours.Add(behaviour);
            }
        }

        public void Remove(IUpdate behaviour)
        {
            if (_overridingBehaviours.Contains(behaviour))
            {
                _overridingBehaviours.Remove(behaviour);
            }

            if (_additionalBehaviours.Contains(behaviour))
            {
                _additionalBehaviours.Remove(behaviour);
            }
        }
        
        public void Update(float deltaTime)
        {
            if (_overridingBehaviours.Count < 1)
            {
                _mainBehaviour.Update(deltaTime);
            }
            else
            {
                foreach (var behaviour in _overridingBehaviours)
                {
                    behaviour.Update(deltaTime);
                }
            }
            
            foreach (var behaviour in _additionalBehaviours)
            {
                behaviour.Update(deltaTime);
            }
        }
        
    }
}