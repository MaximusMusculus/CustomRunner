using System;
using System.Collections.Generic;
using Game.Components;
using UnityEngine;
using VContainer.Unity;


public interface ICharacterContainer : IComponentsEntity, ITickable, IFixedTickable
{
    IMoveComponent MoveComponent { get; }
    Animator Animator { get; }
}


public class CharacterContainer : ICharacterContainer
{
    private Dictionary<Type, IComponent> _components = new Dictionary<Type, IComponent>();
    public Rigidbody2D Rigidbody { get; }
    public IMoveComponent MoveComponent => _moveComponent;
    private readonly MoveComponent _moveComponent;
    
    public Animator Animator { get; }

    public CharacterContainer(Rigidbody2D rigidbody, Animator animator)
    {
        Rigidbody = rigidbody;
        Animator = animator;
        _moveComponent = new MoveComponent(rigidbody);
        _components.Add(typeof(IMoveComponent), MoveComponent);

    }
    
    public bool TryGetComponent<T>(out T component) where T : IComponent
    {
        var result =_components.TryGetValue(typeof(T), out var cmp);
        component = (T) cmp;
        return result;
    }

    public void Tick()
    {
        _moveComponent.Update(Time.deltaTime);
    }

    public void FixedTick()
    {
        _moveComponent.FixedUpdate();
    }
}