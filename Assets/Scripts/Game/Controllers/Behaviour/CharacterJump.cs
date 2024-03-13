using Core;
using Game.Components;
using Game.Properties;
using Game.Shared;
using UnityEngine;

namespace Game.Controllers.Behaviour
{
    public sealed class CharacterJump : IBehaviour
    {
        private readonly IMoveComponent _move;
        private readonly IPropertyComponent _property;
        private readonly IInput _input;
        
        private float _moveSpeed;
        private float _jumpSpeed;

        public CharacterJump(IMoveComponent move, IPropertyComponent property, IInput input)
        {
            _property = property;
            _input = input;
            _move = move;
        }

        public void Activate()
        {
            _jumpSpeed = _property.GetValue(TypeProperty.JumpForce);
            //_character.Animator.SetTrigger(AnimationConstants.OnJump);
            //_character.Animator.SetBool(AnimationConstants.IsGrounded, false);
        }
        public void Deactivate()
        {
        }

        public void Update(float deltaTime)
        {
            _moveSpeed = _property.GetValue(TypeProperty.Speed);
            var moveSpeed = new Vector2(_input.GetAxis().x, 0) * _moveSpeed;
            var jumpSpeed = Vector2.up * _jumpSpeed;
            var moveDirection = moveSpeed + jumpSpeed;
            var distance = moveDirection.magnitude;
            _move.Move(moveDirection.normalized * distance);
            
            _jumpSpeed += Physics2D.gravity.y * Time.deltaTime;
        }
        
        public bool IsJumpEnd()
        {
            return _jumpSpeed <= 0;
        }
    }
}