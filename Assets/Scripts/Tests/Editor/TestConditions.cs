using Core;
using Game.Conditions.Implementations;
using Game.Properties;
using Game.Shared;
using NUnit.Framework;


namespace TestConditions
{
    /*
    public class DefinitionBase
    {
        
    }

    public class DefinitionCountDown
    {
        public float time;
    }
    
    public class DefinitionCheckProperty
    {
        public PropertyType propertyType;  //propertyType
        public TypeCompare typeCompare;
        public float value;
    }
    public class DefinitionCheckInventory
    {
        public int itemId;                  //itemType, abilityType, etc
        public TypeCompare typeCompare;
        public float count;
    }
    
    public class TriggerFactory
    {
        private const string ParamTime = "time";
        private const string ParamPropertyType = "propertyType";
        private const string ParamTypeCondition = "typeCondition";
        private const string ParamValue = "value";
        private const string ParamItemType = "itemType";
        
        
        private ITimeProvider _timeProvider;
        //private ITrigger _triggerAlwayTrue = new AlwaysTrueTrigger();
        
        public ICondition Create(ConditionType type, object definition) //keyValueStorage or concreteClass?
        {
            return type switch
            {
                ConditionType.CountDownTimer => new CountDownTimer(_timeProvider, values[ParamTime]), //orCast
                ConditionType.CheckInventory => new ConditionInventoryCondition(), //orCast
                ConditionType.CheckProperty => new PropertyChecker(values[ParamPropertyType],), //orCast
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
        
        
        private ICondition CreateCountDownTimer(Dictionary<string, object> values)
        {
            return new CountDownTimer(_timeProvider, values[ParamTime]);
        }
    }
    */

    public class CheckersService
    {
        //GetChecker(config)
        //
    }
    
    
    public class TestConditions
    {
        private PropertyHolder _holder;

        [SetUp]
        public void Setup()
        {
            _holder = new PropertyHolder();
            _holder.AddProperty(CharacterProperty.Speed, 5);
            _holder.AddProperty(CharacterProperty.FlyHeight, 0);
        }
        
        [Test]
        public void TestCheckPropertyCondition()
        {
            var condition = new CheckProperty(_holder, CharacterProperty.Speed, TypeCompare.GreaterThan, 5);
            Assert.IsFalse(condition.Check());

            var modifier = new BaseProperty(1, 0);
            _holder.AddModifier(CharacterProperty.Speed, modifier);
            Assert.IsTrue(condition.Check());
            _holder.RemoveModifier(CharacterProperty.Speed, modifier);//можон сократить
            Assert.IsFalse(condition.Check());
        }
        
        [Test] 
        public void TestCountDownTimer()
        {
            var condition = new CheckElapsedTime(1);
            Assert.IsFalse(condition.Check());
            condition.Update(0.5f);
            Assert.IsFalse(condition.Check());
            condition.Update(0.5f);
            Assert.IsTrue(condition.Check());
        }
        

    }
}