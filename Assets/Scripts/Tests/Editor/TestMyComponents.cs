﻿using System;
using System.Collections.Generic;
using Core;
using Game.Properties;
using Game.Shared;
using MessagePipe;
using NUnit.Framework;
using UnityEngine;

namespace TestEffects
{
    public class TestMyComponents
    {
        //base interfaces
        //базовый скелет сущности и компонента
        private interface IEntity{}
        private interface IEntityContainer : IEntity, IDisposable
        {
            bool TryGetComponent<T>(out T component) where T : IComponent;
        }
        
        private interface IComponent{}
        private interface IComponentFactory
        {
            IProperties CreateProperties(IEntityContainer container);
            IEffects CreateEffects(IEntityContainer container);
        }
        
        //кровеносная и нервная системмы
        //interfaces 2 level
        private interface IProperties : IComponent
        {
            float GetValue(CharacterProperty characterProperty);
            bool Has(CharacterProperty characterProperty);

            //???
            void AddProperty(CharacterProperty characterProperty, float baseValue);

            void AddModifier(CharacterProperty characterProperty, BaseProperty modifier);
            void RemoveModifier(CharacterProperty characterProperty, BaseProperty modifier);
        }
        private interface IEffect : IUpdate
        {
            void Start(IEntityContainer entityContainer);
            void End(IEntityContainer entityContainer);
            //Tick(deltaTime);
            bool CanApply(IEntityContainer entityContainer);
            bool IsDone { get; }
        }
        private interface IEffects : IComponent, IUpdate
        {
            void AddEffect(IEffect effect);
        }

 
        //concrete imp hierarchy
        //мышцы и сухожилия - крепятся к скелету, снабжаются кровью
        private class ComponentsFactory : IComponentFactory
        {
            public IProperties CreateProperties(IEntityContainer container)
            {
                return new Properties();
            }

            public IEffects CreateEffects(IEntityContainer container)
            {
                return new Effects(container);
            }
        }
        private class Properties : IProperties
        {
            private readonly Dictionary<CharacterProperty, ModifiedProperty> _properties = new Dictionary<CharacterProperty, ModifiedProperty>();

            public float GetValue(CharacterProperty characterProperty)
            {
                return GetProperty(characterProperty).GetValue();
            }

            public bool Has(CharacterProperty characterProperty)
            {
                return _properties.ContainsKey(characterProperty);
            }

            public void AddProperty(CharacterProperty characterProperty, float baseValue)
            {
                if (_properties.ContainsKey(characterProperty))
                {
                    throw new ArgumentException($"Property {characterProperty} already exists");
                }

                _properties.Add(characterProperty, new ModifiedProperty(baseValue));
            }

            private ModifiedProperty GetProperty(CharacterProperty characterProperty)
            {
                if (_properties.ContainsKey(characterProperty))
                {
                    return _properties[characterProperty];
                }

                throw new ArgumentException($"Can't find property {characterProperty}");
            }

            public void AddModifier(CharacterProperty characterProperty, BaseProperty modifier)
            {
                GetProperty(characterProperty).AddModifier(modifier);
            }

            public void RemoveModifier(CharacterProperty characterProperty, BaseProperty modifier)
            {
                GetProperty(characterProperty).RemoveModifier(modifier);
            }
        }
        private class Effects : IEffects
        {
            private readonly List<IEffect> _effects = new List<IEffect>();
            private readonly List<IEffect> _doneEffects = new List<IEffect>();
            //iEffectsEventDispatcher

            private IEntityContainer _owner; //как внедрить контейнер, в котором я нахожусь?

            /// <summary>
            /// Если какая либо логика взаимодействует с другими компонентами сервисами, то передаем контейнер, с которого получаем нужные сервисы
            /// Если конкретно этот сервис не требует именно контейнер, есму нужны глобальная шина событий 
            /// тут 
            /// </summary>
            /// <param name="ownerEntity"></param>
            public Effects(IEntityContainer ownerEntity)
            {
                _owner = ownerEntity;
            }
            
            public void Dispose()
            {
                foreach (var effect in _effects)
                {
                    effect.End(_owner); // нужен ли?
                }
                _effects.Clear();
                _owner = null;
            }

            public void AddEffect(IEffect effect)
            {
                _effects.Add(effect);
                effect.Start(_owner);
            }

            public void Update(float deltaTime)
            {
                foreach (var effect in _effects)
                {
                    effect.Update(deltaTime);
                    if (effect.IsDone)
                    {
                        effect.End(_owner);
                        _doneEffects.Add(effect);
                    }
                }

                foreach (var doneEffect in _doneEffects)
                {
                    _effects.Remove(doneEffect);
                    if (doneEffect is IDisposable disposableEffect)
                    {
                        disposableEffect.Dispose();
                    }
                    //DispatchEffectDone? + _entityContainer
                }

                _doneEffects.Clear();
            }
        }
        
        
        //concrete 
        //все, что ниже - это конкретика и может переписываться и переконфигурироваться. 
        //кожа, жирок, волосяной покров, косметика и т.д
        private class ConcreteCharacter : IEntityContainer, IUpdate
        {
            public IProperties Properties { get; private set; }
            public IEffects Effects { get; private set; }

            public ConcreteCharacter(IComponentFactory componentFactory)
            {
                Effects = componentFactory.CreateEffects(this);
                Properties = componentFactory.CreateProperties(this); //+config?
            }

            public void Dispose()
            {
               //Effects.Dispose();
                //Properties.Dispose();
            }

            public bool TryGetComponent<T>( out T component) where T : IComponent
            {
                component = default;
                if (typeof(T) == typeof(IProperties))
                {
                    component = (T) Properties;
                    return Properties != null;
                }
                return false;
            }

            public void ApplyEffect(IEffect effect)
            {
                Effects.AddEffect(effect);
            }
            public void Update(float deltaTime)
            {
                Effects.Update(deltaTime);
            }
            
        }
        private class ConcreteForeverSlowEffect : IEffect
        {
            private readonly BaseProperty _modifier;
            private readonly float _time;
            private float _elapsedTime;

            public ConcreteForeverSlowEffect(float time, int value, float multiplier)
            {
                _modifier = new BaseProperty(value, multiplier);
                _time = time;
            }
            
            public void Start(IEntityContainer entityContainer)
            {
                if (entityContainer.TryGetComponent(out IProperties properties))
                {
                    properties.AddModifier(CharacterProperty.Speed, _modifier);
                    _elapsedTime = _time;
                }
            }
            
            public void End(IEntityContainer entityContainer)
            {
                if (entityContainer.TryGetComponent(out IProperties properties))
                {
                    properties.RemoveModifier(CharacterProperty.Speed, _modifier);
                }
            }

            public bool CanApply(IEntityContainer entityContainer)
            {
                return entityContainer.TryGetComponent(out IProperties properties); //Required properties?
            }

            public bool IsDone => _elapsedTime <= 0;
            public void Update(float deltaTime)
            {
                _elapsedTime -= deltaTime;
            }

            public void Dispose()
            {
            }
        }
        
        [Test]
        public void TestCreate()
        {
            var componentFactory = new ComponentsFactory();
            var character = new ConcreteCharacter(componentFactory);
            character.Properties.AddProperty(CharacterProperty.Speed, 5);
            character.ApplyEffect(new ConcreteForeverSlowEffect(1, 1,0));
            Assert.AreEqual(6, character.Properties.GetValue(CharacterProperty.Speed));
            
            character.Update(0.5f);
            Assert.AreEqual(6, character.Properties.GetValue(CharacterProperty.Speed));
            character.Update(1);
            Assert.AreEqual(5, character.Properties.GetValue(CharacterProperty.Speed));
        }
        
        //Вопрос в том, нужно ли делать подписку отписку или же попробовать заюзать очередь событий?
        //аргумент в пользу 1го - сервисы в .net сделаны подписками, + есть готовая шина событий, очередь сообщений - ни разу не юзал.

        public class PrimitiveGlobalEventBus
        {
            public event Action OnEvent;
            public void DispatchEvent()
            {
                OnEvent?.Invoke();
            }
        }
        
        //сли эффект подписывается на что либо, он вызывает отписку сам
        private class EventEndEffect : IEffect, IDisposable //отписка от событий
        {
            private readonly PrimitiveGlobalEventBus _bus;
            private bool _isDone;
            public bool IsDone => _isDone;
            public EventEndEffect(PrimitiveGlobalEventBus bus)
            {
                _bus = bus;
                _bus.OnEvent += BusOnOnEvent;
            }
            public void Dispose()
            {
                _bus.OnEvent -= BusOnOnEvent;
            }

            private void BusOnOnEvent()
            {
                Debug.Log("eventEffect triggered is done");
                _isDone = true;
            }

            public void Update(float deltaTime)
            {
            }

            public void Start(IEntityContainer entityContainer)
            {
                Debug.Log("apply eventEffect");
            }

            public void End(IEntityContainer entityContainer)
            {
                Debug.Log("cancel event effect");
            }

            public bool CanApply(IEntityContainer entityContainer)
            {
                return true;
            }
        }
        
        private class GlobalListenerEffect : IEffect , IDisposable
        {
            private readonly IDisposable _disposable;
            
            public GlobalListenerEffect(ISubscriber<IEffect> subscriber, IPublisher<IEffect> publisher)
            {
                _disposable = subscriber.Subscribe(Handler, Filter);
            }

            private bool Filter(IEffect arg)
            {
                //arg.IsDone
                return false;
            }

            private void Handler(IEffect obj)
            {
                throw new NotImplementedException();
            }


            public void Dispose()
            {
                _disposable?.Dispose();
            }

            public void Update(float deltaTime)
            {
                throw new NotImplementedException();
            }

            public void Start(IEntityContainer entityContainer)
            {
                throw new NotImplementedException();
            }

            public void End(IEntityContainer entityContainer)
            {
                throw new NotImplementedException();
            }

            public bool CanApply(IEntityContainer entityContainer)
            {
                throw new NotImplementedException();
            }

            public bool IsDone { get; }
        }

        private class CustomConfigEffect : IEffect
        {
            //iTrigger = triggerFactory.Create(type, params);
            //iModifier = modifiersFactory.Create(type, params)

            public void Update(float deltaTime)
            {
                throw new NotImplementedException();
            }

            public void Start(IEntityContainer entityContainer)
            {
                throw new NotImplementedException();
            }

            public void End(IEntityContainer entityContainer)
            {
                throw new NotImplementedException();
            }

            public bool CanApply(IEntityContainer entityContainer)
            {
                throw new NotImplementedException();
            }

            public bool IsDone { get; }
        }
        
        
        [Test]
        public void TestDispose()
        {
            //надо понять что есть диспоуз и когда он вызывается. По сути при диспозе будет отписка от событий.
            var eventBus = new PrimitiveGlobalEventBus();
            var componentFactory = new ComponentsFactory();
            var character = new ConcreteCharacter(componentFactory);
            character.ApplyEffect(new EventEndEffect(eventBus));
            eventBus.DispatchEvent();
            character.Update(1);
            eventBus.DispatchEvent();
        }


    }
    
}