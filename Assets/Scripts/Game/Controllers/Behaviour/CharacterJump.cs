using System;
using Core;
using Game.Animations;
using UnityEngine;
using VContainer.Unity;

namespace Game.Controllers.Behaviour
{
    public class CharacterJump : IState, ITickable, IFixedTickable
    {
        private readonly ICharacterContainer _character;
        private readonly IInput _inputAxis;
        private float _moveSpeed;
        private float _jumpSpeed;

        public CharacterJump(ICharacterContainer character, IInput inputAxis)
        {
            _character = character;
            _inputAxis = inputAxis;
        }

        public void Enter()
        {
            _jumpSpeed = 5;// _character.Values.Get(CharacterProperty.JumpSpeed);
            throw new NotImplementedException();
            
            _character.Animator.SetTrigger(AnimationConstants.OnJump);
            _character.Animator.SetBool(AnimationConstants.IsGrounded, false);
        }

        public void Exit()
        {
        }

        public void Tick()
        {
            _jumpSpeed += Physics2D.gravity.y * Time.deltaTime;
            _moveSpeed = 5; // _character.Values.Get(CharacterProperty.RunSpeed);
            throw new NotImplementedException();
        }

        public void FixedTick()
        {
            var jumpDirection = new Vector2(_inputAxis.GetAxis().x, 0) * Time.fixedDeltaTime * _moveSpeed;
            var moveDirection = Vector2.up * (_jumpSpeed * Time.fixedDeltaTime) + jumpDirection;
            var distance = moveDirection.magnitude;
            _character.Rigidbody.position += moveDirection.normalized * distance;
        }

        public bool IsJumpEnd()
        {
            return _jumpSpeed <= 0;
        }
    }
}