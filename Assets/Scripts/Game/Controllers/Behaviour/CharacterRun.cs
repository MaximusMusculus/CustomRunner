using System;
using Core;
using Game.Components;
using Game.Properties;
using Game.Shared;
using UnityEngine;

namespace Game.Controllers.Behaviour
{
    //стратегия линейного движения персонажа 
    public sealed class CharacterRun : IBehaviour
    {
        private readonly IMoveComponent _moveComponent;
        private readonly IPropertyComponent _propertyComponent;
        
        private readonly IInput _input;
        private float _runSpeed;

        public CharacterRun( IMoveComponent moveComponent, IPropertyComponent propertyComponent, IInput input)
        {
            _input = input;

            _moveComponent = moveComponent;
            _propertyComponent = propertyComponent;
            if (!propertyComponent.Has(TypeProperty.Speed))
            {
                throw new ArgumentException("CharacterRun: character does not have speed property");
            }
        }

        public void Activate()
        {
            //_animator.SetBool(AnimationConstants.IsGrounded, true);
            //Animator.SetFloat(AnimationConstants.VelocityXAnimatorId, Mathf.Abs(_runningSpeed) / BaseSpeed);
        }
        public void Deactivate() { }

        
        public void Update(float deltaTime)
        {
            _runSpeed = _propertyComponent.GetValue(TypeProperty.Speed);
            var moveX = _input.GetAxis().x * _runSpeed;
            _moveComponent.Move(new Vector2(moveX, 0));
        }
    }
}