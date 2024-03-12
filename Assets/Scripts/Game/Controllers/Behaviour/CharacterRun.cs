using System;
using Core;
using Game.Animations;
using Game.Components;
using UnityEngine;
using VContainer.Unity;

namespace Game.Controllers.Behaviour
{
    public class CharacterRun : IState, ITickable
    {
        private readonly ICharacterContainer _character;
        private readonly IInput _inputAxis;
        private float _runSpeed;
        private IMoveComponent _moveComponent;

        public CharacterRun(ICharacterContainer character, IInput inputAxis)
        {
            _character = character;
            _inputAxis = inputAxis;
            if (!character.TryGetComponent(out _moveComponent))
            {
                throw new Exception("CharacterRun: character does not have move component");
            }
        }

        public void Enter()
        {
            _character.Animator.SetBool(AnimationConstants.IsGrounded, true);
            //Animator.SetFloat(AnimationConstants.VelocityXAnimatorId, Mathf.Abs(_runningSpeed) / BaseSpeed);
        }
        public void Exit()
        {
            
        }

        public void Tick()
        {
            _runSpeed = 5; 
                //_character.BaseValues.Get(CharacterProperty.RunSpeed);
            var moveX = _inputAxis.GetAxis().x * _runSpeed;
            _moveComponent.Move(new Vector2(moveX, 0));
        }

        public void FixedTick()
        {
            //var speed = _inputAxis.GetAxis().x * _runSpeed * Time.fixedDeltaTime;
            //_character.Rigidbody.position += new Vector2(speed, 0);
        }
    }
}