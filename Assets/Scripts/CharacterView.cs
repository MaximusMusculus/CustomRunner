using UnityEngine;

public class CharacterView : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private Animator _animator;
    public Rigidbody2D Rigidbody => _rigidbody;
    public Animator Animator => _animator;
}