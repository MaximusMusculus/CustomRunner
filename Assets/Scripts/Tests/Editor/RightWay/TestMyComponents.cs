using System;
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
        
        public interface IDefinition { }
        private interface IEntity{}
        
        private interface IEntityContainer : IEntity, IDisposable
        {
            bool TryGetComponent<T>(out T component) where T : IComponent;
            //GetSnapshot(); //Components + datas
        }

        private interface IComponent
        {
            IComponentData GetData();
        }
        private interface IComponentData
        {
            void Restore(IComponent component);
        }
        
        private interface IComponentFactory
        {
            IProperties CreateProperties(IEntityContainer container);
            IEffects CreateEffects(IEntityContainer container);
        }
        
        //кровеносная и нервная системмы
        //interfaces 2 level
        private interface IProperties : IComponent
        {
            float GetValue(TypeProperty typeProperty);
            bool Has(TypeProperty typeProperty);

            //???
            void AddProperty(TypeProperty typeProperty, float baseValue);

            void AddModifier(TypeProperty typeProperty, BaseProperty modifier);
            void RemoveModifier(TypeProperty typeProperty, BaseProperty modifier);
        }
        private interface IEffect : IUpdate
        {
            void ApplyTo(IEntityContainer entityContainer);
            void RemoveFrom(IEntityContainer entityContainer);
            //Tick(deltaTime);
            bool CanApply(IEntityContainer entityContainer);
            bool IsDone { get; }
        }
        public interface IEffectDefinition : IDefinition { }
        
        private interface IEffects : IComponent, IUpdate
        {
            void AddEffect(IEffect effect);
            void AddEffect(IEffectDefinition definition);
        }
        
        /// <summary>
        /// Модель эффекта содержит данные по созднаию...?
        /// </summary>
        interface IEffectModel
        {
            //type
            //config?
        }
        
        
        private class EffectFactory 
        {
            //что является моделью описания? -> созданный класс? Или id?
            public IEffect Create(IEffectModel effectModel)
            {
                return null;
            }
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
            private readonly Dictionary<TypeProperty, ModifiedProperty> _properties = new Dictionary<TypeProperty, ModifiedProperty>();

            public float GetValue(TypeProperty typeProperty)
            {
                return GetProperty(typeProperty).GetValue();
            }

            public bool Has(TypeProperty typeProperty)
            {
                return _properties.ContainsKey(typeProperty);
            }

            public void AddProperty(TypeProperty typeProperty, float baseValue)
            {
                if (_properties.ContainsKey(typeProperty))
                {
                    throw new ArgumentException($"Property {typeProperty} already exists");
                }

                _properties.Add(typeProperty, new ModifiedProperty(baseValue));
            }

            private ModifiedProperty GetProperty(TypeProperty typeProperty)
            {
                if (_properties.ContainsKey(typeProperty))
                {
                    return _properties[typeProperty];
                }

                throw new ArgumentException($"Can't find property {typeProperty}");
            }

            public void AddModifier(TypeProperty typeProperty, BaseProperty modifier)
            {
                GetProperty(typeProperty).AddModifier(modifier);
            }

            public void RemoveModifier(TypeProperty typeProperty, BaseProperty modifier)
            {
                GetProperty(typeProperty).RemoveModifier(modifier);
            }

            public IComponentData GetData()
            {
                throw new NotImplementedException();
            }
        }
        
        
        
        
        /// <summary>
        /// Знает про эффекты ВСЕ, кроме создания - этим занимается фабрика 
        /// Компонент обрабатывающий эффекты. Порождает эффекты, обновляет, следит за их состоянием, удаляет, диспоузит те, что нужно
        /// Может использовать пул, так как, кроме него никто не может породить эффект.
        /// Так же может выдать список эффектов, список отфильтрованных эффектов
        /// может прогнать по эффектам визитора (если потребуется).
        ///
        /// Кстати, при создании может принять некие "правила" для эффектов, какой эффект замещает другой, какой эффект  обновляет время действия и тд.
        /// </summary>
        private class Effects : IEffects
        {
            private readonly List<IEffect> _processEffects = new List<IEffect>();
            private readonly List<IEffect> _doneEffects = new List<IEffect>();
            private IEntityContainer _target; //как внедрить контейнер, в котором я нахожусь?
            
            //-сюда внедрить пул, фабрику. Порождать эффекты здесь, нести за эффекты ответственность. 
            // когда эффекты кончаются - закидывать в пул.

            /// <summary>
            /// Если какая либо логика взаимодействует с другими компонентами сервисами, то передаем контейнер, с которого получаем нужные сервисы
            /// Если конкретно этот сервис не требует именно контейнер, есму нужны глобальная шина событий 
            /// тут 
            /// </summary>
            /// <param name="targetEntity"></param>
            public Effects(IEntityContainer targetEntity)
            {
                _target = targetEntity;
            }
            
            public void AddEffect(IEffect effect) //config + args?
            {
                _processEffects.Add(effect);
                effect.ApplyTo(_target);
            }

            public void AddEffect(IEffectDefinition definition)
            {
                throw new NotImplementedException();
            }
            //CancelEffect(id), or Rules(), orFilter();  
            

            public void Update(float deltaTime)
            {
                foreach (var effect in _processEffects)
                {
                    effect.Update(deltaTime);
                    if (effect.IsDone)
                    {
                        effect.RemoveFrom(_target);
                        _doneEffects.Add(effect);
                    }
                }
                
                foreach (var doneEffect in _doneEffects)
                {
                    _processEffects.Remove(doneEffect);
                    //DispatchEffectDone? + _entityContainer
                }
                _doneEffects.Clear();
            }

            public IComponentData GetData()
            {
                IComponentData da = null;
                da.Restore(this);
                throw new NotImplementedException();
            }
        }
        
        
        //-- пример взаимодействия окружения с объектом
        //в данном случае, мне необязательно знать что я работаю с компонентной сущностью
        private interface IVisitor
        {
            void Visit(IVisitTarget target);
        }
        private interface IVisitTarget
        {
            void AddEffect(IEffect effect);
        }
        private class EffectItemVisitor : IVisitor
        {
            public void Visit(IVisitTarget target)
            {
                target.AddEffect(new CompositeEffect());
            }
        }
        //--




        //concrete 
        //все, что ниже - это конкретика и может переписываться и переконфигурироваться. 
        //кожа, жирок, волосяной покров, косметика и т.д
        
        /// <summary>
        /// Как я хочу взаимодействовать с персонажем? Это же сложное поведение.
        /// Хочу ли я подбирать предметы сам или же предметы будут подбираться сами (при столкновении)
        ///
        /// PicUpItemsComponent
        /// </summary>
        private class CharacterEntity : IEntityContainer, IUpdate, IVisitTarget
        {
            public IProperties Properties { get; private set; } //intelegence... con, agility
            //healths, mana etc
            public IEffects Effects { get; private set; }
            
            //abilities
            //....??

            public CharacterEntity(IComponentFactory componentFactory) //config???
            {
                Effects = componentFactory.CreateEffects(this);
                Properties = componentFactory.CreateProperties(this); //+config?
            }
            
            public void Dispose()
            {
               //Effects.Dispose();
              //  Properties.Dispose();
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

            //visitor - visit?
            public void AddEffect(IEffect effect)
            {
                throw new NotImplementedException();
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
            
            public void ApplyTo(IEntityContainer entityContainer)
            {
                if (entityContainer.TryGetComponent(out IProperties properties))
                {
                    properties.AddModifier(TypeProperty.Speed, _modifier);
                    _elapsedTime = _time;
                }
            }
            
            public void RemoveFrom(IEntityContainer entityContainer)
            {
                if (entityContainer.TryGetComponent(out IProperties properties))
                {
                    properties.RemoveModifier(TypeProperty.Speed, _modifier);
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
            
            
            public class ConcreteForeverSlowEffectSnap 
            {
                public float ElapsedTime;

                public ConcreteForeverSlowEffectSnap(float elapsedTime)
                {
                    ElapsedTime = elapsedTime;
                }
                
                public void Recovery(ConcreteForeverSlowEffect effect)
                {
                    effect._elapsedTime = ElapsedTime;
                }
            }
            public ConcreteForeverSlowEffectSnap GetSnap()
            {
                return new ConcreteForeverSlowEffectSnap(_elapsedTime);
            }
        }


        //дот живет, пока не умрет источник
        private class LiveSourcerDot : IEffect
        {
            private IEntity _source;
            private IProperties _targetProperties;
            
            //Конструкто- шина событий, подписка на события смерти персонажа, с фильтром по нашей цели.
            
            
            public void Apply(IEntityContainer target, IEntity source)
            {
                _source = source;
                if(target.TryGetComponent(out IProperties properties))
                {
                    _targetProperties = properties;
                }
            }
            
            
            public void ApplyTo(IEntityContainer entityContainer) //source
            {
                if(entityContainer.TryGetComponent(out IProperties properties))
                {
                    _targetProperties = properties;
                }
            }
            
            public void RemoveFrom(IEntityContainer entityContainer)
            {
            }

            public bool CanApply(IEntityContainer entityContainer)
            {
                return true;
            }

            public void Update(float deltaTime)
            {
                //каждый кадр наносить определенный урон от источника с id=_source
            }

            public bool IsDone => false;
        }

        [Test]
        public void SnapRecovery()
        {
            var effect = new ConcreteForeverSlowEffect(5, 1, 0);
            effect.Update(0.1f);
            
            var snap = effect.GetSnap(); 
            effect.Update(0.1f);
            effect.Update(0.1f);
            
            snap.Recovery(effect);
            effect.Update(0.1f);
        }
        
        [Test]
        public void TestCreate()
        {
            var componentFactory = new ComponentsFactory();
            var character = new CharacterEntity(componentFactory);
            character.Properties.AddProperty(TypeProperty.Speed, 5);
            character.ApplyEffect(new ConcreteForeverSlowEffect(1, 1,0));
            Assert.AreEqual(6, character.Properties.GetValue(TypeProperty.Speed));
            
            character.Update(0.5f);
            Assert.AreEqual(6, character.Properties.GetValue(TypeProperty.Speed));
            character.Update(1);
            Assert.AreEqual(5, character.Properties.GetValue(TypeProperty.Speed));
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
        private class EventEndEffect : IEffect, IDisposable
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
                //_disposable.Dispose();
            }

            private void BusOnOnEvent()
            {
                Debug.Log("eventEffect triggered is done");
                _isDone = true;
            }

            public void Update(float deltaTime)
            {
            }

            public void ApplyTo(IEntityContainer entityContainer)
            {
                Debug.Log("apply eventEffect");
            }

            public void RemoveFrom(IEntityContainer entityContainer)
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

            public void ApplyTo(IEntityContainer entityContainer)
            {
                throw new NotImplementedException();
            }

            public void RemoveFrom(IEntityContainer entityContainer)
            {
                throw new NotImplementedException();
            }

            public bool CanApply(IEntityContainer entityContainer)
            {
                throw new NotImplementedException();
            }

            public bool IsDone { get; }
        }

        
        //CompositeEffectBuilder.addTrigger().addModify()
        public interface ISnapshot
        {
        }
        public interface ISnapshotable
        {
            ISnapshot GetSnapshot();
            void Recovery(ISnapshot snapshot);
        }
        
        
        public interface ISnap
        {
            void Recovery(ISnapped target, bool recreate = false);
        }
        
        public interface ISnapped
        {
            ISnap GetSnap();
        }
        public class Snapped : ISnapped
        {
            private string _target;

            public ISnap GetSnap()
            {
                return new TiggerSnap(_target);
            }

            public class TiggerSnap : ISnap
            {
                public string targetName;

                public TiggerSnap(string targetName)
                {
                    this.targetName = targetName;
                }

                public void Recovery(ISnapped target, bool recreate = false)
                {
                    (target as Snapped)._target = targetName;
                    //могу сохранить не только снимок, но и доп поля, по которым могу пересоздать объект
                }
            }
        }
        
        private class CompositeEffect : IEffect, IDisposable
        {
            //iTrigger = triggerFactory.Create(type, params);   //
            //iModifier = modifiersFactory.Create(type, params) //
            //EffectFactory.Create(configEffect,entityContainer);
            private ISnapped _snapped;
            
            public class CompositeEffectSnap
            {
                public ISnap snap;

                public CompositeEffectSnap(ISnap snap)
                {
                    this.snap = snap;
                }

                public void Recovery(CompositeEffect effect, bool recreate = false)
                {
                    //effect._compositeTrigger = Factory.Create(type);
                    snap.Recovery(effect._snapped, recreate);
                }
            }

            public CompositeEffectSnap GetSnap()
            {
                return new CompositeEffectSnap(_snapped.GetSnap());
            }


            public void Update(float deltaTime)
            {
                throw new NotImplementedException();
            }

            public void ApplyTo(IEntityContainer entityContainer)
            {
                throw new NotImplementedException();
            }

            public void RemoveFrom(IEntityContainer entityContainer)
            {
                throw new NotImplementedException();
            }

            public bool CanApply(IEntityContainer entityContainer)
            {
                throw new NotImplementedException();
            }

            public bool IsDone { get; }

            //--
            public void Dispose() { }
        }
        
        
        [Test]
        public void TestDispose()
        {
            //надо понять что есть диспоуз и когда он вызывается. По сути при диспозе будет отписка от событий.
            //За объект ответсвеннен тот, кто его создал.
            
            var eventBus = new PrimitiveGlobalEventBus();
            var componentFactory = new ComponentsFactory();
            var character = new CharacterEntity(componentFactory);
            character.ApplyEffect(new EventEndEffect(eventBus));
            eventBus.DispatchEvent();
            character.Update(1);
            eventBus.DispatchEvent();
        }


    }
    
}