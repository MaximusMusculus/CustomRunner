using System;
using System.Collections.Generic;
using Core;
using Game.Components;
using Game.Properties;
using Game.Shared;
using UnityEngine;

/// <summary>
/// Контейнер компонент
/// </summary>
public class ComponentsContainer : IComponentsEntity, IUpdate, IFixedUpdate
{
    private const int DefaultComponentsCount = 8;
    private readonly List<IUpdate> _updateComponents = new List<IUpdate>(DefaultComponentsCount);
    private readonly List<IFixedUpdate> _fixedUpdates = new List<IFixedUpdate>(DefaultComponentsCount);
    private Dictionary<Type, IComponent> _components = new Dictionary<Type, IComponent>(DefaultComponentsCount);

    public bool TryGetComponent<T>(out T component) where T : IComponent
    {
        var result = _components.TryGetValue(typeof(T), out var cmp);
        component = (T) cmp;
        return result;
    }

    private ComponentsContainer() { }
    private void AddComponent<TComponent>(TComponent component) where TComponent : IComponent
    {
        if (!typeof(TComponent).IsInterface)
        {
            throw new ArgumentException(component.GetType() + " is not interface");
        }
        var type = typeof(TComponent);
        
        _components.Add(type, component);
        if (component is IUpdate update)
        {
            _updateComponents.Add(update);
        }

        if (component is IFixedUpdate fixedUpdate)
        {
            _fixedUpdates.Add(fixedUpdate);
        }
    }
    

    public void Update(float deltaTime)
    {
        foreach (var component in _updateComponents)
        {
            component.Update(deltaTime);
        }
    }
    public void FixedUpdate()
    {
        foreach (var component in _fixedUpdates)
        {
            component.FixedUpdate();
        }
    }
    
    public interface IEntityBuilder
    {
        IEntityBuilder NewEntity();
        IEntityBuilder AddProperty(PropertyConfig config);
        IEntityBuilder AddMove(Rigidbody2D rigidbody);
        IEntityBuilder AddBehaviourComponent();

        //for test
        IEntityBuilder AddAnimatorComponent(Animator animator);
        ComponentsContainer GetEntity();
    }
    public static IEntityBuilder Builder(IComponentsFactory componentsFactory)
    {
        return new EntityBuilder(componentsFactory);
    }
    public class EntityBuilder : IEntityBuilder
    {
        private readonly IComponentsFactory _componentsFactory;
        
        private ComponentsContainer _entity;
        public EntityBuilder(IComponentsFactory componentsFactory)
        {
            _componentsFactory = componentsFactory;
        }
        
        public IEntityBuilder NewEntity()
        {
            _entity = new ComponentsContainer();
            return this;
        }

        public IEntityBuilder AddProperty(PropertyConfig config)
        {
            _entity.AddComponent(_componentsFactory.CreatePropertyComponent(config));
            return this;
        }
        
        public IEntityBuilder AddMove(Rigidbody2D rigidbody) //type
        {
            _entity.AddComponent(_componentsFactory.CreateMoveComponent(rigidbody));
            return this;
        }

        public IEntityBuilder AddBehaviourComponent()
        {
            //Factory.CreateBehaviour(type);
            _entity.AddComponent<IBehaviourComponent>(new BehaviourComponent());
            return this;
        }
        
        
        // визуал вынести в отдельный менеджер, тут для теста
        public IEntityBuilder AddAnimatorComponent(Animator animator)//+config?
        {
            _entity.TryGetComponent<IMoveComponent>(out var moveComponent);
            _entity.TryGetComponent<IPropertyComponent>(out var propertyComponent);
            _entity.AddComponent<IAnimationComponent>(new AnimationComponent(animator, moveComponent, propertyComponent));
            return this;
        }
        
        public ComponentsContainer GetEntity()
        {
            var result = _entity;
            _entity = null;
            return result;
        }
        
    }
}