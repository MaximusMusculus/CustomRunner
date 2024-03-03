using UnityEngine;

namespace Game.Animations
{
    public static class AnimationConstants
    {
        public static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
        public static readonly int IsDead = Animator.StringToHash("IsDead");
        public static readonly int OnJump = Animator.StringToHash("OnJump");
    }
}