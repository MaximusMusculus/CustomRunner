using Core;
using Game.Animations;
using UnityEngine;
using VContainer.Unity;

namespace Game.Controllers.Behaviour
{
    public class CharacterFall : IState, ITickable, IFixedTickable
    {
        private readonly ICharacterContainer _character;
        private readonly IInput _inputAxis;
        private float _fallSpeed;
        private float _fallControlForce = 5;

        public CharacterFall(ICharacterContainer character, IInput inputAxis)
        {
            _character = character;
            _inputAxis = inputAxis;
        }

        public void Enter()
        {
            _fallSpeed = 0;
            _character.Animator.SetBool(AnimationConstants.IsGrounded, false);
        }

        public void Exit()
        {
        }

        public void Tick()
        {
            _fallSpeed += Physics2D.gravity.y * Time.deltaTime;
        }

        public void FixedTick()
        {
            var fallDirection = new Vector2(_inputAxis.GetAxis().x, 0) * Time.fixedDeltaTime * _fallControlForce;
            var moveDirection = Vector2.down * (_fallSpeed * Time.fixedDeltaTime) - fallDirection;
            var distance = moveDirection.magnitude;
            _character.Rigidbody.position -= moveDirection.normalized * distance;
        }
    }
}