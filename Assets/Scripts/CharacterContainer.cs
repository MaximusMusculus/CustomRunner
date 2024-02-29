using UnityEngine;


public interface ICharacterContainer
{
    Rigidbody2D Rigidbody { get; }
    Animator Animator { get; }
}

public class CharacterContainer : MonoBehaviour, ICharacterContainer
{
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private Animator _animator;

    public Rigidbody2D Rigidbody => _rigidbody;
    public Animator Animator => _animator;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
    }
}
