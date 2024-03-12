using Core;
using Game.Animations;
using Game.Components;
using UnityEngine;
using VContainer.Unity;

namespace Game.Controllers.Behaviour
{
    public class CharacterFall : IState, ITickable
    {
        private readonly ICharacterContainer _character;
        private readonly IMoveComponent _moveComponent;
        private readonly IInput _inputAxis;
        
        private float _fallSpeed;
        private float _moveSpeed;

        public CharacterFall(ICharacterContainer character, IInput inputAxis)
        {
            _character = character;
            _inputAxis = inputAxis;
            if (!character.TryGetComponent(out _moveComponent))
            {
                throw new System.Exception("CharacterFall: character does not have move component");
            }
        }

        public void Enter()
        {
            _fallSpeed = 0;
            _moveSpeed = 5;
            _character.Animator.SetBool(AnimationConstants.IsGrounded, false);
        }

        public void Exit()
        {
        }

        public void Tick()
        {
            _fallSpeed += Physics2D.gravity.y * Time.deltaTime;
            
            var move = new Vector2(_inputAxis.GetAxis().x, 0) * _moveSpeed;
            var fall = Vector2.up  * _fallSpeed;
            var moveDirection = move + fall;
            var distance = moveDirection.magnitude;
            _moveComponent.Move(moveDirection.normalized * distance);
        }
    }
}