using Core;
using Game.Animations;
using Game.Properties;
using Game.Shared;
using UnityEngine;

namespace Game.Components
{
    public interface IAnimationComponent : IComponent
    {
    }
    
    public class AnimationComponent : IAnimationComponent, IUpdate
    {
        private readonly Animator _animator;
        private readonly IMoveComponent _moveComponent;
        private readonly IPropertyComponent _propertyComponent;
        
        private bool IsGrounded => _moveComponent.IsOnGround;
        private Vector2 Velocity => _moveComponent.Velocity;
        
        
        public AnimationComponent(Animator animator, IMoveComponent moveComponent, IPropertyComponent property)
        {
            _animator = animator;
            _moveComponent = moveComponent;
            _propertyComponent = property;
        }
        
        public void Update(float deltaTime)
        {
            var jumpForce = _propertyComponent.GetValue(TypeProperty.JumpForce);
            if (Velocity.y >= jumpForce)
            {
                _animator.SetTrigger(AnimationConstants.OnJump);
            }
            _animator.SetBool(AnimationConstants.IsGrounded, IsGrounded);
            
        }
    }
}