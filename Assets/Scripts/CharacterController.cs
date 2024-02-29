using StateMachine;
using UnityEngine;
using VContainer.Unity;


public class InvertCondition : ICondition
{
    private readonly ICondition _condition;

    public InvertCondition(ICondition condition)
    {
        _condition = condition;
    }

    public bool Check()
    {
        return !_condition.Check();
    }
}

public class GroundedChecker : ICondition
{
    private readonly LayerMask _groundLayer;
    private readonly ICharacterContainer _character;
    private readonly Collider2D[] results = new Collider2D[6];

    public GroundedChecker(ICharacterContainer character, LayerMask groundLayer)
    {
        _character = character;
        _groundLayer = groundLayer;
    }

    public bool Check()
    {
        var count = Physics2D.OverlapCircleNonAlloc(_character.Rigidbody.position, 0.9f, results, _groundLayer);
        if (count > 0)
        {
            return true;
        }

        return false;
    }
}


public class CharacterController : ITickable, IFixedTickable
{
    private readonly SimpleFsm _fsm;

    private const string _stateRun = "Run";
    private const string _stateJump = "Jump";
    private const string _stateFall = "Fall";
    
    public CharacterController(ICharacterContainer character, IInputAxis inputAxis, IInputJump inputJump, LayerMask groundLayer)
    {
        _fsm = new SimpleFsm();
        var jumpState = new CharacterJump(character);
        
        _fsm.AddState(_stateRun,new CharacterRun(character, inputAxis));
        _fsm.AddState(_stateJump, jumpState);
        _fsm.AddState(_stateFall, new CharacterFall(character));

        _fsm.AddTransition(_stateRun, _stateJump, new DelegateCondition(inputJump.GetIsJump));
        _fsm.AddTransition(_stateRun, _stateFall, new InvertCondition(new GroundedChecker(character, groundLayer)));
        
        _fsm.AddTransition(_stateJump, _stateFall, jumpState);
        _fsm.AddTransition(_stateFall, _stateRun, new GroundedChecker(character, groundLayer));
        
        _fsm.LaunchState(_stateRun);
    }
    

    public void Tick()
    {
        _fsm.Tick();
    }
    public void FixedTick()
    {
        _fsm.FixedTick();
    }
}

public sealed class CharacterFall : IState, ITickable, IFixedTickable
{
    private ICharacterContainer _character;
    private float _fallSpeed;

    public CharacterFall(ICharacterContainer character)
    {
        _character = character;
    }
    
    public void Enter()
    {
        _fallSpeed = 0;
        Debug.Log("fall start");
        //animator is fall = true;
    }

    public void Exit()
    {
        //animator is fall = false;
        Debug.Log("fall end");
    }
    
    public void Tick()
    {
        _fallSpeed += Physics2D.gravity.y * Time.deltaTime;
    }

    public void FixedTick()
    {
        var moveDirection = Vector2.down * (_fallSpeed * Time.fixedDeltaTime);
        var distance = moveDirection.magnitude;
        _character.Rigidbody.position -= moveDirection.normalized * distance;
    }
}

public sealed class CharacterRun : IState, ITickable, IFixedTickable
{
    private ICharacterContainer _character;
    private IInputAxis _inputAxis;
    private float _runSpeed = 5; 

    public CharacterRun(ICharacterContainer character, IInputAxis inputAxis)
    {
        _character = character;
        _inputAxis = inputAxis;
    }
    
    public void Enter()
    {
        //Animator.SetFloat(AnimationConstants.VelocityXAnimatorId, Mathf.Abs(_runningSpeed) / BaseSpeed);
    }

    public void Exit()
    {
        
    }

    public void Tick()
    {
        //_runningSpeed = CalculateSpeed();
        //Debug.Log("v" + _character.Rigidbody.GetPointVelocity(_character.Rigidbody.position));
    }

    public void FixedTick()
    {
        var speed = _inputAxis.GetAxis().x * _runSpeed * Time.fixedDeltaTime * 50;
        _character.Rigidbody.velocity = new Vector2(speed, 0);
    }
}

public sealed class CharacterJump : IState, ITickable, IFixedTickable, ICondition
{
    private float _jumpSpeed;
    private readonly ICharacterContainer _character;

    public CharacterJump(ICharacterContainer character)
    {
        _character = character;
    }

    public void Enter()
    {
        _jumpSpeed = 6f;
    }

    public void Exit()
    {
    }

    public void Tick()
    {
        _jumpSpeed += Physics2D.gravity.y * Time.deltaTime;
    }

    public void FixedTick()
    {
        var moveDirection = Vector2.up * (_jumpSpeed * Time.fixedDeltaTime);
        var distance = moveDirection.magnitude;
        _character.Rigidbody.position += moveDirection.normalized * distance;
        //_character.Animator.SetBool(AnimationConstants.GroundedAnimatorId, _isGrounded);
    }

    public bool Check()
    {
        return _jumpSpeed <= 0;
    }
}