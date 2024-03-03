using Core;
using Game.Properties;
using Game.Shared;

namespace Game.Conditions.Implementations
{
    /// <summary>
    /// Проверка определенного свойства персонажа на соответствие условию.
    /// </summary>
    public class CheckProperty : CheckCondition
    {
        private readonly PropertyHolder _propertyHolder;
        private readonly CharacterProperty _characterProperty;
        private readonly TypeCompare _typeCompare;
        private readonly float _value;
        
        public CheckProperty(PropertyHolder propertyHolder, CharacterProperty characterProperty, TypeCompare typeCompare, float value)
        {
            _propertyHolder = propertyHolder;
            _characterProperty = characterProperty;
            _typeCompare = typeCompare;
            _value = value;
        }

        public override bool Check()
        {
            return _propertyHolder.GetValue(_characterProperty).CheckCompareIsTrue(_typeCompare, _value);
        }
    }
}