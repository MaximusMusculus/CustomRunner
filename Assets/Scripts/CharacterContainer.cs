using UnityEngine;


public interface ICharacterContainer
{
    Rigidbody2D Rigidbody { get; }
    Animator Animator { get; }
    
    IProperties BaseValues { get; }
    IProperties Values { get; }
}

public class CharacterContainer : MonoBehaviour, ICharacterContainer
{
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _runSpeed;
    [SerializeField] private float _flyHeight;
    [SerializeField] private float _jumpSpeed;

    public Rigidbody2D Rigidbody => _rigidbody;
    public Animator Animator => _animator;
    
    public IProperties BaseValues { get; private set; }
    public IProperties Values { get; private set; }
    public IModifyingProperties ModifyingProperties { get; private set; }

    
    /// Создание и конфигурацию перенести в объект, где создается персонаж. Это явно делается не здесь, но неуспеваю =(
    protected void Awake()
    {
        var baseValues = new Properties();
        baseValues.Set(FloatProperty.FlyHeight, _flyHeight);
        baseValues.Set(FloatProperty.RunSpeed, _runSpeed);
        baseValues.Set(FloatProperty.JumpSpeed, _jumpSpeed);
        
        ModifyingProperties = new ModifyProperties(baseValues);
        Values = ModifyingProperties;
    }
}
