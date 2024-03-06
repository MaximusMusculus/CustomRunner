using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Game.Shared;

namespace Game.Properties
{
    public class BaseProperty
    {
        public float BaseValue { get; }
        public float BaseMultiplier { get; }

        public BaseProperty(float baseValue, float baseMultiplier)
        {
            BaseValue = baseValue;
            BaseMultiplier = baseMultiplier;
        }
    }
    

    public class ModifiedProperty : BaseProperty
    {
        private readonly List<BaseProperty> _modifiers = new List<BaseProperty>();
        private float _finalValue;

        public ModifiedProperty(float baseValue, float baseMultiplier = 0) : base(baseValue, baseMultiplier)
        {
        }

        public void AddModifier(BaseProperty modifier)
        {
            _modifiers.Add(modifier);
        }

        public void RemoveModifier(BaseProperty modifier)
        {
            _modifiers.Remove(modifier);
        }

        public float GetValue()
        {
            _finalValue = BaseValue;

            var modifyValue = _modifiers.Sum(s => s.BaseValue);
            var modifyMultiplier = _modifiers.Sum(s => s.BaseMultiplier);

            _finalValue += modifyValue;
            _finalValue *= 1 + modifyMultiplier;
            return _finalValue;
        }
    }
    


    public interface IPropertyComponent 
    {
        float GetValue(CharacterProperty characterProperty);
        bool Has(CharacterProperty characterProperty);

        void AddModifier(CharacterProperty characterProperty, BaseProperty modifier);
        void RemoveModifier(CharacterProperty characterProperty, BaseProperty modifier);
    }
    
    
    /// <summary>
    /// Контейнер свойств объекта с возможностью добавлять модификаторы
    /// </summary>
    public class PropertyComponent : IPropertyComponent
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

}