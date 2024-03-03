using Core;
using Game.Animations;

namespace Game.Controllers.Behaviour
{
    public class CharacterDead : IState
    {
        private readonly ICharacterContainer _character;

        public CharacterDead(ICharacterContainer character)
        {
            _character = character;
        }

        public void Enter()
        {
            _character.Animator.SetBool(AnimationConstants.IsDead, true);
        }

        public void Exit()
        {
            _character.Animator.SetBool(AnimationConstants.IsDead, false);
        }
    }
}