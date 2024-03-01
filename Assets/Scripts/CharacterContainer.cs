using UnityEngine;


public interface ICharacterContainer
{
    Rigidbody2D Rigidbody { get; }
    Animator Animator { get; }
    
    IProperties BaseValues { get; }
    IProperties Values { get; }
}


public class CharacterContainer : ICharacterContainer
{
    public Rigidbody2D Rigidbody { get; }
    public Animator Animator { get; }
    public IProperties BaseValues { get; }
    public IProperties Values { get; }

    public CharacterContainer(Rigidbody2D rigidbody, Animator animator, IProperties baseValues, IProperties modifyValues)
    {
        Rigidbody = rigidbody;
        Animator = animator;
        BaseValues = baseValues;
        Values = modifyValues;
    }
}