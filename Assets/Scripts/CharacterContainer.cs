using UnityEngine;


public interface ICharacterContainer
{
    Rigidbody2D Rigidbody { get; }
    Animator Animator { get; }
}


public class CharacterContainer : ICharacterContainer
{
    public Rigidbody2D Rigidbody { get; }
    public Animator Animator { get; }

    public CharacterContainer(Rigidbody2D rigidbody, Animator animator)
    {
        Rigidbody = rigidbody;
        Animator = animator;
    }
}