using Game.Components;
using Game.Shared;

namespace Game.Properties
{
    public interface IPropertyComponent : IComponent
    {
        float GetValue(TypeProperty typeProperty);
        bool Has(TypeProperty typeProperty);

        void AddModifier(TypeProperty typeProperty, BaseProperty modifier);
        void RemoveModifier(TypeProperty typeProperty, BaseProperty modifier);
    }
}