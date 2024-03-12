using Core;
using Game.Animations;
using Game.Components;
using UnityEngine;
using VContainer.Unity;

namespace Game.Controllers.Behaviour
{
    public class CharacterJump : IState, ITickable
    {
        private readonly ICharacterContainer _character;
        private readonly IInput _inputAxis;
        private float _moveSpeed;
        private float _jumpSpeed;

        private IMoveComponent _moveComponent;

        public CharacterJump(ICharacterContainer character, IInput inputAxis)
        {
            _character = character;
            _inputAxis = inputAxis;
            if (!character.TryGetComponent(out _moveComponent))
            {
                throw new System.Exception("CharacterJump: character does not have move component");
            }
        }

        public void Enter()
        {
            _jumpSpeed = 7;// _character.Values.Get(CharacterProperty.JumpSpeed);
            //throw new NotImplementedException();
            
            _character.Animator.SetTrigger(AnimationConstants.OnJump);
            _character.Animator.SetBool(AnimationConstants.IsGrounded, false);
        }

        public void Exit()
        {
        }

        public void Tick()
        {
            _moveSpeed = 5; // _character.Values.Get(CharacterProperty.RunSpeed);
            _jumpSpeed += Physics2D.gravity.y * Time.deltaTime;

            var moveSpeed = new Vector2(_inputAxis.GetAxis().x, 0) * _moveSpeed;
            var jumpSpeed = Vector2.up * _jumpSpeed;
            var moveDirection = moveSpeed + jumpSpeed;
            
            var distance = moveDirection.magnitude;
            _moveComponent.Move(moveDirection.normalized * distance);
        }



        public bool IsJumpEnd()
        {
            return _jumpSpeed <= 0;
        }
    }
}