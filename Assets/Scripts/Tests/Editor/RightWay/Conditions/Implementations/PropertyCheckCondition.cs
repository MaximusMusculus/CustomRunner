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
        private readonly TypeProperty _typeProperty;
        private readonly TypeCompare _typeCompare;
        private readonly float _value;
        
        public CheckProperty(PropertyComponent propertyComponent, TypeProperty typeProperty, TypeCompare typeCompare, float value)
        {
            _propertyComponent = propertyComponent;
            _typeProperty = typeProperty;
            _typeCompare = typeCompare;
            _value = value;
        }

        public override bool Check()
        {
            return _propertyComponent.GetValue(_typeProperty).CheckCompareIsTrue(_typeCompare, _value);
        }
    }
}