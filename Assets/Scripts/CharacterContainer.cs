using UnityEngine;
using VContainer.Unity;


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

public class TestCharacterController : ITickable
{
    private ICharacterContainer _character;
    private IInputJump _inputJump;
    private IInputAxis _inputAxis;

    private const float TestRunSpeed = 5;

    public TestCharacterController(ICharacterContainer character, IInputJump inputJump, IInputAxis inputAxis)
    {
        _character = character;
        _inputJump = inputJump;
        _inputAxis = inputAxis;
    }

    public void Tick()
    {
        _character.Rigidbody.position += _inputAxis.GetAxis() * TestRunSpeed * Time.deltaTime;
    }
}