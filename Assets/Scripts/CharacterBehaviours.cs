using StateMachine;
using UnityEngine;
using VContainer.Unity;


public class OverlapCircleChecker 
{
    private readonly LayerMask _groundLayer;
    private readonly ICharacterContainer _character;
    private readonly Collider2D[] results = new Collider2D[6];
    private readonly float _radius;

    public OverlapCircleChecker(ICharacterContainer character, LayerMask groundLayer, float radius)
    {
        _character = character;
        _groundLayer = groundLayer;
        _radius = radius;
    }

    public bool Check()
    {
        var count = Physics2D.OverlapCircleNonAlloc(_character.Rigidbody.position, _radius, results, _groundLayer);
        if (count > 0)
        {
            return true;
        }
        return false;
    }
}


public class CharacterFall : IState, ITickable, IFixedTickable
{
    private readonly ICharacterContainer _character;
    private readonly IInputAxis _inputAxis;
    private float _fallSpeed;
    private float _fallControlForce = 5;

    public CharacterFall(ICharacterContainer character, IInputAxis inputAxis)
    {
        _character = character;
        _inputAxis = inputAxis;
    }
    
    public void Enter()
    {
        _fallSpeed = 0;
        _character.Animator.SetBool(AnimationConstants.IsGrounded, false);
    }

    public void Exit()
    {
    }
    
    public void Tick()
    {
        _fallSpeed += Physics2D.gravity.y * Time.deltaTime;
    }

    public void FixedTick()
    {
        var fallDirection = new Vector2(_inputAxis.GetAxis().x, 0) * Time.fixedDeltaTime * _fallControlForce;
        var moveDirection = Vector2.down * (_fallSpeed * Time.fixedDeltaTime) - fallDirection;
        var distance = moveDirection.magnitude;
        _character.Rigidbody.position -= moveDirection.normalized * distance;
    }
}

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

public class CharacterRun : IState, ITickable, IFixedTickable
{
    private ICharacterContainer _character;
    private IInputAxis _inputAxis;
    private float _runSpeed; 

    public CharacterRun(ICharacterContainer character, IInputAxis inputAxis)
    {
        _character = character;
        _inputAxis = inputAxis;
    }
    
    public void Enter()
    {
        _character.Animator.SetBool(AnimationConstants.IsGrounded, true);
        //Animator.SetFloat(AnimationConstants.VelocityXAnimatorId, Mathf.Abs(_runningSpeed) / BaseSpeed);
    }

    public void Exit()
    {
    }

    public void Tick()
    {
        _runSpeed = _character.BaseValues.Get(FloatProperty.RunSpeed);
    }

    public void FixedTick()
    {
        var speed = _inputAxis.GetAxis().x * _runSpeed * Time.fixedDeltaTime;
        _character.Rigidbody.position += new Vector2(speed, 0);
    }
}

public class CharacterJump : IState, ITickable, IFixedTickable
{
    private readonly ICharacterContainer _character;
    private readonly IInputAxis _inputAxis;
    private float _moveSpeed;
    private float _jumpSpeed;

    public CharacterJump(ICharacterContainer character, IInputAxis inputAxis)
    {
        _character = character;
        _inputAxis = inputAxis;
    }

    public void Enter()
    {
        _jumpSpeed = _character.Values.Get(FloatProperty.JumpSpeed);
        _character.Animator.SetTrigger(AnimationConstants.OnJump);
        _character.Animator.SetBool(AnimationConstants.IsGrounded, false);
    }

    public void Exit()
    {
    }

    public void Tick()
    {
        _jumpSpeed += Physics2D.gravity.y * Time.deltaTime;
        _moveSpeed = _character.Values.Get(FloatProperty.RunSpeed);
    }

    public void FixedTick()
    {
        var jumpDirection = new Vector2(_inputAxis.GetAxis().x, 0)* Time.fixedDeltaTime * _moveSpeed;
        var moveDirection = Vector2.up * (_jumpSpeed * Time.fixedDeltaTime) + jumpDirection;
        var distance = moveDirection.magnitude;
        _character.Rigidbody.position += moveDirection.normalized * distance;
    }

    public bool IsJumpEnd()
    {
        return _jumpSpeed <= 0;
    }
}