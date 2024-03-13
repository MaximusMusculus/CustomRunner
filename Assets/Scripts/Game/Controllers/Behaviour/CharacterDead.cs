using Core;
using Game.Animations;
using UnityEngine;

namespace Game.Controllers.Behaviour
{
    public class CharacterDead : IBehaviour
    {
        private readonly Animator _animator;

        public CharacterDead(Animator animator)
        {
            _animator = animator;
        }

        public void Activate()
        {
            _animator.SetBool(AnimationConstants.IsDead, true);
        }

        public void Deactivate()
        {
            _animator.SetBool(AnimationConstants.IsDead, false);
        }

        public void Update(float deltaTime)
        {
            
        }
    }
}