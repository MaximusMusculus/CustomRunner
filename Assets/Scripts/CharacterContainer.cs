using System;
using UnityEngine;


public interface ICharacterContainer
{
    Rigidbody2D Rigidbody { get; }
    Animator Animator { get; }
    Vector3 GroundPoint { get; }
}

public class AnimationConstants
{
    public static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
    public static readonly int OnJump = Animator.StringToHash("OnJump");
    
}
public class CharacterContainer : MonoBehaviour, ICharacterContainer
{
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _groundPoint;

    public Rigidbody2D Rigidbody => _rigidbody;
    public Animator Animator => _animator;
    public Vector3 GroundPoint => _groundPoint.position;

}
