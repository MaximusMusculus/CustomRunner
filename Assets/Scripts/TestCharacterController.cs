using StateMachine;
using UnityEngine;
using VContainer.Unity;


public class CharacterController : ITickable, IFixedTickable
{
    private SimpleFsm _fsm;
    
    
    

    public void Tick()
    {
        _fsm.Tick();
    }
    public void FixedTick()
    {
        _fsm.FixedTick();
    }
}

public class TestCharacterController : IInitializable, ITickable, IFixedTickable
{
    private ICharacterContainer _character;
    private IInputJump _inputJump;
    private IInputAxis _inputAxis;

    private const float TestRunSpeed = 5;
    private float   _jumpSpeed = 6f;

    private readonly ICamera _camera;
    public TestCharacterController(ICamera camera, ICharacterContainer character, IInputJump inputJump, IInputAxis inputAxis)
    {
        _character = character;
        _inputJump = inputJump;
        _inputAxis = inputAxis;
        _camera = camera;
    }
    
    public void Initialize()
    {
        _camera.SetFollowTarget(_character.Rigidbody.transform);
    }

    public void Tick()
    {
        _character.Rigidbody.position += _inputAxis.GetAxis() * TestRunSpeed * Time.deltaTime;
        
        if (_jumpSpeed > 0)
        {
            _jumpSpeed += Physics2D.gravity.y * Time.deltaTime;
        }
        if (_inputJump.GetIsJump())
        {
            _jumpSpeed = 6f;
        }
    }

    public void FixedTick()
    {
        var moveDirection = Vector2.up * (_jumpSpeed * Time.fixedDeltaTime);
        var distance = moveDirection.magnitude;
        _character.Rigidbody.position += moveDirection.normalized * distance;
    }
}