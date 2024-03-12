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
}