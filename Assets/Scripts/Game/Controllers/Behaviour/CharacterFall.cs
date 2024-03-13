using Core;
using Game.Components;
using Game.Properties;
using Game.Shared;
using UnityEngine;

namespace Game.Controllers.Behaviour
{
    public sealed class CharacterFall : IBehaviour
    {
        private readonly IMoveComponent _moveComponent;
        private readonly IPropertyComponent _propertyComponent;
        private readonly IInput _input;

        private float _fallSpeed;
        private float _moveSpeed;

        public CharacterFall( IInput input, IMoveComponent moveComponent, IPropertyComponent propertyComponent)
        {
            _input = input;
            _moveComponent = moveComponent;
            _propertyComponent = propertyComponent;
        }

        public void Activate()
        {
            _fallSpeed = 0;
            _moveSpeed = _propertyComponent.GetValue(TypeProperty.Speed);
        }

        public void Deactivate()
        {
        }

        public void Update(float deltaTime)
        {
            _fallSpeed += Physics2D.gravity.y * Time.deltaTime;
            var move = new Vector2(_input.GetAxis().x, 0) * _moveSpeed;
            var fall = Vector2.up  * _fallSpeed;
            var moveDirection = move + fall;
            var distance = moveDirection.magnitude;
            _moveComponent.Move(moveDirection.normalized * distance);
        }

  
    }
}