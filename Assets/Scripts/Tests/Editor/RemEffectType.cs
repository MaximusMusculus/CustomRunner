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
    
    public interface ITestModifier
    {
        void Apply();
        void Cancel();
    }

    //BaseModifier -> concreteModifier
    public abstract class TestTestModifier : IUpdate, ITestModifier
    {
        public virtual void Update(float deltaTime) { }
        public abstract void Apply();
        public abstract void Cancel();
    }
    
    public class TestTestPropertyTestTestModifier : TestTestModifier
    {
        private readonly CharacterProperty _property;
        private readonly PropertyComponent _propertyComponent;
        private readonly BaseProperty _propertyModifier;

        public TestTestPropertyTestTestModifier(PropertyComponent propertyComponent, CharacterProperty property, float value, float multiplier) 
        {
            _property = property;
            _propertyComponent = propertyComponent;
            _propertyModifier = new BaseProperty(value, multiplier);
        }

        public override void Apply()
        {
            _propertyComponent.AddModifier(_property, _propertyModifier);
        }

        public override void Cancel()
        {
            _propertyComponent.RemoveModifier(_property, _propertyModifier);
        }
    }
    
    public abstract class TestEffect : IUpdate
    {
        public virtual void Update(float deltaTime) { }
        public virtual bool IsCanApply() { return true; }
        public abstract void Apply();
        public abstract void Cancel();
        public abstract bool IsDone();
    }


    //can reset ->
    public class TestTestEffectSimple : TestEffect
    {
        private readonly CheckCondition _endCondition;
        private readonly TestTestModifier _testTestModifier;

        public TestTestEffectSimple(PropertyComponent propertyComponent, CharacterProperty property, float value, float multiplier, float duration)
        {
            _endCondition = new CheckElapsedTime(duration);
            _testTestModifier = new TestTestPropertyTestTestModifier(propertyComponent, property, value, multiplier);
        }

        public override void Apply()
        {
            _testTestModifier.Apply();  //где будут находится правила наложения модификаторов (не накладывать 100-500 одинаковых)
        }

        public override void Cancel()
        {
            _testTestModifier.Cancel();
        }
        
        public override bool IsDone()
        {
            return _endCondition.Check();
        }

        public override void Update(float deltaTime)
        {
            _endCondition.Update(deltaTime);
            _testTestModifier.Update(deltaTime);
        }
    }
    

   


    //-- конкретный эффект на основе энама. для упрощения создания
    public class TestConditionTestEffect : TestEffect
    {
        private readonly CheckCondition _endCondition;
        private readonly TestTestModifier _testTestModifier;

        public TestConditionTestEffect(CheckCondition endCondition, TestTestModifier testTestModifier)
        {
            _endCondition = endCondition;
            _testTestModifier = testTestModifier;
        }
        
        public override void Apply()
        {
            _endCondition.Reset();
            _testTestModifier.Apply();  //где будут находится правила наложения модификаторов (не накладывать 100-500 одинаковых)
        }

        public override void Cancel()
        {
            _testTestModifier.Cancel();
        }
        
        public override bool IsDone()
        {
            return _endCondition.Check();
        }

        public override void Update(float deltaTime)
        {
            _endCondition.Update(deltaTime);
            _testTestModifier.Update(deltaTime);
        }
    }
    
    
    
    //где и кем создаются эффекты?
    //CreateEffect->config+target
    /// как быть с правилами наложения эффектов? В конфиге? А как быть с эффектами, которые отменяют друг друга?
    /// нужен список эффектов на объекте, по типу холдера Параметров ()
    public class TestEffectsService : IUpdate // iBroadcastReceiver
    {
        private readonly List<TestEffect> _newEffects = new List<TestEffect>();
        private readonly List<TestEffect> _effects = new List<TestEffect>();
        private readonly List<TestEffect> _doneEffects = new List<TestEffect>();

        public void AddEffect(TestEffect testEffect) //target + rules (effectId) не добавлять тот же эффект?
        {
            _newEffects.Add(testEffect);
        }
        
        public void Update(float deltaTime)
        {
            foreach (var effect in _newEffects)
            {
                _effects.Add(effect);
                effect.Apply();
            }
            _newEffects.Clear();
            
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
         protected virtual void DisposeEffect(TestEffect testEffect)
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
        
        private PropertyComponent _propertyComponent;
        
        

        [SetUp]
        public void Setup()
        {
            _propertyComponent = new PropertyComponent();
            _propertyComponent.AddProperty(CharacterProperty.Speed, Speed);
        }

        [Test]
        public void TestTimedSpeedUp()
        {
            var effect = new TestConditionTestEffect(new CheckElapsedTime(EffectTime), new TestTestPropertyTestTestModifier(_propertyComponent, CharacterProperty.Speed, 1, 0));
            var service = new TestEffectsService();
            service.AddEffect(effect);
            service.Update(0.5f);
            Assert.AreEqual(Speed + 1, _propertyComponent.GetValue(CharacterProperty.Speed));
            service.Update(1);
            Assert.AreEqual(Speed, _propertyComponent.GetValue(CharacterProperty.Speed));
        }

    }

}