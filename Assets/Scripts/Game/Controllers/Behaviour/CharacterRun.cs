using System;
using Core;
using Game.Animations;
using UnityEngine;
using VContainer.Unity;

namespace Game.Controllers.Behaviour
{
    public class CharacterRun : IState, ITickable, IFixedTickable
    {
        private readonly ICharacterContainer _character;
        private readonly IInput _inputAxis;
        private float _runSpeed;

        public CharacterRun(ICharacterContainer character, IInput inputAxis)
        {
            _character = character;
            _inputAxis = inputAxis;
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
            throw new NotImplementedException();
            //_runSpeed = _character.BaseValues.Get(CharacterProperty.RunSpeed);
        }

        public void FixedTick()
        {
            var speed = _inputAxis.GetAxis().x * _runSpeed * Time.fixedDeltaTime;
            _character.Rigidbody.position += new Vector2(speed, 0);
        }
    }
}