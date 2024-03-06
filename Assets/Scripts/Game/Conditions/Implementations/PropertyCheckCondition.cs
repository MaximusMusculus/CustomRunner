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
        private readonly PropertyComponent _propertyComponent;
        private readonly CharacterProperty _characterProperty;
        private readonly TypeCompare _typeCompare;
        private readonly float _value;
        
        public CheckProperty(PropertyComponent propertyComponent, CharacterProperty characterProperty, TypeCompare typeCompare, float value)
        {
            _propertyComponent = propertyComponent;
            _characterProperty = characterProperty;
            _typeCompare = typeCompare;
            _value = value;
        }

        public override bool Check()
        {
            return _propertyComponent.GetValue(_characterProperty).CheckCompareIsTrue(_typeCompare, _value);
        }
    }
}