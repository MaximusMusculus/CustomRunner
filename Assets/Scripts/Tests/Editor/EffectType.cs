using System.Collections.Generic;
using Core;
using Game.Conditions;
using Game.Conditions.Implementations;
using Game.Properties;
using Game.Shared;
using NUnit.Framework;


namespace TestEffects
{
    public enum TargetModify
    {
        None,
        CharacterProperty, //->PropertyModifier class
        Inventory,
        //ChangeState ??
        //abilitiies
        //??..
    }
    
    public interface IModifier
    {
        void Apply();
        void Cancel();
    }

    //BaseModifier -> concreteModifier
    public abstract class Modifier : IUpdate, IModifier
    {
        public virtual void Update(float deltaTime) { }

        public abstract void Apply();
        public abstract void Cancel();
    }
    
    public class PropertyModifier : Modifier
    {
        private readonly CharacterProperty _property;
        private readonly PropertyHolder _propertyHolder;
        private readonly BaseProperty _propertyModifier;

        public PropertyModifier(PropertyHolder propertyHolder, CharacterProperty property, float value, float multiplier) 
        {
            _property = property;
            _propertyHolder = propertyHolder;
            _propertyModifier = new BaseProperty(value, multiplier);
        }

        public override void Apply()
        {
            _propertyHolder.AddModifier(_property, _propertyModifier);
        }

        public override void Cancel()
        {
            _propertyHolder.RemoveModifier(_property, _propertyModifier);
        }
    }
    
    public abstract class Effect : IUpdate
    {
        public virtual void Update(float deltaTime) { }
        
        public abstract void Apply();
        public abstract void Cancel();
        public abstract bool IsDone();
    }

    public class ConditionEffect : Effect
    {
        private readonly CheckCondition _endCondition;
        private readonly Modifier _modifier;

        public ConditionEffect(CheckCondition endCondition, Modifier modifier)
        {
            _endCondition = endCondition;
            _modifier = modifier;
        }

        public override void Apply()
        {
            _endCondition.Reset();
            _modifier.Apply();
        }

        public override void Cancel()
        {
            _modifier.Cancel();
        }
        
        public override bool IsDone()
        {
            return _endCondition.Check();
        }

        public override void Update(float deltaTime)
        {
            _endCondition.Update(deltaTime);
            _modifier.Update(deltaTime);
        }
    }
    
    
    //где и кем создаются эффекты?
    public class EffectsService : IUpdate // iBroadcastReceiver
    {
        private readonly List<Effect> _effects = new List<Effect>();
        private readonly List<Effect> _doneEffects = new List<Effect>();

        public void AddEffect(Effect effect)
        {
            _effects.Add(effect);
            effect.Apply();
        }
        
        public void Update(float deltaTime)
        {
            foreach (var effect in _effects)
            {
                effect.Update(deltaTime);
                if (effect.IsDone())
                {
                    _doneEffects.Add(effect);
                }
            }

            foreach (var effect in _doneEffects)
            {
                effect.Cancel();
                DisposeEffect(effect);
                _effects.Remove(effect);
            }
            _doneEffects.Clear();
        }

        //разобрать эффект и(или) отправить его в пул
        protected virtual void DisposeEffect(Effect effect)
        {
        }

        public void Reset() //dispose?
        {
            foreach (var effect in _effects)
            {
                effect.Cancel();
                DisposeEffect(effect);
            }
            _effects.Clear();
            _doneEffects.Clear();
            OnReset();
        }
        protected virtual void OnReset(){}
        
    }
    
    
    

    public class TestEffects
    {
        private const int Speed = 5;
        private const int EffectTime = 1;
        
        private PropertyHolder _propertyHolder;
        
        

        [SetUp]
        public void Setup()
        {
            _propertyHolder = new PropertyHolder();
            _propertyHolder.AddProperty(CharacterProperty.Speed, Speed);
        }

        [Test]
        public void TestTimedSpeedUp()
        {
            var effect = new ConditionEffect(new CheckElapsedTime(EffectTime), new PropertyModifier(_propertyHolder, CharacterProperty.Speed, 1, 0));
            var service = new EffectsService();
            service.AddEffect(effect);
            service.Update(0.5f);
            Assert.AreEqual(Speed + 1, _propertyHolder.GetValue(CharacterProperty.Speed));
            service.Update(1);
            Assert.AreEqual(Speed, _propertyHolder.GetValue(CharacterProperty.Speed));
        }

    }

}