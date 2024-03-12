using System.Collections.Generic;
using System.Linq;

namespace Game.Properties
{
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
}