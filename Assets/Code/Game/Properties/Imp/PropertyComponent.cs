using System;
using System.Collections.Generic;
using Game.Shared;

namespace Game.Properties
{
    /// <summary>
    /// Контейнер свойств объекта с возможностью добавлять модификаторы
    /// </summary>
    public class PropertyComponent : IPropertyComponent
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


    }
}